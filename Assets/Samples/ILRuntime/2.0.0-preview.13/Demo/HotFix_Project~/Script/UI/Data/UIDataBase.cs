﻿using System.Collections.Generic;
using UnityEngine;

namespace RedRedJiang.Unity
{
    public class UIDataBase : IBaseNotify
    {
        #region Excute

        public  void Excute(int _evenMa, object _Message = null)
        {
            if (DicEventDelegate.Count > 0)
            {
                if (DicEventDelegate.ContainsKey(_evenMa))
                {
                    DicEventDelegate[_evenMa](_Message);
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

        private Dictionary<int, DelExtueHandle> mDicEventDelegate = new Dictionary<int, DelExtueHandle>();
        public Dictionary<int, DelExtueHandle> DicEventDelegate { get => mDicEventDelegate; set => mDicEventDelegate = value; }


        public void Register(params int[] _EventMa)
        {

            mListEventMa.AddRange(_EventMa);
            UISystem.Instance.Add(this, mListEventMa.ToArray());
        }

        public void Cancel()
        {
            UISystem.Instance.Remove(this, mListEventMa.ToArray());
            mListEventMa.Clear();
        }

        public void Cancel(params int[] _EventMa)
        {
            for (int i = 0; i < _EventMa.Length; i++)
            {
                UISystem.Instance.Remove(this, _EventMa[i]);
                mListEventMa.Remove(_EventMa[i]);
            }
        }

        public void SendMsg(string _Area, int _EventMa, object _Message = null)
        {
            GameFacade.Instance.SendMsg(_Area, _EventMa, _Message);
        }

        #endregion
    }
}
