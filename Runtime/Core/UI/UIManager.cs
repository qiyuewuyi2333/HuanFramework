using System;
using System.Threading.Tasks;
using Huan.Framework.Core.UI.View;
using Huan.Framework.Core.UI.ViewModel;
using Huan.Framework.Runtime.Core.UI;
using UnityEngine;

namespace Huan.Framework.Core.UI
{
    public class UIManager : BaseManager<UIManager>
    {
        #region API

        public static UnityEngine.Camera UICamera
        {
            get => _instance._uiCamera;
        }

        public static void AddUIAsync(UIName name, BaseViewModel vm = null)
        {
            try
            {
                _ = _instance.AddUIAsyncInternal(name, vm);
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
                throw;
            }
        }

        public static Canvas Canvas => _instance._canvas;
        public static bool ContainsUI(UIName name)
        {
            return _instance.ContainsUIInternal(name);
        }

        public static void BackUI()
        {
            try
            {
                _instance.BackUIInternal();
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
                throw;
            }
        }

        public static void BackUI(UIName name)
        {
            try
            {
                _instance.BackUIInternal(name);
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
                throw;
            }
        }

        public static void BackAllUI()
        {
            try
            {
                _instance.BackAllUIInternal();
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
                throw;
            }
        }

        #endregion

        private UIList _uiList = new UIList();
        private UIFactory _uiFactory;
        private Transform _uiRoot;
        private Canvas _canvas;
        public UnityEngine.Camera _uiCamera;
        private UIName _creatingUI;

        private async Task AddUIAsyncInternal(UIName name, BaseViewModel vm)
        {
            _creatingUI = name;
            IView view = await _uiFactory.CreateUI(name);
            _uiList.Add(name, view);

            // 根据model创建ViewModel
            view.Init();
            view.Show(vm);
            _creatingUI = UIName.None;
        }

        private void BackUIInternal()
        {
            var node = _uiList.RemoveLast();
            if (node == null)
            {
                return;
            }

            var view = node.View;
            view.Hide();
            Debug.Log("BackUI successfully.");
        }

        private void BackUIInternal(UIName name)
        {
            var node = _uiList.Remove(name);
            if (node == null)
            {
                return;
            }

            var view = node.View;
            view.Hide();
            Debug.Log("BackUI successfully.");
        }

        protected override Task InitInternal()
        {
            _uiRoot = GameObject.Find("UIRoot").transform;
            _uiCamera = GameObject.Find("UICamera").GetComponent<UnityEngine.Camera>();
            _canvas = _uiRoot.Find("Canvas").GetComponent<Canvas>();
            _uiFactory = new UIFactory(_canvas);
            return Task.CompletedTask;
        }

        protected override void CleanupInternal()
        {
        }

        private void BackAllUIInternal()
        {
            while (_uiList.Count > 0)
            {
                BackUIInternal();
            }
        }
        
        private bool ContainsUIInternal(UIName name)
        {
            return _uiList.Contains(name);   
        }
    }
}