using System;

/*
 * @author LuoZichu
 * @time 2019/7/1
 */

namespace RedRedJiang.Unity
{
    public class MonsterStandState : MonsterFSMState
    {
        
        public MonsterStandState(MonsterBase mMonsterBase,MonsterFSMSystem mFSMSystem) : base(mMonsterBase, mFSMSystem)
        {
            mStateID = StateID.Stand;
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
