/*
 * @author LuoZichu
 * @time 2019/7/1
 */

namespace RedRedJiang.Unity
{
    class AnimationSystem : SystemBase<AnimationSystem>, ISystem
    {
        //#region Signle
        //private static AnimationSystem _Instance;
        //public static AnimationSystem Instance
        //{
        //    get
        //    {
        //        if (_Instance == null)
        //        {
        //            _Instance = Activator.CreateInstance<AnimationSystem>();
        //        }

        //        return _Instance;
        //    }
        //}
        //#endregion

        #region Field

        #region Public

        #endregion

        #region Private

        private ExcuteAnimation mAnimationSmlpe;

        #endregion

        #endregion

        #region Method

        #region Public
        public AnimationSystem()
        {
            mSystemName = "AnimationSystem";
        }

        public void AnimationInit()
        {
            mAnimationSmlpe = new ExcuteAnimation();
            mAnimationSmlpe.AnimationInit();
        }

        public void AnimationUpdate()
        {
            if(mAnimationSmlpe != null)
                mAnimationSmlpe.AnimationUpdate();
        }

        public void AnimationEnd()
        {
            if (mAnimationSmlpe != null)
            {
                mAnimationSmlpe.AnimationEnd();
                mAnimationSmlpe = null;
            }
        }

        public void Clean()
        {
            mAnimationSmlpe.Clean();
        }
        #endregion

        #region Private

        #endregion

        #endregion
    }
}
