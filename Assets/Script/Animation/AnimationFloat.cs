using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/*
 * @author LuoZichu
 * @time 2019/7/1
 */

namespace Assets.Script.Animation
{
    public class AnimationFloat : AnimationProPerty
    {
        private float mOriginFloat;
        private float mEndFloat;

        public float OriginFloat { get { return mOriginFloat; } }
        public float EndFloat { get { return mEndFloat; } }

        public AnimationFloat(float mOriginFloat, float mEndFloat)
        {
            this.mOriginFloat = mOriginFloat;
            this.mEndFloat = mEndFloat;
        }
    }
}
