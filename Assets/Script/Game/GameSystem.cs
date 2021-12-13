using System.Collections.Generic;
using Assets.Script.Audio;
using Assets.Script.Base;
using System;
using UnityEngine;
using Assets.Script.Monster;
using Assets.Script.Common;

/*
 * @author LuoZichu
 * @time 2019/7/1
 */

namespace Assets.Script.Game
{
    public class GameSystem : SystemBase
    {
        #region 单例
        private static GameSystem _Instance;
        public static GameSystem Instance
        {
            get
            {
                if (_Instance == null)
                {
                    _Instance = Activator.CreateInstance<GameSystem>();
                }

                return _Instance;
            }
        }
        #endregion

        #region Field

        #endregion

        #region Method

        #region Public

        public GameSystem()
        {
            mSystemName = Common.CommonClass.mGameSystemName;

        }

        public void GameEnd()
        {

        }

        public void GameInit()
        {

        }

        public void GameUpdate()
        {

        }

        /// <summary>
        /// 相机跟随
        /// </summary>
        private void LateUpdate()
        {

        }

  
        public void SetFirst(bool _IsTrue)
        {

        }

        #endregion

       

        #endregion
    }
}
