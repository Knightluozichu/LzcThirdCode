using System;

/*
 * @author LuoZichu
 * @time 2019/7/1
 */

namespace RedRedJiang.Unity
{
    public class AnimationInfo
    {
        #region Field

        #region Private
        private float mStartTime;

        /// <summary>
        /// 是否完成了
        /// </summary>
        private bool isFinish;

        private bool isActionFinish;

        /// <summary>
        /// 回调
        /// </summary>
        private Action<AnimationInfo> mCallBack;

        /// <summary>
        /// 持续时间
        /// </summary>
        private float mEndTime;

        /// <summary>
        /// 待做动画的对象
        /// </summary>
        private object mObjectX;

        /// <summary>
        /// 待做动画的参数
        /// </summary>
        private AnimationProPerty mAnimationPro;

        /// <summary>
        /// 待做什么动画
        /// </summary>
        private AnimationXState mCurrentState;
        #endregion

        #region Public 

        public bool IsFinish { get { return isFinish; } }

        public float EndTime { set { mEndTime = value; } get { return mEndTime; } }

        public AnimationXState CurrentState { get { return mCurrentState; } }

        public AnimationProPerty AnimationPro { get { return mAnimationPro; } }

        public object ObjectX { get { return mObjectX; } }

        public bool IsActionFinish { get { return isActionFinish; } }

        public float StartTime { get { return mStartTime; } }

        #endregion

        #endregion

        #region Method

        #region Public
        public AnimationInfo(float mEndTime, object mObjectX, AnimationXState mCurrentState, AnimationProPerty mAnimationPro, Action<AnimationInfo> mCallBack = null)
        {
            this.mEndTime = mEndTime;
            this.mObjectX = mObjectX;
            this.mCurrentState = mCurrentState;
            this.mAnimationPro = mAnimationPro;
            this.mCallBack = mCallBack;
            isActionFinish = false;
            isFinish = false;
            mStartTime = mEndTime;
        }

        public void CallBack()
        {
            if (mCallBack != null && isFinish)
            {
                mCallBack(this);
            }

            isActionFinish = true;
        }

        public void SetFinish()
        {

            isFinish = true;

        }

        #endregion

        #region Private

        #endregion
        #endregion
    }
}
