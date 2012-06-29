using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using BLToolkit.Reflection.Emit;

namespace Duck.Tape
{
    internal class ClassFactory<TInterface> where TInterface : class
    {
        private static AssemblyBuilderHelper _assemblyBuilder;
        private static Dictionary<Tuple<Type, Type>, Type> _inMemoryCache; 

        private Assembly _previouslyGeneratedAssembly;
        public Type InterfaceToImplement { get; set; }
        public Type ClassToWrap { get; set; }

        static ClassFactory()
        {
            _assemblyBuilder = new AssemblyBuilderHelper(GetGeneratedAssemblyPath());
            _inMemoryCache = new Dictionary<Tuple<Type, Type>, Type>();
        }

        private static string GetGeneratedAssemblyPath()
        {
            return @"C:\Users\pvps.SPHINXIT\Documents\GitHub\Duck.Tape\Duck.Tape.Sandbox\bin\Debug\Duck.Tape.Generated.dll";
        }

        ~ClassFactory()
        {
#if !DEBUG
            if(_previouslyGeneratedAssembly == null)
                try
                {
                    _assemblyBuilder.Save();
                }
                catch (Exception)
                {
                    return;
                }
#endif
        }

        public ClassFactory(Type classType)
        {
            InterfaceToImplement = typeof(TInterface);
            ClassToWrap = classType;
        }

        public TInterface GetInstance(object target)
        {
            return Generate().GetConstructors()[0].Invoke(new[] { target }) as TInterface;
        }

        private Type Generate()
        {
            if (_inMemoryCache.Any(x => x.Key.Item1 == InterfaceToImplement && x.Key.Item2 == ClassToWrap))
                return _inMemoryCache.First(x => x.Key.Item1 == InterfaceToImplement && x.Key.Item2 == ClassToWrap).Value;

#if !DEBUG
            //Type cache = null;
            //if (TryCheckCache(ref cache))
            //    return cache;
#endif
            var typeBuilder = _assemblyBuilder.DefineType(GetGeneratedTypeName(), typeof(object), InterfaceToImplement);

            //create private field
            var classToWrapField = typeBuilder.DefineField(GetGeneratedToWrapFieldName(), ClassToWrap, FieldAttributes.Private);

            //map interface members
            MapInterfaceProperties(classToWrapField, typeBuilder);
            MapInterfaceEvents(classToWrapField, typeBuilder);
            MapInterfaceMethods(classToWrapField, typeBuilder);

            //define constructor
            DefineConstructor(classToWrapField, typeBuilder);

            var generatedType = typeBuilder.Create();

            _inMemoryCache.Add(new Tuple<Type, Type>(InterfaceToImplement, ClassToWrap), generatedType);

            return generatedType;
        }

        private bool TryCheckCache(ref Type result)
        {
            try
            {
                //if we already have the assembly in memory, don't reload it
                if (_previouslyGeneratedAssembly == null)
                    _previouslyGeneratedAssembly = Assembly.LoadFile(new FileInfo(GetGeneratedAssemblyPath()).FullName);

                //find the concrete type for the given domaintype
                result = _previouslyGeneratedAssembly.GetTypes().FirstOrDefault(x => x.GetInterface(InterfaceToImplement.Name) != null);
                if(result != null)
                    return true;
            }
            catch (ReflectionTypeLoadException exception)
            {
                try
                {
                    result = exception.Types.FirstOrDefault(x => x != null && x.GetInterface(InterfaceToImplement.Name) != null);
                    if(result != null)
                        return true;
                }
                catch
                {
                    return false;
                }
            }
            return false;
        }

        private void MapInterfaceEvents(FieldBuilder classToWrapField, TypeBuilderHelper typeBuilder)
        {
            foreach (var @event in ClassToWrap.GetEvents())
            {
                var eventBuilder = typeBuilder.TypeBuilder.DefineEvent(@event.Name, @event.Attributes, @event.EventHandlerType);

                CreateEventAddMethod(@event, eventBuilder, classToWrapField, typeBuilder);
                CreateEventRemoveMethod(@event, eventBuilder, classToWrapField, typeBuilder);
            }
        }

        private void DefineConstructor(FieldInfo classToWrapField, TypeBuilderHelper typeBuilder)
        {
            typeBuilder
                .DefinePublicConstructor(ClassToWrap)
                .Emitter
                .ldarg_0
                .call(typeof(object).GetConstructors()[0])
                .ldarg_0
                .ldarg_1
                .stfld(classToWrapField)
                .ret();
        }

        private void MapInterfaceMethods(FieldInfo classToWrapField, TypeBuilderHelper typeBuilder)
        {
            //loop al interface methods
            foreach (var methodInfo in InterfaceToImplement.GetMethods())
            {
                //define the method in the generated type
                var emitter = typeBuilder.DefineMethod(methodInfo)
                                .Emitter
                                    //this.[fieldName]
                                    .ldarg_0
                                    .ldfld(classToWrapField);

                for (var index = 0; index < methodInfo.GetParameters().Length; index++)
                    //pops all method arguments on the stack, so we can use them to 
                    //call the targetMethod
                    emitter.ldarg(index + 1);

                emitter
                    //this.[fieldName].[MethodToImplement]([parameters])
                    .callvirt(GetMappedMethod(methodInfo))
                    //return
                    .ret();
            }
        }
        
