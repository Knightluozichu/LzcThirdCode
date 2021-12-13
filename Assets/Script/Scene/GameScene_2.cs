/*
 * @author LuoZichu
 * @time 2019/7/1
 */

namespace RedRedJiang.Unity
{
    public class GameScene_2 : SceneBase
    {


        public GameScene_2() : base("GameScene_2")
        {
        }

        public override void StateStart()
        {
            GameFacade.Instance.SetTimeChange(1);
           
            Register((int)SceneEventMa.JumpScene_3, (int)SceneEventMa.GameScene_2);

            DicEventDelegate.Add((int)SceneEventMa.JumpScene_3, JumpScene_3Scene);
            DicEventDelegate.Add((int)SceneEventMa.GameScene_2, GameScene_2Scene);

            GameSystem.Instance.GameInit();

        }

        public override void StateUpdate()
        {
            GameFacade.Instance.Update();
            GameSystem.Instance.GameUpdate();
        }

        public override void StateEnd()
        {
           
            GameSystem.Instance.GameEnd();
            AnimationSystem.Instance.Clean();
            EndScene();
        }

        private void JumpScene_3Scene(object _Message)
        {
            SceneStateController.Instance.SetState(new JumpScene_3());
        }

        private void GameScene_2Scene(object message)
        {
            SceneStateController.Instance.SetState(new GameScene_2());
        }
    }
}
