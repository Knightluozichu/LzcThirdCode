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
        //�����ļ�·��
        //Զ����Դ��ַ
        //Զ��ResourcesList_MD5��ַ
        //���غ�ͱ������Ա�
        //���ز��컯����Դ
        //�Ѳ��컯����Դ�滻��

        /// <summary>
        /// �����ļ�
        /// </summary>
        private readonly string _resourceVersionName = "ResLink";
        public readonly string _localResourceVersionName = "ResourcesList_MD5.json";

        private List<string> downFile = new List<string>();

        public void Init()
        {
            var fileTa = Resources.Load<TextAsset>(_resourceVersionName);

            RemoteConfig rConfig = JsonMapper.ToObject<RemoteConfig>(fileTa.text);

            if (rConfig == null)
            {
                Debug.Log("rConfig is null!");
                return;
            }

            HTTPRequest request = new HTTPRequest(new Uri(rConfig.remoteMD5file), OnRequestFinished);
            request.Send();

            if (downFile.Count > 0)
            {
                foreach (var item in downFile)
                {
                    request = new HTTPRequest(new Uri(Path.Combine(rConfig.remoteRes, rConfig.remoteRes)), OnRequestFinished);
                    request.Send();
                }
            }
        }

        void OnRequestFinished(HTTPRequest request, HTTPResponse response)
        {
            Debug.Log("Request Finished! Text received: " + response.DataAsText);

            var pare_local_str = File.ReadAllText(Path.Combine(Application.streamingAssetsPath, _localResourceVersionName));

            var pare_remote_str = response.DataAsText;

            var localInfo = JsonMapper.ToObject<KeyValuesInfo>(pare_local_str);

            var remoteInfo = JsonMapper.ToObject<KeyValuesInfo>(pare_remote_str);

            var resLocal = localInfo.mKeyValuesInfo.Select(r => r.mValues);

            var resRemote = remoteInfo.mKeyValuesInfo.Select(r => r.mValues);

            downFile = (List<string>)resRemote.Except(resLocal);

            //downFile = from r in other
            //           join l in remoteInfo.mKeyValuesInfo.Select(x=>x.mValues)
            //           on r equals l
            //           select new {  }


        }

    }

}
