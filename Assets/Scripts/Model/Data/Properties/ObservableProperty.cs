using UnityEngine;

namespace Model.Data.Properties
{
    public class ObservableProperty<T>
    {
        [SerializeField] private T _value;
        
        public delegate void OnPropertyChanged(T newValue, T oldValue);

        public event OnPropertyChanged OnChanged;

        public T Value
        {
            get => _value;
            set
            {
                var isSame = _value.Equals(value);
                if (isSame) return;

                var oldValue = _value;
                _value = value;
                
                OnChanged?.Invoke(_value, oldValue);
            }
        }
    }
}