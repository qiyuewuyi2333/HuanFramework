using Huan.Framework.Core.UI.View;
using UnityEngine;

namespace Huan.Framework.Core.UI
{
    public class UINode
    {
        public IView View = null;
        public UIName Name = UIName.None;

        public UINode NextNode = null;
        public UINode prevNode = null;

        public UINode()
        {
            View = null;
            NextNode = null;
            prevNode = null;
        }
    }

    public class UIList
    {
        private UINode _dummyHead;
        private UINode _currentNode;
        private UINode _dummyTail;
        private int _uiCount;

        public int Count => _uiCount;

        public UIList()
        {
            _dummyHead = new UINode();
            _dummyHead.Name = UIName.DummyHead;
            _dummyTail = new UINode();
            _dummyTail.Name = UIName.DummyTail;

            _dummyHead.prevNode = _dummyTail;
            _dummyHead.NextNode = _dummyTail;
            _dummyTail.prevNode = _dummyHead;
            _dummyTail.NextNode = _dummyHead;

            _currentNode = _dummyHead;
            _uiCount = 0;
        }

        /// <summary>
        /// 栈式向后添加
        /// </summary>
        /// <param name="uiName"></param>
        /// <param name="view"></param>
        public void Add(UIName uiName, IView view)
        {
            UINode node = new UINode();
            node.View = view;
            node.Name = uiName;
            node.NextNode = _currentNode.NextNode;
            node.prevNode = _currentNode;
            _currentNode.NextNode.prevNode = node;
            _currentNode.NextNode = node;

            _currentNode = node;
            _uiCount++;
        }

        public UINode Remove(IView view)
        {
            UINode node = _dummyHead.NextNode;
            while (node != _dummyTail)
            {
                if (node.View == view)
                {
                    if (node == _currentNode)
                    {
                        _currentNode = _currentNode.prevNode;
                    }
                    node.prevNode.NextNode = node.NextNode;
                    node.NextNode.prevNode = node.prevNode;
                    _uiCount--;
                    Debug.Log("UINode Remove successfully. ");
                    return node;
                }

                node = node.NextNode;
            }

            Debug.Log("UINode Remove failed.");
            return null;
        }

        public UINode Remove(UIName name)
        {
            UINode node = _dummyHead.NextNode;
            while (node != _dummyTail)
            {
                if (node.Name == name)
                {
                    if (node == _currentNode)
                    {
                        _currentNode = _currentNode.prevNode;
                    }
                    node.prevNode.NextNode = node.NextNode;
                    node.NextNode.prevNode = node.prevNode;
                    _uiCount--;
                    Debug.Log("UINode Remove successfully. ");
                    return node;
                }

                node = node.NextNode;
            }

            Debug.Log("UINode Remove failed.");
            return null;
        }

        public UINode RemoveLast()
        {
            var node = _currentNode;
            _currentNode = _currentNode.prevNode;
            if (node == _dummyHead)
            {
                Debug.LogError("UINode Remove failed.");
                return null;
            }

            node.prevNode.NextNode = node.NextNode;
            node.NextNode.prevNode = node.prevNode;
            _uiCount--;
            Debug.Log("UINode Remove successfully. ");
            return node;
        }
        public bool Contains(UIName name)
        {
            UINode node = _dummyHead.NextNode;
            while (node != _dummyTail)
            {
                if (node.Name == name)
                {
                    return true;
                }

                node = node.NextNode;
            }

            return false;
        }
    }
}