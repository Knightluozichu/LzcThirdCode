using UnityEngine;

/*
 * @author LuoZichu
 * @time 2019/7/1
 */

namespace Assets.Script.Animation
{
    public class AnimationVec3 : AnimationProPerty
    {
        private Vector3 mOrignVec3;
        private Vector3 mEndVec3;

        public Vector3 OrignVec3 { get { return mOrignVec3; } }
        public Vector3 EndVec3 { get { return mEndVec3; } }

        public AnimationVec3(Vector3 mOrignVec3, Vector3 mEndVec3)
        {
            this.mOrignVec3 = mOrignVec3;
            this.mEndVec3 = mEndVec3;
        }
    }
}
