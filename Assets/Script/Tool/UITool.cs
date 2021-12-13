using UnityEngine;

/*
 * @author LuoZichu
 * @time 2019/7/1
 */

namespace RedRedJiang.Unity
{
    public static class UITool
    {
        private static Transform mRootNodeTrans;

        /// <summary>
        /// RootNode
        /// </summary>
        public static Transform RootNodeTrans
        {
            get
            {
                if (mRootNodeTrans == null)
                {
                    mRootNodeTrans = GameObject.Find("GameStart").transform;
                }
                return mRootNodeTrans;
            }
        }
        /// <summary>
        /// UICanvas
        /// </summary>
        /// <returns></returns>
        public static Canvas GetUICanvas()
        {
            return UISystem.Instance.UICanvas_Canvas;
        }

        /// <summary>
        /// BgCanvas
        /// </summary>
        /// <returns></returns>
        public static Canvas GetBgCanvas()
        {
            return BgDefaultSystem.Instance.BgCanvas_Canvas;
        }

        public static T FindChild<T>(GameObject _Parent, string _ChildName)
        {
            Transform mUIgob = _Parent.transform.Find(_ChildName);
            if (mUIgob != null)
            {
                return mUIgob.GetComponent<T>();
            }
            else
            {
                Debug.Log("要寻找的名字不存在：----->" + _ChildName);
                return default(T);
            }


        }

        /// <summary>
        /// 设置UI父节点
        /// </summary>
        /// <param name="child"></param>
        /// <param name="parent"></param>
        public static void SetParentUI(Transform child,Transform parent)
        {
            child.SetParent(parent, false);
            child.localPosition = Vector2.zero;
            child.localRotation = Quaternion.identity;
            child.localScale = Vector3.one;
        }
    }
}
