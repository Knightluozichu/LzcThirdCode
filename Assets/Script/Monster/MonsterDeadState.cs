
using System;


/*
 * @author LuoZichu
 * @time 2019/7/1
 */

namespace RedRedJiang.Unity
{
    public class MonsterDeadState : MonsterFSMState
    {
        public MonsterDeadState(MonsterBase mMonsterObj, MonsterFSMSystem mFSMSystem) : base(mMonsterObj, mFSMSystem)
        {
            mStateID = StateID.Death;
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
