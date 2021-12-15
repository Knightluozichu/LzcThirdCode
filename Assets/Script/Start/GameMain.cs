using UnityEngine;
using System.Collections;
using System.IO;
using ILRuntime.Runtime.Enviorment;
using UnityEngine.Networking;
using System;
using AppDomain = ILRuntime.Runtime.Enviorment.AppDomain;

namespace RedRedJiang.Unity.Start
{
    //��������Ϊ��ȡ��ʹ��WWW�ľ��棬Unity2018�Ժ��Ƽ�ʹ��UnityWebRequest�����ڼ����Կ���Demo��Ȼʹ��WWW
#pragma warning disable CS0618
    public class GameMain : MonoBehaviour
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
            //������Ŀ��Ӧ�������д������ط�����dll�����ߴ����AssetBundle�ж�ȡ��ƽʱ�����Լ�Ϊ����ʾ����ֱ�Ӵ�StreammingAssets�ж�ȡ��
            //��ʽ������ʱ����Ҫ������д������ط���ȡdll

            //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            //���DLL�ļ���ֱ�ӱ���HotFix_Project.sln���ɵģ��Ѿ�����Ŀ�����ú����Ŀ¼ΪStreamingAssets����VS��ֱ�ӱ��뼴�����ɵ���ӦĿ¼�������ֶ�����
            //����Ŀ¼��Assets\Samples\ILRuntime\1.6\Demo\HotFix_Project~
            //���¼���д��ֻΪ��ʾ����û�д����ڱ༭���л���Androidƽ̨�Ķ�ȡ����Ҫ�����޸�
#if UNITY_ANDROID
        WWW www = new WWW(Application.streamingAssetsPath + "/startgame/hotfix_project.dll.bytes");
#else
            WWW www = new WWW("file:///" + Application.streamingAssetsPath + "/startgame/hotfix_project.dll.bytes");
#endif
            
                yield return www;
            if (!string.IsNullOrEmpty(www.error))
                UnityEngine.Debug.LogError(www.error);
            AssetBundle bundle = www.assetBundle;
            TextAsset asset = bundle.LoadAsset("HotFix_Project.dll", typeof(TextAsset)) as TextAsset;
            byte[] dll = asset.bytes;
            www.Dispose();

            //PDB�ļ��ǵ������ݿ⣬����Ҫ����־����ʾ������кţ�������ṩPDB�ļ����������ڻ��������ڴ棬��ʽ����ʱ�뽫PDBȥ��������LoadAssembly��ʱ��pdb��null����
#if UNITY_ANDROID
        www = UnityWebRequest.Get(Application.streamingAssetsPath + "/startgame/hotfix_project.pdb.bytes");
#else
            www = new WWW("file:///" + Application.streamingAssetsPath + "/startgame/hotfix_project.pdb.bytes");
#endif
            yield return www;
            if (!string.IsNullOrEmpty(www.error))
                UnityEngine.Debug.LogError(www.error);
            bundle = www.assetBundle;
            asset = bundle.LoadAsset("HotFix_Project.pdb", typeof(TextAsset)) as TextAsset;
            byte[] pdb = asset.bytes;
            fs = new MemoryStream(dll);
            p = new MemoryStream(pdb);
            try
            {
                appdomain.LoadAssembly(fs);
                //appdomain.LoadAssembly(fs, p, new ILRuntime.Mono.Cecil.Pdb.PdbReaderProvider());
            }
            catch(Exception e)
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
            var kk = appdomain.GetType("RedRedJiang.Unity.GameLoop");
            //HelloWorld����һ�η�������
            appdomain.Invoke("RedRedJiang.Unity.GameLoop", "Init", null, null);

        }

        private void OnDestroy()
        {
            if (fs != null)
                fs.Close();
            if (p != null)
                p.Close();
            fs = null;
            p = null;
        }

        void Update()
        {

        }
    }

}
