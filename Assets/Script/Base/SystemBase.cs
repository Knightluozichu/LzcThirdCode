using System.Collections.Generic;

/*
 * @author LuoZichu
 * @time 2019/7/1
 */

namespace RedRedJiang.Unity
{
    public class SystemBase<T> :Singleton<T>, IBaseNotify where T : SystemBase<T>
    {
        protected string mSystemName;
        public string SystemName { get { return mSystemName; } }

        private Dictionary<int, DelExtueHandle> mDicEventDelegate = new Dictionary<int, DelExtueHandle>();
        public Dictionary<int, DelExtueHandle> DicEventDelegate { get => mDicEventDelegate; set => mDicEventDelegate = value; }

        /// <summary>
        /// 缓存事件码关心的脚本集合
        /// </summary>
        private Dictionary<int, List<IBaseNotify>> mDicEvent = new Dictionary<int, List<IBaseNotify>>();

        /// <summary>
        /// 事件处理
        /// </summary>
        /// <param name="_evenMa">事件码</param>
        /// <param name="_Message">消息</param>
        public void Excute(int _evenMa, object _Message = null)
        {
            //CheckDicEvent();
            List<IBaseNotify> mListBaseScript = null;
            if (mDicEvent.ContainsKey(_evenMa))
            {
                mListBaseScript = mDicEvent[_evenMa];

                if (mListBaseScript.Count < 1)
                {
                    UnityEngine.Debug.Log("这个事件码" + _evenMa + "并没有绑定任何脚本");
                    mDicEvent.Remove(_evenMa);
                    return;
                }
                else
                {
                    for (int i = mListBaseScript.Count - 1; i >= 0; i--)
                    {
                        if (mListBaseScript[i] != null)
                        {
                            mListBaseScript[i].Excute(_evenMa, _Message);
                        }
                        else
                        {
                            mListBaseScript.Remove(mListBaseScript[i]);
                        }
                    }
                }
            }
            else
            {
                UnityEngine.Debug.Log("这个事件" + _evenMa + "还没注册");
                return;
            }

        }

        /// <summary>
        /// 添加事件，一个事件码关心多个脚本
        /// </summary>
        /// <param name="_EventMa">事件码</param>
        /// <param name="_BaseScript">脚本引用</param>
        private void Add(int _EventMa,IBaseNotify _BaseScript)
        {
            if (_BaseScript == null) {UnityEngine.Debug.Log("脚本"+ _BaseScript + "没有实例化");return; }
            List<IBaseNotify> mListBaseScript = null;

            if(mDicEvent.ContainsKey(_EventMa))
            {
                mListBaseScript = mDicEvent[_EventMa];

                if(mListBaseScript.Contains(_BaseScript))
                {
                   UnityEngine.Debug.Log("这个事件码"+ _EventMa + "已经添加过这个脚本引用了"+ _BaseScript );
                    return;
                }
                else
                {
                    mListBaseScript.Add(_BaseScript);
                }
            }
            else
            {
                mListBaseScript = new List<IBaseNotify>();
                mListBaseScript.Add(_BaseScript);
                mDicEvent.Add(_EventMa, mListBaseScript);
            }
            //CheckDicEvent();
        }

        /// <summary>
        /// 一个脚本关心多个事件码
        /// </summary>
        /// <param name="_BaseScript"></param>
        /// <param name="_EventMa"></param>
        public void Add(IBaseNotify _BaseScript,params int[] _EventMa)
        {
            for(int i = 0; i < _EventMa.Length;i++)
            {
                Add(_EventMa[i], _BaseScript);
            }
        }

        /// <summary>
        /// 移除关心的脚本
        /// </summary>
        /// <param name="_EventMa"></param>
        /// <param name="_BaseScript"></param>
        private void Remove(int _EventMa,IBaseNotify _BaseScript)
        {
            if (_BaseScript == null) {UnityEngine.Debug.Log("脚本"+ _BaseScript + "没有实例化" ); return; }

            List<IBaseNotify> mListBaseScript = null;

            if(mDicEvent.ContainsKey(_EventMa))
            {
                mListBaseScript = mDicEvent[_EventMa];

                if(mListBaseScript.Contains(_BaseScript))
                {
                    mListBaseScript.Remove(_BaseScript);

                    if(mListBaseScript.Count == 0)
                    {
                        mDicEvent.Remove(_EventMa);
                    }
                }
                else
                {
                   UnityEngine.Debug.Log("这个事件"+ _EventMa + "没有绑定这个脚本"+ _BaseScript );
                }
            }
            else
            {
               UnityEngine.Debug.Log("这个事件"+ _EventMa + "没有注册");
            }
        }

        /// <summary>
        /// 移除对应的事件码
        /// </summary>
        /// <param name="_BaseScript"></param>
        /// <param name="_EventMa"></param>
        public void Remove(IBaseNotify _BaseScript,params int[] _EventMa)
        {
            for(int i = 0; i< _EventMa.Length;i++)
            {
                Remove(_EventMa[i], _BaseScript);
            }
        }

        #region Test

        private void CheckDicEvent()
        {
            foreach(var i  in mDicEvent)
            {
               UnityEngine.Debug.Log(i.Key + ":" + i.Value);
            }
        }

        #endregion
    }
}
