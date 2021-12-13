/*
 * @author LuoZichu
 * @time 2019/7/1
 */

namespace RedRedJiang.Unity
{
    public class TheFrontLine_1 : SceneBase
    {
        
        public TheFrontLine_1() : base("TheFrontLine_1")
        {
        }

        public override void StateStart()
        {
            Register((int)SceneEventMa.GameScene_2);

            DicEventDelegate.Add((int)SceneEventMa.GameScene_2, GameScene_2Scene);

            GameFacade.Instance.OpenUI(CommonClass.mStartPanelName);
        }

        public override void StateEnd()
        {
            EndScene();
        }

        public override void StateUpdate()
        {
            GameFacade.Instance.Update();
        }
        private void GameScene_2Scene(object message)
        {
            SceneStateController.Instance.SetState(new GameScene_2());
        }
    }
}
