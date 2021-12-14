/*
 * @author LuoZichu
 * @time 2019/7/1
 */

namespace RedRedJiang.Unity
{
    public class JumpScene_3 : SceneBase
    {
 
        public JumpScene_3():base("JumpScene_3")
        {
            
        }

        public override void StateStart()
        {
            Register((int)SceneEventMa.GameScene_2);

            DicEventDelegate.Add((int)SceneEventMa.GameScene_2, GameScene_2Scene);



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
