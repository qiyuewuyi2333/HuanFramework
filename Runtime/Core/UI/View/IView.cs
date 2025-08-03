using Huan.Framework.Core.UI.Controller;
using Huan.Framework.Core.UI.ViewModel;

namespace Huan.Framework.Core.UI.View
{
    public interface IView
    {
        public void Init();
        public void Show(BaseViewModel vm);
        public void Hide();
    }
}