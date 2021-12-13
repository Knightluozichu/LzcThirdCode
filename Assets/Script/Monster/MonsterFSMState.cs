using System.Collections.Generic;
using UnityEngine;

/*
 * @author LuoZichu
 * @time 2019/7/1
 */

namespace RedRedJiang.Unity
{
    public abstract class MonsterFSMState
    {
        #region Property

        protected MonsterBase mMonsterObj;
        protected MonsterFSMSystem mFSMSystem;

        protected Dictionary<StateTransition, StateID> map = new Dictionary<StateTransition, StateID>();
        protected StateID mStateID;
        public StateID ID { get { return mStateID; } }
        public MonsterBase MonsterObj { get { return mMonsterObj; } }

        #endregion

        #region Method
        public MonsterFSMState(MonsterBase mMonsterObj, MonsterFSMSystem mFSMSystem)
        {
            this.mMonsterObj = mMonsterObj;
            this.mFSMSystem = mFSMSystem;

        }

        public void End()
        {
            map.Clear();
            map = null;
            mFSMSystem = null;
            mMonsterObj = null;
        }
        public void AddTransition(StateTransition _Tran, StateID _Id)
        {
            if (map.ContainsKey(_Tran))
            {
                Debug.Log("map has been :" + _Tran);
                return;
            }

            map.Add(_Tran, _Id);
        }

        public void DeleteTransition(StateTransition _Tran)
        {
            if (map.ContainsKey(_Tran))
            {
                map.Remove(_Tran);
                return;
            }

            Debug.Log("map has not :" + _Tran);
        }

        public StateID GetOutputState(StateTransition _Tran)
        {
            if (map.ContainsKey(_Tran))
            {
                return map[_Tran];
            }

            return default(StateID);
        }

        #endregion

        #region Virtual

        public virtual void DoBeforeEntering() { }
        public virtual void DoBeforeLeaving() { }

        #endregion

        #region abstract

        public abstract void Act();
        public abstract void Reason();

        #endregion
    }
}
