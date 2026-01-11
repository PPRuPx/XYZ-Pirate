using UnityEngine;

namespace Model.Data.Properties
{
    public abstract class PersistentProperty<T>
    {
        [SerializeField] protected T _value;
        protected T _stored;
        protected T _defaultValue;

        public delegate void OnPropertyChanged(T newValue, T oldValue);

        public event OnPropertyChanged OnChanged;

        public PersistentProperty(T defaultValue)
        {
            _defaultValue = defaultValue;
        }

        public T Value
        {
            get => _stored;
            set
            {
                var isEquals = _stored.Equals(value);
                if (isEquals) return;

                var oldValue = _stored;
                Write(value);
                _stored = _value = value;
                
                OnChanged?.Invoke(value, oldValue);
            }
        }

        protected void Init()
        {
            _stored = _value = Read(_defaultValue);
        }
        
        protected abstract void Write(T value);
        protected abstract T Read(T defaultValue);
        
        public void Validate()
        {
            if (!_stored.Equals(_value))
                Value = _value;
        }
    }
}