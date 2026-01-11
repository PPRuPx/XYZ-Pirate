using System;
using UnityEngine;
using Utils.Disposables;

namespace Model.Data.Properties
{
    public class ObservableProperty<T>
    {
        [SerializeField] private T _value;
        
        public delegate void OnPropertyChanged(T newValue, T oldValue);

        public event OnPropertyChanged OnChanged;

        public IDisposable Subscribe(OnPropertyChanged call)
        {
            OnChanged += call;
            return new ActionDisposable(() => OnChanged -= call);
        }

        public IDisposable SubscribeAndInvoke(OnPropertyChanged call)
        {
            OnChanged += call;
            var dispose = new ActionDisposable(() => OnChanged -= call);
            call(_value, _value);
            return dispose;
        }
        
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