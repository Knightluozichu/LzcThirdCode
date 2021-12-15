using System.Collections.Generic;
using UnityEngine;


/*
 * @author LuoZichu
 * @time 2019/7/1
 */

namespace RedRedJiang.Unity
{
    /// <summary>
    /// 外观模式 也是 消息中转中心
    /// </summary>
    public class GameFacade :Singleton<GameFacade> 
    {
        #region Message relay encapsulation

        Dictionary<string, IBaseNotify> mDic = new Dictionary<string, IBaseNotify>();

        #endregion

        private bool mIsAwake;

        #region Method
        #region Public
        public void InitAwake()
        {
            mIsAwake = true;
            //ResourceVersionCheckSystem
            //mDic.Add(CommonClass.mResourceVersionCheckSystemName, ResourceVersionCheckSystem.Instance);
            mDic.Add(CommonClass.mSceneStateControllerName, SceneStateController.Instance);
            mDic.Add(CommonClass.mModelSystemName, ModelSystem.Instance);
            mDic.Add(CommonClass.mUISystemName, UISystem.Instance);
            mDic.Add(CommonClass.mResourcesSystemName, ResourcesSystem.Instance);
            mDic.Add(CommonClass.mGameMapSystemName, GameMapSystem.Instance);
            mDic.Add(CommonClass.mGameSystemName, GameSystem.Instance);
            mDic.Add(CommonClass.mAnimationSystemName, AnimationSystem.Instance);
            mDic.Add(CommonClass.mAudioSystemName, AudioSystem.Instance);
            mDic.Add(CommonClass.mUICtrlSystemName, UICtrlSystem.Instance);

            //ResourceVersionCheckSystem.Instance.Init();
            GameSystem.Instance.GameInit();
            ResourcesSystem.Instance.Init();
            UISystem.Instance.Init();
            AudioSystem.Instance.InitAwake();
            AnimationSystem.Instance.AnimationInit();
            
            GameMapSystem.Instance.GameInit();
            ModelSystem.Instance.ModelInit();
            
        }

        public void Init()
        {
           if(!mIsAwake)
            {
                InitAwake();
            }
        }

        public void End()
        {
            GameSystem.Instance.GameEnd();
            ModelSystem.Instance.ModelEnd();
            AnimationSystem.Instance.AnimationEnd();

        }

        public void Update()
        {
            GameSystem.Instance.GameUpdate();
            AnimationSystem.Instance.AnimationUpdate();
        }

        public void OpenUI(string uibase)
        {
            UISystem.Instance.OpenUIFrom(uibase);
        }

        public void CloseUI(string _UIName)
        {
             UISystem.Instance.CloseUIFrom(_UIName);
        }

        /// <summary>
        /// 消息中转
        /// </summary>
        /// <param name="_Area"></param>
        /// <param name="_EventMa"></param>
        /// <param name="_Message"></param>
        public void SendMsg(string _Area,int _EventMa,object _Message = null)
        {
            if(!mDic.ContainsKey(_Area))
            {
               UnityEngine.Debug.Log("表驱动法中的表键不存在："+ _Area );
            }
            else
            {
                mDic[_Area].Excute(_EventMa, _Message);
            }

        }

        public void SetTimeChange(int isPlay)
        {
            Time.timeScale = isPlay;
        }
        #endregion

        #region Private



        #endregion

        #endregion
    }
}
