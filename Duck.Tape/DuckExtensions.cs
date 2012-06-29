namespace Duck.Tape
{
    public static class DuckExtensions
    {
        public static TInterface Duck<TInterface>(this object target) 
            where TInterface : class 
        {
            return new ClassFactory<TInterface>(target.GetType()).GetInstance(target);
        }
    }
}