using System;


/*
 * @author LuoZichu
 * @time 2019/7/1
 */

namespace RedRedJiang.Unity
{
    public class MonsterMoveState : MonsterFSMState
    {
 
        public MonsterMoveState(MonsterBase mMonsterObj, MonsterFSMSystem mFSMSystem) : base(mMonsterObj, mFSMSystem)
        {
            mStateID = StateID.Run;
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
