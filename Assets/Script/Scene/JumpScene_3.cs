using Assets.Script.Animation;
using Assets.Script.Base;
using Assets.Script.Data.Model;
using Assets.Script.Enum;
using Assets.Script.Game;
using Assets.Script.Map;
using Assets.Script.Resources;
using Assets.Script.UI;

/*
 * @author LuoZichu
 * @time 2019/7/1
 */

namespace Assets.Script.Scene
{
    public class JumpScene_3 : SceneBase
    {
 
        public JumpScene_3():base("JumpScene_3")
        {
            
        }

        public override void StateStart()
        {
            Register((int)SceneEventMa.GameScene_2);

            mDicEventDelegate.Add((int)SceneEventMa.GameScene_2, GameScene_2Scene);



        }

        public override void StateUpdate()
        {
            GameFacade.Instance.Update();
        }

        public override void StateEnd()
        {
            EndScene();
        }

        private void GameScene_2Scene(object message)
        {
            SceneStateController.Instance.SetState(new GameScene_2());
        }
    }
}
