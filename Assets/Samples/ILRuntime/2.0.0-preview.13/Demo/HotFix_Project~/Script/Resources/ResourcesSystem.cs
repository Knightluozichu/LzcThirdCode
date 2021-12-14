using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

/*
 * @author LuoZichu
 * @time 2019/7/1
 */

namespace RedRedJiang.Unity
{
    public class ResourcesSystem : SystemBase<ResourcesSystem>,ISystem
    {
        //#region Instance
        //private static ResourcesSystem _Instance;
        //public static ResourcesSystem Instance
        //{
        //    get
        //    {
        //        if (_Instance == null)
        //        {
        //            _Instance = Activator.CreateInstance<ResourcesSystem>();
        //        }

        //        return _Instance;
        //    }
        //}
        //#endregion

        #region Field
        private const string mPathInfo_Resource = "ResourcesList";
        //private const string mPathInfo_MD5 = "Md5";

        private const string mExcelTitleInfoPath = "titleInfo";
        private const string mExcelAchievementPath = "AchievementInfo";

        private bool mIsOnce;
        public bool IsOnce { get { return mIsOnce; } }

        private Dictionary<string, AssetBundle> mDicAssetBundle = new Dictionary<string, AssetBundle>();

        private KeyValuesInfo mKeyValues = null;
        /// <summary>
        /// 资源路径
        /// </summary>
        private Dictionary<string, string> mDicResPathInfo = new Dictionary<string, string>();

        /// <summary>
        /// MD5文件对比
        /// </summary>
        //private Dictionary<string,string> keyValuePairs_MD5 = new Dictionary<string, string>();

        /// <summary>
        /// 资源缓存
        /// </summary>
        private Dictionary<string, UnityEngine.Object> mDicChacheObj = new Dictionary<string, UnityEngine.Object>();
        private Dictionary<string, UnityEngine.Object[]> mDicChacheObjArray = new Dictionary<string, UnityEngine.Object[]>();

        public Dictionary<string, string> DicResPathInfo { get { return mDicResPathInfo; } }

        private string streamstring_path = Application.streamingAssetsPath + "/";
        #endregion

        #region Method
        #region Public

        public ResourcesSystem()
        {
            mSystemName = CommonClass.mResourcesSystemName;
        }

        public void InitAwake()
        {
            InitDicPath();
        }

        /// <summary>
        /// 初始化ResourcesList文本
        /// </summary>
        public void Init()
        {
            if (!mIsOnce)
            {
                InitAwake();
                mIsOnce = true;
            }
        }

        public void GameEnd()
        {
            mDicResPathInfo.Clear();
            mDicChacheObj.Clear();
            DicResPathInfo.Clear();
            //_Instance = null;
            mDicAssetBundle.Clear();
        }
        #endregion

        #region Private

        /// <summary>
        /// 加载文本
        /// </summary>
        private void InitDicPath()
        {
            if (!Directory.Exists(Application.streamingAssetsPath))
            {
                Debug.LogError($"Application.streamingAssetsPath 文件夹不存在 {Application.streamingAssetsPath}");
                return;
            }

            TextAsset mTextAssets = UnityEngine.Resources.Load<TextAsset>(mPathInfo_Resource);

            if (!mTextAssets)
            {
                UnityEngine.Debug.Log("请检查：" + mPathInfo_Resource + "是否有误！");
                return;
            }

            try
            {
                mKeyValues = JsonUtility.FromJson<KeyValuesInfo>(mTextAssets.text);
                /*                UnityEngine.Debug.Log(mKeyValues);*/
            }
            catch (Exception e)
            {
                UnityEngine.Debug.Log("错误为：" + e.Message);
            }

            for (int i = 0; i < mKeyValues.mKeyValuesInfo.Count; i++)
            {
                mDicResPathInfo[mKeyValues.mKeyValuesInfo[i].mKey] = mKeyValues.mKeyValuesInfo[i].mValues;
            }
            ///Test
            //foreach (var i in mDicResPathInfo)
            //{
            //   UnityEngine.Debug.Log(i.Value);
            //}

            
        }
        #endregion
        #endregion

