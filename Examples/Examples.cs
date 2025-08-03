using System.Collections;
using Huan.Framework.Core.Data;
using Huan.Framework.Core.Event;
using Huan.Framework.Core.UI;
using Huan.Framework.Examples.UI.BtnGo;
using Huan.Framework.Examples.UI.ToggleTip;
using Huan.Framework.Runtime.Core.Asset;
using Huan.Game.Card;
using UnityEngine;

namespace Huan.Framework.Examples
{
    public class Examples : MonoBehaviour
    {
        private EventListener _eventListener;

        [SerializeField] private HandView handView;

        private void Start()
        {
            _eventListener = new EventListener();
            DontDestroyOnLoad(this.gameObject);
        }

        private void OnTestEvent(int value)
        {
            Debug.Log("接收到事件：" + value);
        }

        // [ContextMenu("测试卡牌")]
        // private void TestCard()
        // {
        //     for (int i = 0; i < 5; i++)
        //     {
        //         CardViewModel card = new(DataManager.Tables.TbCardConfig.Get(1));
        //         CardView cardView =
        //             CardSystemView.Instance.CardViewCreator.CreateCardView(card, transform.position, Quaternion.identity, this.transform);
        //         StartCoroutine(handView.AddCard(cardView));
        //     }
        // }

        [ContextMenu("测试场景")]
        private void TestScene()
        {
            UIManager.AddUIAsync(UIName.BtnGoMenu, new BtnGoViewModel());
        }

        [ContextMenu("测试UI出现")]
        private void TestUIShow()
        {
            var vm = new ToggleTipViewModel();
            UIManager.AddUIAsync(UIName.ToggleTip, vm);
            StartCoroutine(AddVMNum(vm));
        }

        private IEnumerator AddVMNum(ToggleTipViewModel vm)
        {
            while (true)
            {
                vm.Num.Value++;
                if (vm.Num.Value > 10)
                    yield break;
                yield return new WaitForSeconds(1);
            }
        }

        [ContextMenu("测试UI隐藏")]
        private void TestUIHide()
        {
            UIManager.BackUI();
        }

        [ContextMenu("测试EventSystem 发布事件")]
        private void TestEventSystem()
        {
            _eventListener.RegisterEvent<int>(EventConstType.TestEvent, OnTestEvent);
            EventManager.FireEvent(EventConstType.TestEvent, 100);
            _eventListener.UnRegisterEvent(EventConstType.TestEvent);
            EventManager.FireEvent(EventConstType.TestEvent, 100);
            _eventListener.RegisterEvent<int, float>(EventConstType.TestEvent2,
                ((i, f) => { Debug.Log($"TestEvent2 Res: {f * i}"); }));
            EventManager.FireEvent(EventConstType.TestEvent2, 2, 5.5f);
        }

        [ContextMenu("测试Yooasset和Luban")]
        private void TestLubanAndYooasset()
        {
            var cubeAssetHandle =
                AssetManager.LoadAssetAsync<GameObject>("Assets/ResourcesAB/Test/Prefabs/Cube.prefab");
            cubeAssetHandle.Completed += handle =>
            {
                if (cubeAssetHandle.AssetObject is not GameObject cubePrefab)
                    return;

                var cubes = DataManager.Tables.TbCube.DataList;
                foreach (var cube in cubes)
                {
                    var ins = Instantiate(cubePrefab);
                    ins.transform.localPosition = new Vector3(cube.Position.X, cube.Position.Y, cube.Position.Z);
                    ins.name = cube.Name;
                }
            };
        }
    }
}