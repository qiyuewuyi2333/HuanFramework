using Huan.Framework.Core.Scene;
using Huan.Framework.Core.UI.View;
using UnityEngine;
using UnityEngine.UI;

namespace Huan.Framework.Examples.UI.BtnGo
{
    public class BtnGoView : BaseView<BtnGoViewModel, BtnGoController>
    {
        [SerializeField] private Button btn;

        // 双向数据流
        public override void InitEvents()
        {
            btn.onClick.AddListener(() =>
            {
                Debug.Log("点击了按钮");
                SceneManager.LoadScene("MainMenu");
            });
        }

        protected override void ShowUI(BtnGoViewModel vm)
        {
            
        }

        protected override void HideUI()
        {
        }
    }
}