        #region 请求资源
        /// <summary>
        /// 请求资源
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="_ObjName">对象名称</param>
        /// <param name="isCache">缓存</param>
        /// <returns></returns>
        public T ResObj<T>(string _ObjName, bool isCache) where T : class
        {
            ///如果名字为空，就返回
            ///如果缓存存在就返回
            ///从路径信息得到路径，www进行加载，返回对象
            ///如果要缓存，就把对象缓存到缓存里
            UnityEngine.Object mObj = null;
            if (string.IsNullOrEmpty(_ObjName))
            {
                UnityEngine.Debug.Log("_ObjName 是空的！");
                return mObj as T;
            }

            if (mDicChacheObj.ContainsKey(_ObjName))
            {
                mObj = mDicChacheObj[_ObjName];
                return mObj as T;
            }

            string mPath = null;

            if (mDicResPathInfo.TryGetValue(_ObjName, out mPath)) // 路径
            {
                //有路径
                mObj = LoadAssetRes(_ObjName, mPath.ToLower()); // 对象   // 设计问题
            }
            else
            {
                //不存在这个对象的路径
                UnityEngine.Debug.Log("不存在这个对象" + _ObjName + "的路径");
            }

            if (isCache)
            {
                if (mDicChacheObj.ContainsKey(_ObjName))
                {
                    UnityEngine.Debug.Log("缓存字典已经存在" + _ObjName);
                }
                else
                {
                    mDicChacheObj[_ObjName] = mObj;
                }
            }

            return mObj as T;
        }

        /// <summary>
        /// 请求图集 资源 以数组的形式读取Sprite
        /// objs[0]里面存储的是这个图集的引用（类型为Texture2D），
        /// 再之后，也就是从第二个开始就是里面小图的引用（类型为Sprite）
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="_ObjName"></param>
        /// <param name="isCache"></param>
        /// <returns></returns>
        public T[] ResAltas<T>(string _ObjName, bool isCache)
        {
            ///如果名字为空，就返回
            ///如果缓存存在就返回
            ///从路径信息得到路径，www进行加载，返回对象
            ///如果要缓存，就把对象缓存到缓存里
            UnityEngine.Object[] mObj = null;
            if (string.IsNullOrEmpty(_ObjName))
            {
                UnityEngine.Debug.Log("_ObjName 是空的！");
                return mObj as T[];
            }

            if (mDicChacheObj.ContainsKey(_ObjName))
            {
                mObj = mDicChacheObjArray[_ObjName];
                return mObj as T[];
            }

            string mPath = null;

            if (mDicResPathInfo.TryGetValue(_ObjName, out mPath)) // 路径
            {
                //有路径
                mObj = LoadALLAssetRes(mPath.ToLower()); // 对象   // 设计问题
            }
            else
            {
                //不存在这个对象的路径
                UnityEngine.Debug.Log("不存在这个对象" + _ObjName + "的路径");
            }

            if (isCache)
            {
                if (mDicChacheObjArray.ContainsKey(_ObjName))
                {
                    UnityEngine.Debug.Log("缓存字典已经存在" + _ObjName);
                }
                else
                {
                    mDicChacheObjArray[_ObjName] = mObj;
                }
            }

            return mObj as T[];
        }
        #endregion

