using Assets.Script.Common;
using UnityEngine;
using System;


/*
 * @author LuoZichu
 * @time 2019/7/1
 */

namespace Assets.Script.Monster
{
    public class MonsterMoveState : MonsterFSMState
    {
 
        public MonsterMoveState(MonsterBase mMonsterObj, MonsterFSMSystem mFSMSystem) : base(mMonsterObj, mFSMSystem)
        {
            mStateID = Enum.StateID.Run;
        }

        public override void Act()
        {
           
        }

        public override void Reason()
        {
            
        }

        public override void DoBeforeEntering()
        {
            base.DoBeforeEntering();
          
        }
    }
}
