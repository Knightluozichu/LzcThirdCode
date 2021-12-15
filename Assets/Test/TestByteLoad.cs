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
    //下面这行为了取消使用WWW的警告，Unity2018以后推荐使用UnityWebRequest，处于兼容性考虑Demo依然使用WWW
#pragma warning disable CS0618
    public class TestByteLoad : MonoBehaviour
    {
        //AppDomain是ILRuntime的入口，最好是在一个单例类中保存，整个游戏全局就一个，这里为了示例方便，每个例子里面都单独做了一个
        //大家在正式项目中请全局只创建一个AppDomain
        AppDomain appdomain;

        System.IO.MemoryStream fs;
        System.IO.MemoryStream p;
        void Start()
        {
            StartCoroutine(LoadHotFixAssembly());
        }

        IEnumerator LoadHotFixAssembly()
        {
            //首先实例化ILRuntime的AppDomain，AppDomain是一个应用程序域，每个AppDomain都是一个独立的沙盒
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
                Debug.LogError("加载热更DLL失败，请确保已经通过VS打开Assets/Samples/ILRuntime/1.6/Demo/HotFix_Project/HotFix_Project.sln编译过热更DLL message:" + e.Message);
            }

            InitializeILRuntime();
            OnHotFixLoaded();
        }

        void InitializeILRuntime()
        {
#if DEBUG && (UNITY_EDITOR || UNITY_ANDROID || UNITY_IPHONE)
            //由于Unity的Profiler接口只允许在主线程使用，为了避免出异常，需要告诉ILRuntime主线程的线程ID才能正确将函数运行耗时报告给Profiler
            appdomain.UnityMainThreadID = System.Threading.Thread.CurrentThread.ManagedThreadId;
#endif
            //这里做一些ILRuntime的注册，HelloWorld示例暂时没有需要注册的
        }

        void OnHotFixLoaded()
        {
            //HelloWorld，第一次方法调用
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
