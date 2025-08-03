using System;

namespace Huan.Framework.Core.UI.ViewModel
{
    public class BindableProperty<T>
    {
        public T Value
        {
            get => _value;
            set
            {
                IsActive = true;
                if (!object.Equals(_value, value))
                {
                    _value = value;
                    OnValueChanged?.Invoke(_value);
                }
            }
        }
        public void BindOnValueChanged(Action<T> callback)
        {
            IsActive = true;
            OnValueChanged += callback;
        }
        public void Clear()
        {
            if (!IsActive) return;
            
            _value = default;
            OnValueChanged = null;
            IsActive = false;
        }

        private T _value;
        public bool IsActive = false;
        public event Action<T> OnValueChanged;

        public BindableProperty()
        {
            _value = default(T);
        }

        public BindableProperty(T value)
        {
            _value = value;
        }

        public static implicit operator BindableProperty<T>(bool v)
        {
            throw new NotImplementedException();
        }
    }
}