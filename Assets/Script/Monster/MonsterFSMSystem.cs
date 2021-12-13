using System.Collections.Generic;
using Assets.Script.Enum;
using UnityEngine;

/*
 * @author LuoZichu
 * @time 2019/7/1
 */

namespace Assets.Script.Monster
{
    public class MonsterFSMSystem
    {
        #region Field
        private List<MonsterFSMState> mListStates = new List<MonsterFSMState>();
        private StateID mCurrentId;
        private MonsterFSMState mCurrentState;
        #endregion

        public StateID CurrentID { get { return mCurrentId; } }
        public MonsterFSMState CurrentState { get { return mCurrentState; } }

        public MonsterFSMSystem()
        {
        }

        /// <summary>
        /// 更新状态
        /// </summary>
        /// <param name="_Player"></param>
        public void UpdateState()
        {
            mCurrentState.Act();
            mCurrentState.Reason();
        }

        /// <summary>
        /// 增加状态
        /// </summary>
        /// <param name="_State"></param>
        private void AddState(MonsterFSMState _State)
        {
/*            Debug.Log("AddState");*/
            if (_State == null)
            {
                Debug.Log("FSMError: _State is null");
                return;
            }

            if (mListStates.Count == 0)
            {
                mCurrentId = _State.ID;
                mCurrentState = _State;
                mListStates.Add(_State);
                mCurrentState.DoBeforeEntering();
                return;
            }

            foreach (var state in mListStates)
            {
                if (state.ID == _State.ID)
                {
                    Debug.Log("mListStates has been " + _State.ID + "state name" + _State);
                    return;
                }
            }

            mListStates.Add(_State);
        }

        public void AddState(params MonsterFSMState[] _arrayState)
        {
            foreach (MonsterFSMState i in _arrayState)
            {
                AddState(i);
            }
        }

        /// <summary>
        /// 移除状态
        /// </summary>
        /// <param name="_Id"></param>
        public void DeleteState(StateID _Id)
        {
            //Debug.Log(mListStates.Count);
            //foreach(var i in mListStates)
            //{
            //    Debug.Log(i);
            //}
            for (int i = 0; i< mListStates.Count;i++)
            {
                if (_Id == mListStates[i].ID)
                {
                    mListStates[i] = null;
                    mListStates.Remove(mListStates[i]);
                    return;
                }
            }

            Debug.Log("mListStates has not :" + _Id);
        }



        /// <summary>
        /// 执行转换
        /// </summary>
        /// <param name="_Tran"></param>
        public void PerformTransition(StateTransition _Tran)
        {

            StateID nextId = mCurrentState.GetOutputState(_Tran);

            mCurrentId = nextId;

/*            Debug.Log("mCurrentId" + mCurrentId);*/

            foreach (var state in mListStates)
            {
                if (mCurrentId == state.ID)
                {
                    mCurrentState.DoBeforeLeaving();
                    mCurrentState = state;
                    mCurrentState.DoBeforeEntering();
                    return;
                }
            }
        }
    }
}
