using UnityEngine;
using System.Collections;
using System.IO;
using ILRuntime.Runtime.Enviorment;
using UnityEngine.Networking;
using System;
using AppDomain = ILRuntime.Runtime.Enviorment.AppDomain;
using System.Reflection;

namespace RedRedJiang.Unity.Start
{
    //��������Ϊ��ȡ��ʹ��WWW�ľ��棬Unity2018�Ժ��Ƽ�ʹ��UnityWebRequest�����ڼ����Կ���Demo��Ȼʹ��WWW
#pragma warning disable CS0618
    public class TestByteLoad : MonoBehaviour
    {
        //AppDomain��ILRuntime����ڣ��������һ���������б��棬������Ϸȫ�־�һ��������Ϊ��ʾ�����㣬ÿ���������涼��������һ��
        //�������ʽ��Ŀ����ȫ��ֻ����һ��AppDomain
        AppDomain appdomain;

        System.IO.MemoryStream fs;
        System.IO.MemoryStream p;
        void Start()
        {
            StartCoroutine(LoadHotFixAssembly());
        }

        IEnumerator LoadHotFixAssembly()
        {
            //����ʵ����ILRuntime��AppDomain��AppDomain��һ��Ӧ�ó�����ÿ��AppDomain����һ��������ɳ��
            appdomain = new ILRuntime.Runtime.Enviorment.AppDomain();

            WWW www = new WWW(Application.streamingAssetsPath + "/startgame/hotfix_project.dll.bytes");
            yield return www;
            AssetBundle bundle = www.assetBundle;
            TextAsset asset = bundle.LoadAsset("HotFix_Project.dll", typeof(TextAsset)) as TextAsset;

            var assembly = Assembly.Load(asset.bytes);


            //fs = new MemoryStream(dll);
            //p = new MemoryStream(pdb);
            try
            {
                appdomain.LoadAssembly(new MemoryStream(asset.bytes));
                //appdomain.LoadAssembly(fs, p, new ILRuntime.Mono.Cecil.Pdb.PdbReaderProvider());
            }
            catch (Exception e)
            {
                Debug.LogError("�����ȸ�DLLʧ�ܣ���ȷ���Ѿ�ͨ��VS��Assets/Samples/ILRuntime/1.6/Demo/HotFix_Project/HotFix_Project.sln������ȸ�DLL message:" + e.Message);
            }

            InitializeILRuntime();
            OnHotFixLoaded();
        }

        void InitializeILRuntime()
        {
#if DEBUG && (UNITY_EDITOR || UNITY_ANDROID || UNITY_IPHONE)
            //����Unity��Profiler�ӿ�ֻ���������߳�ʹ�ã�Ϊ�˱�����쳣����Ҫ����ILRuntime���̵߳��߳�ID������ȷ���������к�ʱ�����Profiler
            appdomain.UnityMainThreadID = System.Threading.Thread.CurrentThread.ManagedThreadId;
#endif
            //������һЩILRuntime��ע�ᣬHelloWorldʾ����ʱû����Ҫע���
        }

        void OnHotFixLoaded()
        {
            //HelloWorld����һ�η�������
            appdomain.Invoke("HotFix_Project.InstanceClass", "StaticFunTest", null, null);

        }

        private void OnDestroy()
        {
            //if (fs != null)
            //    fs.Close();
            //if (p != null)
            //    p.Close();
            //fs = null;
            //p = null;
        }

        void Update()
        {

        }
    }

}
