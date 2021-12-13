using UnityEngine;
using System;


/*
 * @author LuoZichu
 * @time 2019/7/1
 */

namespace RedRedJiang.Unity
{
    public class MonsterAttackState : MonsterFSMState
    {
        private float mAttackTimer = 0;
       
        public MonsterAttackState(MonsterBase mMonsterObj, MonsterFSMSystem mFSMSystem) : base(mMonsterObj, mFSMSystem)
        {
            mStateID = StateID.Attack;

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
            this.MonsterObj.CProperty.GameObj.GetComponent<UnityEngine.Animator>().SetInteger("state", 0);
        }

        public override void DoBeforeLeaving()
        {
            base.DoBeforeLeaving();

        }


    }
}
