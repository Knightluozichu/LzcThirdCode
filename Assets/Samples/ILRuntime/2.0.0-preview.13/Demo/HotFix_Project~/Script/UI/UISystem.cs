using System;
using System.Collections.Generic;
using UnityEngine;

/*
 * @author LuoZichu
 * @time 2019/7/1
 */

namespace RedRedJiang.Unity
{
    public class UISystem : SystemBase<UISystem>, ISystem
    {
        //#region 单例
        //private static UISystem _Instance;
        //public static UISystem Instance
        //{
        //    get
        //    {
        //        if (_Instance == null)
        //        {
        //            _Instance = Activator.CreateInstance<UISystem>();
        //        }

        //        return _Instance;
        //    }
        //}
        //#endregion

        #region UIBase

        /// <summary>
        /// 总的UI窗体集合
        /// </summary>
        private Dictionary<string, UIBase> mDicUIBase = new Dictionary<string, UIBase>();

        /// <summary>
        /// 显示集合
        /// </summary>
        private Dictionary<UIBase, bool> mDicIsUIShow = new Dictionary<UIBase, bool>();
        public Transform UICanvas { get { return UITool.GetUICanvas().transform; } }
        public Transform Tran_FullNode { get { return UnityTool.FindChild(UICanvas.gameObject, CommonClass.mTran_FullNodeName).transform; } }
        public Transform Tran_ReversalNode { get { return UnityTool.FindChild(UICanvas.gameObject, CommonClass.mTran_ReversalNodeName).transform; } }
        public Transform Tran_PopNode { get { return UnityTool.FindChild(UICanvas.gameObject, CommonClass.mTran_PopNodeName).transform; } }

        private const string mUI_CameraName = "UI_Camera";

        private Camera mUiCamera;
        public Camera UICamera { get { return mUiCamera = mUiCamera ?? UITool.FindChild<Camera>(UISystem.Instance.UICanvas.gameObject, mUI_CameraName); } }

        #endregion

        /// <summary>
        /// 各个UI界面 实例
        /// </summary>
        #region Field 
  

        private Canvas mUICanvas_Canvas;

        /// <summary>
        /// UICanvas
        /// </summary>
        public Canvas UICanvas_Canvas
        {
            get
            {
                if(mUICanvas_Canvas == null)
                {
                    mUICanvas_Canvas = UnityTool.CreateGameObject(CommonClass.mRootNodeName, UITool.RootNodeTrans.Find(mGoj_Trans_UIName)).GetComponent<Canvas>();
                }
                return mUICanvas_Canvas;
            }
        }

        private const string mGoj_Trans_UIName = "Goj_Trans_UI";

        private StartPanelView mStartPanel = new StartPanelView();
        private SecondPanelView mSecondPanel = new SecondPanelView();
        #endregion

        #region Method
        #region Public
        public UISystem()
        {
            mSystemName = CommonClass.mUISystemName;
        }

        public void InitAwake()
        {
            //节点初始化 mSecondPanel

            #region UI窗体添加总集合
            mDicUIBase.Add(CommonClass.mStartPanelName, mStartPanel);
            mDicUIBase.Add(CommonClass.mSecondPanelName, mSecondPanel);
            #endregion

            #region UI显示窗体 添加
            mDicIsUIShow.Add(mStartPanel, false);
            mDicIsUIShow.Add(mSecondPanel, false);
            #endregion
        }

        public void Init()
        {
            InitAwake();
            BgDefaultSystem.Instance.BgDefaultInitAwake();

        }

        #region UI窗体注销
        public void EndUI()
        {
            mDicUIBase.Clear();
            mStartPanel = null;
            mSecondPanel = null;
        }
        #endregion

        //如果这里参数改成 UIBase 即为多肽，改为UIBase后却无法管理 是否已经显示问题
        public UIBase OpenUIFrom(string _UIName) 
        {
            if (string.IsNullOrEmpty(_UIName))
            {
                Debug.Log(_UIName + "is null!");
                return null;
            }

            UIBase mUIbase = null;

            if (mDicUIBase.TryGetValue(_UIName, out mUIbase))
            {

                if (mDicIsUIShow[mUIbase]) // 状态为改变 （这地方因为没有什么内容，只是简单的逻辑关系，所以可以直接设置状态，就可以解决，假如有一些代码以外资源相关的事情存在，就不可以） 
                {
                    //mDicIsUIShow[mUIbase] = false;
                    Debug.Log(_UIName + "已经在显示集合中了"); return mUIbase;
                }

                GameObject mUIFromGob = LoadUIForm(_UIName, mUIbase);//加载界面预设 放在对应的节点下

                if (mUIFromGob)
                {
                    mUIbase.GojPanel = mUIFromGob;

                    mDicIsUIShow[mUIbase] = true;

                    OpenThisUIFrom(mUIbase, mUIFromGob, _UIName);
                }

                return mUIbase;
            }

            return null;
        }


        public UIBase CloseUIFrom(string _UIName)
        {
            if (string.IsNullOrEmpty(_UIName)) { return null; }

            UIBase mUIBase = null;

            if (mDicUIBase.TryGetValue(_UIName, out mUIBase))
            {
                if (!mDicIsUIShow[mUIBase]) { Debug.Log(_UIName + "并没有显示这个UI"); return null; }

                GameObject _UIFromGob = UnityTool.FindChild(Tran_FullNode.gameObject, _UIName);

                mDicIsUIShow[mUIBase] = false;

                CloseThisUIFrom(mUIBase, _UIFromGob);

            }
            else
            {
                Debug.Log(_UIName + "对应的UIBase不存在！");
                return null;
            }

            //mUIBase.GojPanel = null;

            return mUIBase;
        }

        #endregion

        #region Private

        private GameObject LoadUIForm(string _UIName, UIBase _UIBase)
        {
            GameObject _UINormal = null;
            GameObject _UINormal0 = ResourcesSystem.Instance.ResObj<GameObject>(_UIName, true);
            if (_UINormal0 != null)
            {
                _UINormal = UnityEngine.Object.Instantiate(_UINormal0);
                if (_UINormal != null)
                {
                    switch (_UIBase.UIForm_Pos)
                    {
                        case UIFormPos.Normal:
                            _UINormal.transform.SetParent(Tran_FullNode, false);
                            break;
                        case UIFormPos.Reversal:
                            _UINormal.transform.SetParent(Tran_ReversalNode, false);
                            break;
                        case UIFormPos.Pop:
                            _UINormal.transform.SetParent(Tran_PopNode, false);
                            break;
                    }
                }
                else
                {
                    Debug.Log(_UINormal + "is null,_UIName 加载不出来！");
                    return null;
                }

                _UINormal.transform.localPosition = Vector3.zero;
                _UINormal.transform.localRotation = Quaternion.identity;
                _UINormal.transform.localScale = Vector3.one;
                _UINormal.name = _UIName;
                _UINormal.SetActive(false);
            }
            
            return _UINormal;
        }

        private void OpenThisUIFrom(UIBase _UIbase, GameObject _UIFromGob,string uiName)
        {
            _UIbase.ShowUIForm(uiName, _UIFromGob);
            _UIbase.UIInit();
        }

        private void CloseThisUIFrom(UIBase _UIbase, GameObject _UIFromGob)
        {
            _UIbase.CloseUIFrom(_UIFromGob);
            //_UIbase.UIEnd();
        }
        #endregion
        #endregion


    }
}
