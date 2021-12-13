using System;

/*
 * @author LuoZichu
 * @time 2019/7/1
 */

namespace Assets.Script.Animation
{
    public class InvokeTimeInfo
    {
        /// <summary>
        /// 计时
        /// </summary>
        public float DelayTime { set; get; }

        /// <summary>
        /// 完成时回调
        /// </summary>
        private Action<InvokeTimeInfo> CallBack;

        /// <summary>
        /// 是否完成
        /// </summary>
        public bool isFinish { set; get; } 

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="DelayTime">时长</param>
        /// <param name="CallBack">完成时回调</param>
        public InvokeTimeInfo(float DelayTime, Action<InvokeTimeInfo> CallBack = null)
        {
            this.DelayTime = DelayTime;
            this.CallBack = CallBack;
        }


        public void DoCallBack()
        {
            if(CallBack != null)
            {
                CallBack(this);
            }
            isFinish = true;
        }

        /// <summary>
        /// 倒计时
        /// </summary>
        public void UpdateTimer()
        {
            if(DelayTime <= 0 && !isFinish)
            {
                DoCallBack();
                return;
            }

            DelayTime -= UnityEngine.Time.deltaTime;
        }
    }
}
