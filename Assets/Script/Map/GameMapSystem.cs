using Assets.Script.Base;
using System;
using System.Collections.Generic;

/*
 * @author LuoZichu
 * @time 2019/7/1
 */

namespace Assets.Script.Map
{
    public class GameMapSystem : SystemBase
    {
        #region Instance
        private static GameMapSystem _Instance;
        public static GameMapSystem Instance
        {
            get
            {
                if (_Instance == null)
                {
                    _Instance = Activator.CreateInstance<GameMapSystem>();
                }

                return _Instance;
            }
        }
        #endregion

        #region Method

        #region Public

        public GameMapSystem()
        {
            mSystemName = Common.CommonClass.mGameMapSystemName;
        }

        public void GameInit()
        {
           
        }

        public void GameUpdate()
        {
           
        }

        public void GameEnd()
        {

        }

        #endregion

        #region Private


        #endregion

        #endregion
    }
}
