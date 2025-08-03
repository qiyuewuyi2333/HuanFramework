using System;
using Huan.Framework.Core.UI.View;
using Huan.Framework.Core.UI.ViewModel;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Huan.Framework.Examples.UI.ToggleTip
{
    public class ToggleTipView : BaseView<ToggleTipViewModel, ToggleTipController>
    {
        [SerializeField] private Toggle toggle;
        [SerializeField] private TextMeshProUGUI textNum;

        // 双向数据流
        public event Action<bool> OnToggleValueChanged;

        public override void InitEvents()
        {
            toggle.onValueChanged.AddListener(newValue => OnToggleValueChanged?.Invoke(newValue));
        }

        protected override void ShowUI(ToggleTipViewModel vm)
        {
                IBindAttribute bindAttribute = vm;
                bindAttribute.BindOnValueChanged(vm.Num, OnNumChanged);
        }

        private void OnNumChanged(int num)
        {
            textNum.text = num.ToString();
        }

        protected override void HideUI()
        {
        }
    }
}