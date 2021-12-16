using UnityEngine;
using UnityEngine.SceneManagement;

/*
 * @author LuoZichu
 * @time 2019/7/1
 */

namespace RedRedJiang.Unity
{
    public class SceneStateController : SystemBase<SceneStateController>, ISystem
    {
        
        //private static SceneStateController _Instance;
        //public static SceneStateController Instance
        //{
        //    get
        //    {
        //        if (_Instance == null)
        //        {
        //            _Instance = Activator.CreateInstance<SceneStateController>();
        //        }

        //        return _Instance;
        //    }
        //}

        public SceneStateController()
        {
            mSystemName = CommonClass.mSceneStateControllerName;
        }

        private AsyncOperation mAsyOp;
        private SceneBase mIScene;
        private bool isRunStart;
        public void SetState(SceneBase _IScene,bool isLoadAsync = true)
        {
            if(mIScene != null)
            {
                mIScene.StateEnd();
            }

            mIScene = _IScene;

            isRunStart = false;

            if (isLoadAsync)
            {
                mAsyOp = SceneManager.LoadSceneAsync(mIScene.SceneName);
            }
            else
            {
                mIScene.StateStart();
            }
        }

        public void StateUpdate()
        {
            if (mAsyOp != null && !mAsyOp.isDone) return;

            if(mAsyOp != null && mAsyOp.isDone && !isRunStart)
            {
                isRunStart = true;
                mAsyOp = null;
            }

            if(mIScene != null)
            {
                if (isRunStart)
                {
                    mIScene.StateStart();
                    isRunStart = false;
                }

                mIScene.StateUpdate();
            }
            
        }

        public void End()
        {
            mAsyOp = null;
            mIScene.StateEnd();
            mIScene = null;
            isRunStart = false;
        }
    }
}
