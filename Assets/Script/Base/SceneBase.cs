using Assets.Script.Scene;
using System.Collections.Generic;
using Assets.Script.Notify;

/*
 * @author LuoZichu
 * @time 2019/7/1
 */

namespace Assets.Script.Base
{
    public class SceneBase : IBaseNotify
    {
        public override void Excute(int _evenMa, object _Message = null)
        {
            if (mDicEventDelegate.Count > 0)
            {
                if (mDicEventDelegate.ContainsKey(_evenMa))
                {
                    if (mDicEventDelegate[_evenMa] != null)
                    {
                        mDicEventDelegate[_evenMa](_Message);
                    }
                }
            }
        }
        #region information
        //缓存自身的时间集合
        private List<int> mListEventMa = new List<int>();

        public void Register(params int[] _EventMa)
        {
            mListEventMa.AddRange(_EventMa);

            SceneStateController.Instance.Add(this, mListEventMa.ToArray());
        }

        public void Cancel()
        {
            SceneStateController.Instance.Remove(this, mListEventMa.ToArray());
            mListEventMa.Clear();
        }

        public void Cancel(params int[] _EventMa)
        {
            for (int i = 0; i < _EventMa.Length; i++)
            {
                SceneStateController.Instance.Remove(this, _EventMa[i]);
                mListEventMa.Remove(_EventMa[i]);
            }
        }

        public void SendMsg(string _Area, int _EventMa, object _Message)
        {
            GameFacade.Instance.SendMsg(_Area, _EventMa, _Message);
        }

        protected void EndScene()
        {
            Cancel();
            mDicEventDelegate.Clear();
            mDicEventDelegate = null;
        }
        #endregion

        private string mSceneName;

        public string SceneName { get { return mSceneName; } }

        public SceneBase(string mSceneName)
        {
            this.mSceneName = mSceneName;
        }

        public virtual void StateStart() { }
        public virtual void StateUpdate() { }
        public virtual void StateEnd() { }
    }
}
