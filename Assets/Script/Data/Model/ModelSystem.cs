/*
 * @author LuoZichu
 * @time 2019/7/1
 */

namespace RedRedJiang.Unity
{
    public class ModelSystem : SystemBase<ModelSystem>, ISystem
    {
        //#region Signle
        //private static ModelSystem _Instance;
        //public static ModelSystem Instance
        //{
        //    get
        //    {
        //        if (_Instance == null)
        //        {
        //            _Instance = Activator.CreateInstance<ModelSystem>();
        //        }

        //        return _Instance;
        //    }
        //}
        //#endregion

        #region Field

        #region PlayerData

        private GlobalModel mGlobalModelExture;
        public GlobalModel GlobalModelExture { get { return mGlobalModelExture; } }


        #endregion

        #endregion

        #region Mothod

        #region Public

        public ModelSystem()
        {
            mSystemName = CommonClass.mModelSystemName;

        }


        public void ModelInit()
        {

            mGlobalModelExture = new GlobalModel();
            mGlobalModelExture.ModelInit();
        }

        public void ModelEnd()
        {
            if (mGlobalModelExture != null)
            {
                mGlobalModelExture.ModelEnd();
                mGlobalModelExture = null;
            }
        }

        #region 数据相关

        /// <summary>
        /// 存档数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="_Path"></param>
        /// <param name="t"></param>
        public void SaveData<T>(string _Path, T t)
        {
            BinarySystem.Instance.SerializeOfArchive<T>(_Path, t);
        }

        /// <summary>
        /// 获取存档数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="_Path"></param>
        /// <returns></returns>
        public T GetData<T>(string _Path) where T : class
        {
            return BinarySystem.Instance.DeserializeOfArchive<T>(_Path);
        }

        /// <summary>
        /// 加载配置数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="_NameKey"></param>
        /// <returns></returns>
        public T GetDataConst<T>(string _NameKey)
        {
            return BinarySystem.Instance.DeseriallizsOfArchiveConstData<T>(_NameKey);
        }

        #endregion

        #endregion

        #region Private


        #endregion

        #endregion
    }
}
