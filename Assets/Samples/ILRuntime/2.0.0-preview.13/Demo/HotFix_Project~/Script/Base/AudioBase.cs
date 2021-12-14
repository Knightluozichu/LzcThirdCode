using System.Collections.Generic;

/*
 * @author LuoZichu
 * @time 2019/7/1
 */

namespace RedRedJiang.Unity
{
    public class AudioBase : IBaseNotify
    {
        #region information
        //缓存自身的时间集合
        private List<int> mListEventMa = new List<int>();

        private Dictionary<int, DelExtueHandle> mDicEventDelegate = new Dictionary<int, DelExtueHandle>();
        public Dictionary<int, DelExtueHandle> DicEventDelegate { get => mDicEventDelegate; set => mDicEventDelegate = value; }


        public void Register(params int[] _EventMa)
        {
            mListEventMa.AddRange(_EventMa);
            AudioSystem.Instance.Add(this, mListEventMa.ToArray());
        }

        public void Cancel()
        {
            AudioSystem.Instance.Remove(this, mListEventMa.ToArray());
            mListEventMa.Clear();
        }

        public void Cancel(params int[] _EventMa)
        {
            for (int i = 0; i < _EventMa.Length; i++)
            {
                AudioSystem.Instance.Remove(this, _EventMa[i]);
                mListEventMa.Remove(_EventMa[i]);
            }
        }

        public void SendMsg(string _Area, int _EventMa, object _Message)
        {
            GameFacade.Instance.SendMsg(_Area, _EventMa, _Message);
        }

        public void Excute(int _evenMa, object _Message = null)
        {
            
        }

        #endregion
    }
}
