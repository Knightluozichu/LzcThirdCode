using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RedRedJiang.Unity
{
    public class BgDefaultSystem : SystemBase<BgDefaultSystem>
    {
        //#region 单例
        //private static BgDefaultSystem _Instance;
        //public static BgDefaultSystem Instance
        //{
        //    get
        //    {
        //        if (_Instance == null)
        //        {
        //            _Instance = Activator.CreateInstance<BgDefaultSystem>();
        //        }

        //        return _Instance;
        //    }
        //}
        //#endregion


        private const string mBg_Trans_DefaultName = "Goj_Trans_Default";
        private const string mBg_CameraName = "Bg_Camera";

        private BgDefaultBase mBgDefaultPanel;

        private Dictionary<BgDefaultBase, bool> mDicBgShowUIFrom = new Dictionary<BgDefaultBase, bool>();

        private Canvas mBgCanvas_Canvas;

        /// <summary>
        /// BgCanvas
        /// </summary>
        public Canvas BgCanvas_Canvas
        {
            get
            {
                if (mBgCanvas_Canvas == null)
                {
                    mBgCanvas_Canvas = UnityTool.CreateGameObject(CommonClass.mRootNodeDefalut, UITool.RootNodeTrans.Find(mBg_Trans_DefaultName)).GetComponent<Canvas>();
                }
                return mBgCanvas_Canvas;
            }
        }

        private Camera mCameraBgAdaptive;
        public Camera CameraBgAdaPtive
        {
            get
            {
                if (null == mCameraBgAdaptive)
                {
                    mCameraBgAdaptive = UITool.FindChild<Camera>(BgCanvas_Canvas.gameObject, mBg_CameraName);
                }

                return mCameraBgAdaptive;
            }
        }

        private StartPanelBg mStartPanelBgDefault = new StartPanelBg();
        private SecondPanelBg mSecondPanelBgDefault = new SecondPanelBg();

        public void BgDefaultInitAwake()
        {
            SetCanvasScalerScript();
            mDicBgShowUIFrom.Add(mStartPanelBgDefault, false);
            mDicBgShowUIFrom.Add(mSecondPanelBgDefault, false);
        }

        public void Open_Bg_Trans_Default(string bgDefaultNamePanel)
        {
            Debug.Log(bgDefaultNamePanel);
            ///检查参数
            ///检查集合里是否存在
            ///检查要打开的背景是否已经打开
            ///加载对象 给对象赋值 
            ///背景面板初始化
            if (string.IsNullOrEmpty(bgDefaultNamePanel))
            {
                return;
            }

            bool isShow = false;

            if (!CheckDicShowUIFrom(bgDefaultNamePanel, out isShow))
            {
                return;
            }

            if (isShow || mBgDefaultBase == null)
            {
                return;
            }

            if (mLastBgDefaultBase != null)
            {
                mDicBgShowUIFrom[mLastBgDefaultBase] = false;
            }

            mBgDefaultBase.Goj_Bg_Panel = UnityTool.CreateGameObject(bgDefaultNamePanel, BgCanvas_Canvas.transform);

            mBgDefaultBase.BgDefaultInit();

            mDicBgShowUIFrom[mBgDefaultBase] = true;

        }
        private void Close_Bg_Trans_Default()
        {
            if (mBgDefaultBase == null)
            {
                return;
            }


            mBgDefaultBase.BgDefaultEnd();
        }

        private BgDefaultBase mBgDefaultBase = null;
        private BgDefaultBase mLastBgDefaultBase = null;
        private bool CheckDicShowUIFrom(string bgDefaultNamePanel, out bool isShow)
        {
            isShow = false;
            bool isHas = false;

            foreach (KeyValuePair<BgDefaultBase, bool> i in mDicBgShowUIFrom)
            {
                if (i.Key.BgDefaultPanelName.Equals(bgDefaultNamePanel))
                {
                    isHas = true;
                    isShow = i.Value;
                    if (mBgDefaultBase != null)
                    {
                        Close_Bg_Trans_Default();
                    }
                    mLastBgDefaultBase = mBgDefaultBase;
                    mBgDefaultBase = i.Key;
                }
            }

            //Debug.Log("isHas--->" + isHas + "isShow--->" + isShow);
            return isHas;
        }



        //1.0  
        //1.2  
        //1.3    155
        //1.4    154
        //1.5    152.5
        //1.6    151
        //1.7    149.5
        //1.8    147.5
        //1.9    146
        //2.0    144.5
        //2.1    142.5
        #region 适配
        private Dictionary<float, float> mDicAdaptiveOfViewMap;
        public Dictionary<float, float> DicAdaptiveOfViewMap
        {
            get
            {
                if (null == mDicAdaptiveOfViewMap)
                {
                    mDicAdaptiveOfViewMap = new Dictionary<float, float>();
                    mDicAdaptiveOfViewMap.Add(1.0f, 155f);
                    mDicAdaptiveOfViewMap.Add(1.1f, 155f);
                    mDicAdaptiveOfViewMap.Add(1.2f, 155f); 
                    mDicAdaptiveOfViewMap.Add(1.3f, 155f);
                    mDicAdaptiveOfViewMap.Add(1.4f, 154f);
                    mDicAdaptiveOfViewMap.Add(1.5f, 152.5f);
                    mDicAdaptiveOfViewMap.Add(1.6f, 150.5f);
                    mDicAdaptiveOfViewMap.Add(1.7f, 149.5f);
                    mDicAdaptiveOfViewMap.Add(1.8f, 147.5f);
                    mDicAdaptiveOfViewMap.Add(1.9f, 146f);
                    mDicAdaptiveOfViewMap.Add(2.0f, 144.5f);
                    mDicAdaptiveOfViewMap.Add(2.1f, 142.5f);
                    mDicAdaptiveOfViewMap.Add(2.2f, 141f);
                    mDicAdaptiveOfViewMap.Add(2.3f, 139.5f);
                }

                return mDicAdaptiveOfViewMap;
            }
        }

        private void SetCanvasScalerScript()
        {
            float screenW = Screen.width;
            float screenH = Screen.height;

            float mScreenProportion = screenW / screenH;

            mScreenProportion = (float)Math.Round(mScreenProportion, 1);

            float matchWidthOrHeight = 0;

            if (mScreenProportion > 1.7f)
            {
                matchWidthOrHeight = 1;
            }

            BgCanvas_Canvas.GetComponent<CanvasScaler>().matchWidthOrHeight = matchWidthOrHeight;

            CameraBgAdaPtive.fieldOfView = DicAdaptiveOfViewMap[mScreenProportion];
        }

        #endregion
    }
}
