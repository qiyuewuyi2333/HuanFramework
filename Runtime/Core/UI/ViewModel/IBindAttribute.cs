using System;

namespace Huan.Framework.Core.UI.ViewModel
{
    public interface IBindAttribute
    {
        /// <summary>
        /// 在每次绑定完属性之后调用
        /// </summary>
        public void OnBindProperty<T>(BindableProperty<T> property);

        public void BindOnValueChanged<T>(BindableProperty<T> property, Action<T> callback)
        {
            callback(property.Value);
            property.BindOnValueChanged(callback);
            OnBindProperty(property);
        }
    }
}