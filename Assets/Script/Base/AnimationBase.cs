using System.Collections.Generic;
using Assets.Script.Animation;
using Assets.Script.Notify;
using UnityEngine;

/*
 * @author LuoZichu
 * @time 2019/7/1
 */

namespace Assets.Script.Base
{
    public class AnimationBase : IBaseNotify
    {
        #region Excute

        public override void Excute(int _evenMa, object _Message = null)
        {
            if (mDicEventDelegate.Count > 0)
            {
                if (mDicEventDelegate.ContainsKey(_evenMa))
                {
                    mDicEventDelegate[_evenMa](_Message);
                }
                else
                {
                    Debug.Log("mEventDelegate 不包含" + _evenMa + "这个事件码。");
                }
            }
            else
            {
                Debug.Log("mEventDelegate 长度为 0");
            }
        }

        #endregion

        #region information
        //缓存自身的时间集合
        private List<int> mListEventMa = new List<int>();

        public void Register(params int[] _EventMa)
        {
            mListEventMa.AddRange(_EventMa);
            AnimationSystem.Instance.Add(this, mListEventMa.ToArray());
        }

        public void Cancel()
        {
            AnimationSystem.Instance.Remove(this, mListEventMa.ToArray());
            mListEventMa.Clear();
        }

        public void Cancel(params int[] _EventMa)
        {
            for (int i = 0; i < _EventMa.Length; i++)
            {
                AnimationSystem.Instance.Remove(this, _EventMa[i]);
                mListEventMa.Remove(_EventMa[i]);
            }
        }

        public void SendMsg(string _Area, int _EventMa, object _Message)
        {
            GameFacade.Instance.SendMsg(_Area, _EventMa, _Message);
        }

        #endregion
    }
}
