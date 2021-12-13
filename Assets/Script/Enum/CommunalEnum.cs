
/*
 * @author LuoZichu
 * @time 2019/7/1
 */

namespace RedRedJiang.Unity
{
    public enum UIFormPos
    {
        Normal,
        Reversal,
        Pop
    }

    public enum StateID
    {
        Stand,//站立
        Run,//移动
        Attack,//攻击
        Death,//死亡
    }

    public enum StateTransition
    {
        LoserVisioin,//失去视野
        SeePeople,//看到敌人
        CanAttack,//可以攻击
        BloodEmpty,//血量为空
    }

    public enum EightDir
    {
        Up,
        Right,
        Down,
        Left,
        RightUp,
        RightDown,
        LeftUp,
        LeftDown
    }

    public enum AnimationXState
    {
        ShadeHiddenToText,

        TransSpriteTo,

        ScaleTo,

        TransTo,

        RateToZ,

        BarTo,

        RendererMaterialFloatTo,

        HRectWOneTo,

        WRectTo,

        HiddenToImg,

        TransUITo,

        InvokeTime,

        ShadeHiddenToSpriteRender,
    }

}
