using System;
using System.Collections.Generic;

namespace Huan.Framework.Core.UI.ViewModel
{
    public abstract class BaseViewModel : IViewModel, IBindAttribute
    {
        protected List<Delegate> Bindings = new();
        protected event Action OnClear;

        public void Clear()
        {
            OnClear?.Invoke();
        }

        public void OnBindProperty<T>(BindableProperty<T> property)
        {
            OnClear += property.Clear;
        }
    }
}