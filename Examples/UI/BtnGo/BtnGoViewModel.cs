using System;
using Huan.Framework.Core.UI.ViewModel;

namespace Huan.Framework.Examples.UI.BtnGo
{
    public class BtnGoViewModel: BaseViewModel
    {
        // Data from model to UI
        public BindableProperty<int> Num = new(0);
        
        // If model want to get data from ui, then it must register a callback itself.
        // Data from UI to model
        public Action<int> OnNumChange;

        public BtnGoViewModel()
        {
            
        }
    }
}