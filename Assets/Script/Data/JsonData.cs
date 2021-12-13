using LitJson;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

/*
 * @author LuoZichu
 * @time 2019/7/1
 */

namespace Assets.Script.Data
{
    class JsonData : IJsonClass
    {
        public override T LoadJson<T>(string _PathName)
        {
            
            string file = JsonDataPath() + "/" + _PathName + ".json";

            FileInfo t = new FileInfo(file);

            if (!Directory.Exists(JsonDataPath()))
            {
                Debug.Log("不存在路径--->" + file);
            }

            if (!t.Exists)
            {
                return null;
            }
            else
            {

            }

            string json = File.ReadAllText(file);
            T _playerdata = new T();
            _playerdata = JsonUtility.FromJson<T>(json);

            return _playerdata;
        }

        public override void SaveJson<T>(T _Class, string _PathName)
        {
            if (!Directory.Exists(JsonDataPath()))
            {
                Directory.CreateDirectory(JsonDataPath());
            }
            string file = JsonDataPath() + "/" + _PathName + ".json";

            FileInfo t = new FileInfo(file);

            if (!t.Exists)
            {
                t.CreateText().Dispose();
                Debug.Log(file);
            }
            else
            {
                
            }


            string json = JsonUtility.ToJson(_Class, true);


            File.WriteAllText(file, json);
        }

        public T JsonABAnalysis<T>(string _Name) where T : class
        {
            UnityEngine.Object obj = Resources.ResourcesSystem.Instance.ResObj<UnityEngine.Object>(_Name, false);
            if (!obj) return null;
            return AnalysisJson<T>(obj);
        }

        public T JsonResourcesAnalysis<T>(string _Name) where T : class
        {
            UnityEngine.Object obj = UnityEngine.Resources.Load<UnityEngine.Object>(_Name);
            if (!obj) return null;
            return AnalysisJson<T>(obj);
        }

        private T AnalysisJson<T>(UnityEngine.Object _Object)where T:class
        {
            TextAsset dataTa = UnityEngine.Object.Instantiate(_Object) as TextAsset;
            T mData = null;
            try
            {
                mData = JsonUtility.FromJson<T>(dataTa.text);
            }
            catch (Exception e)
            {
               UnityEngine.Debug.Log("错误为：" + e.Message);
            }

            return mData as T;
        }

        public void SaveResourcesPathJson<T>(T _Class ,string _Name)where T :class
        {

       
                string file = Application.dataPath + "/Resources/" + _Name + ".json";

                try
                {
                    string json = JsonUtility.ToJson(_Class, true);

                    File.WriteAllText(file, json);
                }
                catch(Exception e)
                {
                    Debug.Log(e.Message);
                }


            Debug.Log(file);

        }
    }
}
