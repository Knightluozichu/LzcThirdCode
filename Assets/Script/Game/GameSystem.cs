/*
 * @author LuoZichu
 * @time 2019/7/1
 */

namespace RedRedJiang.Unity
{
    public class GameSystem : SystemBase<GameSystem>, ISystem
    {
        //#region 单例
        //private static GameSystem _Instance;
        //public static GameSystem Instance
        //{
        //    get
        //    {
        //        if (_Instance == null)
        //        {
        //            _Instance = Activator.CreateInstance<GameSystem>();
        //        }

        //        return _Instance;
        //    }
        //}
        //#endregion

        #region Field

        #endregion

        #region Method

        #region Public

        public GameSystem()
        {
            mSystemName = CommonClass.mGameSystemName;

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
