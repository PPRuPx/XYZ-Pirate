namespace Model.Data.Properties
{
    public abstract class PrefsPersistentProperty<T> : PersistentProperty<T>
    {
        protected string Key; 
        
        protected PrefsPersistentProperty(T defaultValue, string key) : base(defaultValue)
        {
            Key = key;
        }
    }
}