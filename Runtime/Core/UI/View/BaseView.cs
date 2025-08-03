using System;
using Huan.Framework.Core.UI.Controller;
using Huan.Framework.Core.UI.ViewModel;
using UnityEngine;

namespace Huan.Framework.Core.UI.View
{
    public abstract class BaseView<TViewModel, TController> : MonoBehaviour, IView where TViewModel : BaseViewModel where TController : BaseController, new()
    {
        protected TViewModel _viewModel = null;
        protected TController _controller = null;

        private bool _isShowing = false;
        private bool _isInit = false;

        public event Action<IView> OnShow;
        public event Action<IView> OnHide;

        public void Init()
        {
            if (_isInit) return;

            InitEvents();
            _controller = new TController();
            _isInit = true;
        }

        public abstract void InitEvents();

        public void Show(BaseViewModel vm)
        {
            if (_isShowing) return;

            if (vm is not TViewModel viewModel)
            {
                Debug.LogError($"ViewModel not match. UI Name: {gameObject.name}");
                return;
            }
                
            _viewModel = viewModel;
            ShowUI(_viewModel);

            _isShowing = true;
            OnShow?.Invoke(this);
        }

        private void UnbindCurViewModel()
        {
            _viewModel.Clear();
            _viewModel = null;
        }

        /// <summary>
        /// 自定义逻辑 通常 数据绑定
        /// </summary>
        /// <param name="vm"></param>
        protected abstract void ShowUI(TViewModel vm);

        /// <summary>
        /// 自动清理绑定
        /// </summary>
        public void Hide()
        {
            if (!_isShowing)
            {
                Debug.LogError("我都没有showing，你还想关我？");
                return;
            }
            HideUI();
            UnbindCurViewModel();
            _isShowing = false;
            OnHide?.Invoke(this);
            Destroy(gameObject);
        }

        /// <summary>
        /// 自定义逻辑 如关闭UI的时候发送事件
        /// </summary>
        protected abstract void HideUI();
    }
}