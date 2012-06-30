Duck.Tape
=
Sample Usage
-
    //class A does not implement ITest explicitly
    A a = new A();
    ITest aAsITest = a.Duck<ITest>();

What is it
-
Duck.Tape is actually one extension method on any reference type which allows the user of the library to cast an arbitrary class to an interface it implicitly implements. This means if a given class X has all the members of an interface Y, but X does not inherit from Y, it can still be converted to Y.

How it works
-
This library will generate a 'man-in-the-middle' class which implements the target interface explicitly. The implementation of the members in this class is just forwarding all calls to the actual type.