        #region 加载资源
        /// <summary>
        /// 加载资源
        /// </summary>
        /// <param name="bundleName">AssetBundle名字，相对的是小写路径名字</param>
        /// <returns></returns>
        private UnityEngine.Object LoadAssetRes(string bundleName,string bundlePath)
        {
            //jing xiang 
            AssetBundle assetBundle = AssetBundle.LoadFromFile(System.IO.Path.Combine(streamstring_path, "StreamingAssets"));

            AssetBundleManifest manifest = assetBundle.LoadAsset<AssetBundleManifest>("AssetBundleManifest");

            assetBundle.Unload(false);
            assetBundle = null;

            string[] dependencies = manifest.GetAllDependencies(bundleName);

            foreach (string dependency in dependencies)
            {
                if (!mDicAssetBundle.ContainsKey(dependency))
                {
                    AssetBundle ab = AssetBundle.LoadFromFile(Path.Combine(streamstring_path, dependency));
                    mDicAssetBundle.Add(dependency, ab);
                }

            }

            //必须要有文件夹包裹的ab
            //var lastSpit = bundleName.LastIndexOf('/');
            //var lastPoint = bundleName.LastIndexOf('.');
            //var name = bundleName.Substring(lastSpit + 1, lastPoint - lastSpit - 1);

            if (mDicAssetBundle.ContainsKey(bundleName))
            {
                return mDicAssetBundle[bundleName].LoadAsset(bundleName);
            }

            var bundle = AssetBundle.LoadFromFile(System.IO.Path.Combine(streamstring_path, bundlePath));

            mDicAssetBundle.Add(bundleName, bundle);

            UnityEngine.Object asset = bundle.LoadAsset(bundleName);

            return asset;

        }

        private UnityEngine.Object LoadAssetResTest(string bundleName, string bundlePath)
        {
            //jing xiang 
            AssetBundle assetBundle = AssetBundle.LoadFromFile(System.IO.Path.Combine(streamstring_path, "StreamingAssets"));

            AssetBundleManifest manifest = assetBundle.LoadAsset<AssetBundleManifest>("AssetBundleManifest");

            assetBundle.Unload(false);
            assetBundle = null;

            string[] dependencies = manifest.GetAllDependencies(bundleName);

            foreach (string dependency in dependencies)
            {
                if (!mDicAssetBundle.ContainsKey(dependency))
                {
                    AssetBundle ab = AssetBundle.LoadFromFile(Path.Combine(streamstring_path, dependency));
                    mDicAssetBundle.Add(dependency, ab);
                }

            }

            if (mDicAssetBundle.ContainsKey(bundleName))
            {
                return mDicAssetBundle[bundleName].LoadAsset(bundleName);
            }

            var bundle = AssetBundle.LoadFromFile(System.IO.Path.Combine(streamstring_path, bundlePath));

            mDicAssetBundle.Add(bundleName, bundle);

            UnityEngine.Object asset = bundle.LoadAsset(bundleName);

            return asset;

        }

        private UnityEngine.Object[] LoadALLAssetRes(string bundleName)
        {
            string path = Application.streamingAssetsPath + "/";
            //jing xiang 
            AssetBundle assetBundle = AssetBundle.LoadFromFile(System.IO.Path.Combine(path, "StreamingAssets"));

            AssetBundleManifest manifest = assetBundle.LoadAsset<AssetBundleManifest>("AssetBundleManifest");

            assetBundle.Unload(false);
            assetBundle = null;

            string[] dependencies = manifest.GetAllDependencies(bundleName);

            foreach (string dependency in dependencies)
            {
                if (!mDicAssetBundle.ContainsKey(dependency))
                {
                    AssetBundle ab = AssetBundle.LoadFromFile(Path.Combine(path, dependency));
                    mDicAssetBundle.Add(dependency, ab);
                }

            }

            string[] two = bundleName.Split('/');

            var bundle = AssetBundle.LoadFromFile(System.IO.Path.Combine(path, bundleName));

            mDicAssetBundle.Add(bundleName, bundle);

            UnityEngine.Object[] asset = bundle.LoadAssetWithSubAssets(two[two.Length - 1]);

            return asset;
        }

        #endregion


    }

    #region Serializable
    [Serializable]
    public class KeyValuesInfo
    {
        public List<KeyValuesNode> mKeyValuesInfo;
    }
    [Serializable]
    public class KeyValuesNode
    {
        public string mKey;
        public string mValues;
    }

    #endregion
}