        private void MapInterfaceProperties(FieldBuilder classToWrapField, TypeBuilderHelper typeBuilder)
        {
            foreach (var property in InterfaceToImplement.GetProperties())
            {
                var propertyBuilder = typeBuilder
                    .TypeBuilder
                    .DefineProperty(property.Name, PropertyAttributes.None, property.PropertyType, null);

                CreatePropertyGetMethod(property, propertyBuilder, classToWrapField, typeBuilder);
                CreatePropertySetMethod(property, propertyBuilder, classToWrapField, typeBuilder);
            }
        }

        private void CreatePropertyGetMethod(PropertyInfo p, PropertyBuilder property, FieldBuilder classToWrapField, TypeBuilderHelper typeBuilder)
        {
            //set setter name and return type
            var setterName = p.GetGetMethod().Name;
            var returnType = p.GetGetMethod().ReturnType;

            //determine security attributes
            var methodAttributes = MethodAttributes.Public | MethodAttributes.Virtual;

            //create setter method
            var getBuilder = typeBuilder.DefineMethod(setterName, methodAttributes, returnType, null);

            //emit IL opcode
            getBuilder
                .Emitter
                .nop
                .ldarg_0
                .ldfld(classToWrapField)
                .callvirt(ClassToWrap.GetProperty(p.Name, p.PropertyType, new Type[] { }).GetGetMethod())
                .ret();

            //apply the created method to the getter
            property.SetGetMethod(getBuilder);
        }
        
        private void CreatePropertySetMethod(PropertyInfo p, PropertyBuilder property, FieldBuilder classToWrapField, TypeBuilderHelper typeBuilder)
        {
            //set setter name and return type
            var setterName = p.GetSetMethod().Name;
            var returnType = p.GetSetMethod().ReturnType;

            //determine security attributes
            var methodAttributes = MethodAttributes.Public | MethodAttributes.Virtual;

            //create setter method
            var getBuilder = typeBuilder.DefineMethod(setterName, methodAttributes, null, returnType);

            //emit IL opcode
            getBuilder
                .Emitter
                .nop
                .ldarg_0
                .ldfld(classToWrapField)
                .ldarg_1
                .callvirt(ClassToWrap.GetProperty(p.Name, p.PropertyType, new Type[] { }).GetSetMethod())
                .ret();

            //apply the created method to the getter
            property.SetSetMethod(getBuilder);
        }

        private void CreateEventRemoveMethod(EventInfo eventInfo, EventBuilder eventBuilder, FieldBuilder classToWrapField, TypeBuilderHelper typeBuilder)
        {
            var removeMethodBuilder = typeBuilder.DefineMethod(eventInfo.GetAddMethod().Name, MethodAttributes.Public, eventInfo.GetAddMethod().ReturnType, eventInfo.GetAddMethod().GetParameters().Select(x => x.ParameterType).ToArray());

            removeMethodBuilder
                .Emitter
                .ldarg_0
                .ldfld(classToWrapField)
                .ldarg_1
                .callvirt(ClassToWrap.GetEvent(eventInfo.Name).GetRemoveMethod())
                .ret();
            
            eventBuilder.SetRemoveOnMethod(removeMethodBuilder);
        }

        private void CreateEventAddMethod(EventInfo eventInfo, EventBuilder eventBuilder, FieldBuilder classToWrapField, TypeBuilderHelper typeBuilder)
        {
            var addMethodBuilder = typeBuilder.DefineMethod(eventInfo.GetAddMethod().Name, MethodAttributes.Public, eventInfo.GetAddMethod().ReturnType, eventInfo.GetAddMethod().GetParameters().Select(x => x.ParameterType).ToArray());

            addMethodBuilder
                .Emitter
                .ldarg_0
                .ldfld(classToWrapField)
                .ldarg_1
                .callvirt(ClassToWrap.GetEvent(eventInfo.Name).GetAddMethod())
                .ret();

            eventBuilder.SetAddOnMethod(addMethodBuilder);
        }

        private string GetGeneratedToWrapFieldName()
        {
            return string.Format("_field_{0}", ClassToWrap.Name);
        }

        private string GetGeneratedTypeName()
        {
            return string.Format("{0}_as_{1}", ClassToWrap.Name, InterfaceToImplement.Name);
        }

        private MethodInfo GetMappedMethod(MethodInfo methodInfo)
        {
            var parameters = methodInfo.GetParameters();

            //reflection has a hard time discovering generic methods, so we have to do it ourselves
            if (methodInfo.IsGenericMethod)
                return (from mi in ClassToWrap.GetMethods()
                        where mi.Name == methodInfo.Name
                        where mi.IsGenericMethodDefinition
                        where mi.GetParameters().Length == parameters.Length
                        where mi.ToString() == methodInfo.ToString()
                        select mi).FirstOrDefault();

            //search for the non-generic method in the given logicClass
            return (from mi in ClassToWrap.GetMethods()
                    where mi.Name == methodInfo.Name
                    where mi.GetParameters().Length == parameters.Length
                    where mi.ReturnType == methodInfo.ReturnType
                    where CheckParameterOrder(mi, methodInfo)
                    select mi).FirstOrDefault();
        }

        private bool CheckParameterOrder(MethodInfo method1, MethodInfo method2)
        {
            var method1Parameters = method1.GetParameters();
            var method2Parameters = method2.GetParameters();

            if (method1Parameters.Count() != method2Parameters.Count())
                return false;

            for (int i = 0; i < method1Parameters.Count(); i++)
            {
                if (method1Parameters[i].ParameterType != method2Parameters[i].ParameterType)
                    return false;
            }

            return true;
        }
    }
}
