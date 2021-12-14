using LitJson;
using UnityEngine;
using BestHTTP;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace RedRedJiang.Unity
{
    [Serializable]
    public class RemoteConfig
    {
        public string remoteRes;
        public string remoteMD5file;
    }

    public class ResourceVersionCheckSystem : SystemBase<ResourceVersionCheckSystem>, ISystem
    {
        //配置文件路径
        //远程资源地址
        //远程ResourcesList_MD5地址
        //下载后和本地做对比
        //下载差异化的资源
        //把差异化的资源替换掉

        /// <summary>
        /// 配置文件
        /// </summary>
        private readonly string _resourceVersionName = "ResLink";
        //本地MD5码文件名
        public readonly string _localResourceVersionName = "ResourcesList_MD5";
        //本地下载目录名
        private readonly string _persistentRes = "/Res";

        //下载列表
        private List<string> downFile = new List<string>();

        //远程配置
        private RemoteConfig rConfig;

        public void Init()
        {
            var fileTa = Resources.Load<TextAsset>(_resourceVersionName);

            rConfig = JsonMapper.ToObject<RemoteConfig>(fileTa.text);

            if (rConfig == null)
            {
                Debug.Log("rConfig is null!");
                return;
            }

            HTTPRequest request = new HTTPRequest(new Uri(rConfig.remoteMD5file), OnRequestFinished);
            request.Send();

            
        }

        /// <summary>
        /// 下载差异文件并覆盖
        /// </summary>
        /// <param name="item"></param>
        private void HttpGet(string item)
        {
            var remotePathFile = rConfig.remoteRes +  item;
            Debug.Log("down url path:" + remotePathFile);
            var request_item = new HTTPRequest(new Uri(remotePathFile), (req, resp) =>
            {
                //resp.Data
                switch (req.State)
                {
                    // The request finished without any problem.
                    case HTTPRequestStates.Finished:
                        if (resp.IsSuccess)
                        {
                            //下载本地路径 persistentDataPath
                            //对应创建目录
                            //写入文件
                            //旧本地路径 streamingAssetsPath
                            //移除旧文件，复制新文件
                            //改写本地MD5文件

                            var path = Application.persistentDataPath + _persistentRes + item;
                            var oldFilePath = Application.streamingAssetsPath + item;
                            var directory = Path.GetDirectoryName(path);

                            Directory.CreateDirectory(directory);

                            Debug.Log(path);

                            FileStream filestr = File.Create(path);
                            filestr.Write(resp.Data, 0, resp.Data.Length);
                            filestr.Flush(); //流会缓冲，此行代码指示流不要缓冲数据，立即写入到文件。
                            filestr.Close(); //关闭流并释放所有资源，同时将缓冲区的没有写入的数据，写入然后再关闭。
                            filestr.Dispose();//释放流所占用的资源，Dispose()会调用Close(),Close()会调用Flush(); 也会写入缓冲区内的数据。


                            File.Delete(oldFilePath);
                            File.Copy(path, oldFilePath);


                        }
                        else
                        {
                            Debug.LogWarning(string.Format("Request finished Successfully, but the server sent an error. Status Code: {0}-{1} Message: {2}",
                                                            resp.StatusCode,
                                                            resp.Message,
                                                            resp.DataAsText));
                        }
                        break;

                    // The request finished with an unexpected error. The request's Exception property may contain more info about the error.
                    case HTTPRequestStates.Error:
                        Debug.LogError("Request Finished with Error! " + (req.Exception != null ? (req.Exception.Message + "\n" + req.Exception.StackTrace) : "No Exception"));
                        break;

                    // The request aborted, initiated by the user.
                    case HTTPRequestStates.Aborted:
                        Debug.LogWarning("Request Aborted!");
                        break;

                    // Connecting to the server is timed out.
                    case HTTPRequestStates.ConnectionTimedOut:
                        Debug.LogError("Connection Timed Out!");
                        break;

                    // The request didn't finished in the given time.
                    case HTTPRequestStates.TimedOut:
                        Debug.LogError("Processing the request Timed Out!");
                        break;
                }

            });
            request_item.Send();
        }

        /// <summary>
        /// 检查差异文件
        /// </summary>
        /// <param name="request"></param>
        /// <param name="response"></param>
        void OnRequestFinished(HTTPRequest request, HTTPResponse response)
        {
           

            switch (request.State)
            {
                // The request finished without any problem.
                case HTTPRequestStates.Finished:
                    if (response.IsSuccess)
                    {
                        // Everything went as expected!
                        Debug.Log("Request Finished! remote Text received: " + response.DataAsText);

                        //var path = Path.Combine(Application.streamingAssetsPath, _localResourceVersionName);
                        //var path = Path.Combine(Application)
                        try
                        {
                            //var pare_local_str = File.ReadAllText(path);

                            var pare_local_str = Resources.Load<TextAsset>(_localResourceVersionName).text;
                            
                            Debug.Log("local Text received:" + pare_local_str);

                            var pare_remote_str = response.DataAsText;

                            var localInfo = JsonMapper.ToObject<KeyValuesInfo>(pare_local_str);

                            var remoteInfo = JsonMapper.ToObject<KeyValuesInfo>(pare_remote_str);

                            var resLocal = localInfo.mKeyValuesInfo.Select(r => r.mKey);

                            var resRemote = remoteInfo.mKeyValuesInfo.Select(r => r.mKey);

                            var keys = resRemote.Except(resLocal).ToList();

                            foreach (var item in remoteInfo.mKeyValuesInfo)
                            {
                                foreach (var key in keys)
                                {
                                    if (item.mKey.Equals(key))
                                    {
                                        downFile.Add(item.mValues);
                                    }
                                }
                            }

                            if (downFile.Count > 0)
                            {
                                foreach (var item in downFile)
                                {
                                    HttpGet(item);
                                }

                                File.WriteAllText(Path.Combine(Application.dataPath, "Resources", _localResourceVersionName + ".json"), pare_remote_str);
                            }

                            downFile.Clear();
                        }
                        catch (Exception ex)
                        {
                            Debug.LogError(ex.Message);
                        }

                    }
                    else
                    {
                        Debug.LogWarning(string.Format("Request finished Successfully, but the server sent an error. Status Code: {0}-{1} Message: {2}",
                                                        response.StatusCode,
                                                        response.Message,
                                                        response.DataAsText));
                    }
                    break;

                // The request finished with an unexpected error. The request's Exception property may contain more info about the error.
                case HTTPRequestStates.Error:
                    Debug.LogError("Request Finished with Error! " + (request.Exception != null ? (request.Exception.Message + "\n" + request.Exception.StackTrace) : "No Exception"));
                    break;

                // The request aborted, initiated by the user.
                case HTTPRequestStates.Aborted:
                    Debug.LogWarning("Request Aborted!");
                    break;

                // Connecting to the server is timed out.
                case HTTPRequestStates.ConnectionTimedOut:
                    Debug.LogError("Connection Timed Out!");
                    break;

                // The request didn't finished in the given time.
                case HTTPRequestStates.TimedOut:
                    Debug.LogError("Processing the request Timed Out!");
                    break;
            }

        }

    }

}
