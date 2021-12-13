
using Assets.Script.Monster;
using UnityEngine;
using Assets.Script.Enum;
using Assets.Script.Data.Model;
using Assets.Script.Game;
using Assets.Script.Map;
using Assets.Script.Animation;
using Assets.Script.Base;

/*
 * @author LuoZichu
 * @time 2019/7/1
 */

namespace Assets.Script.Scene
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

            mDicEventDelegate.Add((int)SceneEventMa.JumpScene_3, JumpScene_3Scene);
            mDicEventDelegate.Add((int)SceneEventMa.GameScene_2, GameScene_2Scene);

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
