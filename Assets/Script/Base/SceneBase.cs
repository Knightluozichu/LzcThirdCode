using System.Collections.Generic;

/*
 * @author LuoZichu
 * @time 2019/7/1
 */

namespace RedRedJiang.Unity
{
    public class SceneBase : IBaseNotify
    {
        public  void Excute(int _evenMa, object _Message = null)
        {
            if (DicEventDelegate.Count > 0)
            {
                if (DicEventDelegate.ContainsKey(_evenMa))
                {
                    if (DicEventDelegate[_evenMa] != null)
                    {
                        DicEventDelegate[_evenMa](_Message);
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
            DicEventDelegate.Clear();
            DicEventDelegate = null;
        }
        #endregion

        private string mSceneName;

        public string SceneName { get { return mSceneName; } }

        private Dictionary<int, DelExtueHandle> mDicEventDelegate = new Dictionary<int, DelExtueHandle>();
        public Dictionary<int, DelExtueHandle> DicEventDelegate { get => mDicEventDelegate; set => mDicEventDelegate = value; }


        public SceneBase(string mSceneName)
        {
            this.mSceneName = mSceneName;
        }

        public virtual void StateStart() { }
        public virtual void StateUpdate() { }
        public virtual void StateEnd() { }
    }
}
