//using RedRedJiang.Unity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using UnityEditor;
using UnityEngine;


public static class ResourcesMD5
{
    /// <summary>
    /// 获取文件MD5
    /// </summary>
    /// <param name="filePath"></param>
    /// <returns></returns>
    public static string GetFileHash(string filePath)
    {
        try
        {
            FileStream fs = new FileStream(filePath, FileMode.Open);
            int len = (int)fs.Length;
            byte[] data = new byte[len];
            fs.Read(data, 0, len);
            fs.Close();
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] result = md5.ComputeHash(data);
            string fileMD5 = "";
            foreach (byte b in result)
            {
                fileMD5 += Convert.ToString(b, 16);
            }
            return fileMD5;
        }
        catch (FileNotFoundException e)
        {
            Console.WriteLine(e.Message);
            return "";
        }
    }

    public static bool CompareHash(string bo1, string bo2)
    {
        return bo1.Equals(bo2);
    }
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

public class BuildAssetBundle
{
    //可修改
    private const string mPathResourcesName = "Resources"; //映射关系 存储json文件夹名称
    private const string mPathName = "Art";//要打资源的目录
    private static string mPathInfo = "ResourcesList";//名字和路径 映射关系 json文件名称
    private static string mPathInfo_MD5 = "ResourcesList_MD5";//路径和md5 映射关系 json文件名称

    //可修改
    //要打资源的目录下的文件夹名字
    private const string mPrefabsPathName = "Prefabs";
    private const string mTexturesPathName = "Textures";
    private const string mMaterialsPathName = "Materials";
    private const string mAnimationPathName = "Animation";
    private const string mAnimatorPathName = "Animator";
    private const string mFontPathName = "Font";
    private const string mDataPathName = "Data";
    private const string mAudioPathName = "Audio";
    private const string mShaderPathName = "Shader";

    private static KeyValuesInfo mKeyValue = new KeyValuesInfo();
    private static List<KeyValuesNode> mListKey = new List<KeyValuesNode>();

    //MD5
    private static KeyValuesInfo mKeyValue_MD5 = new KeyValuesInfo();

    [MenuItem("AssetBundle/Build All")]
    public static void CreateAllAB()
    {
        Caching.ClearCache();
        string path = Application.streamingAssetsPath;
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
            AssetDatabase.Refresh();
        }

        BuildPipeline.BuildAssetBundles(path, BuildAssetBundleOptions.ChunkBasedCompression, EditorUserBuildSettings.activeBuildTarget);
        AssetDatabase.Refresh();
    }

    [MenuItem("AssetBundle/Set BuildAsset")]
    public static void CreateBuildAsset()
    {
        SetAssetName(mPrefabsPathName);
        SetAssetName(mTexturesPathName);
        SetAssetName(mMaterialsPathName);
        SetAssetName(mAnimationPathName);//
        SetAssetName(mAnimatorPathName);
        SetAssetName(mFontPathName);
        SetAssetName(mDataPathName);
        SetAssetName(mAudioPathName);
        SetAssetName(mShaderPathName);
        Init();
    }

    [MenuItem("AssetBundle/Make MD5")]
    private static void MakeMd5Async()
    {
        var jsonPath = Path.Combine(Application.streamingAssetsPath, mPathInfo_MD5 + ".json");

        if(File.Exists(jsonPath))
        {
            File.Delete(jsonPath);
            AssetDatabase.Refresh();
        }

        if (!Directory.Exists(Application.streamingAssetsPath))
        {
            Debug.LogError("Application.streamingAssetsPath 文件夹不存在！");
            return;
        }

        mKeyValue_MD5.mKeyValuesInfo = new List<KeyValuesNode>();

        FileInfo[] fileInfos = new DirectoryInfo(Application.streamingAssetsPath).GetFiles("*", SearchOption.AllDirectories);

        var key_Md5 = default(KeyValuesNode);

        foreach (FileInfo file in fileInfos)
        {
            if (file.Name.EndsWith(".meta"))
            {
                continue;
            }
            var md5 = ResourcesMD5.GetFileHash(file.FullName);

            if (md5 != null)
            {
                key_Md5 = new KeyValuesNode();
                var fullName = file.FullName.Replace("\\", "/");
                key_Md5.mValues = fullName.Replace(Application.streamingAssetsPath, string.Empty);
                key_Md5.mKey = md5;
                mKeyValue_MD5.mKeyValuesInfo.Add(key_Md5);
            }
        }

        string json = JsonUtility.ToJson(mKeyValue_MD5, true);

        
        if (!File.Exists(jsonPath))
        {
            File.Create(jsonPath).Close();
        }
        
        File.WriteAllText(jsonPath, json);

        AssetDatabase.Refresh();

        Debug.Log("MD5json文件制作完成！");
    }

    [MenuItem("AssetBundle/Clear All AB Name")]
    public static void ClearAssetBundleName()
    {
        if (Directory.Exists(Application.streamingAssetsPath))
        {
            CheckDirectories(Application.streamingAssetsPath);
        }
        else
        {
            Directory.CreateDirectory(Application.streamingAssetsPath);
            AssetDatabase.Refresh();
        }

        mKeyValue = new KeyValuesInfo();
        mListKey = new List<KeyValuesNode>();

        string[] allAssetBundleNames = AssetDatabase.GetAllAssetBundleNames();

        EditorUtility.DisplayProgressBar("清除AssetName名称", "正在设置AssetName名称...", 0f);

        for (int i = 0; i < allAssetBundleNames.Length; i++)
        {
            EditorUtility.DisplayProgressBar("清除AssetName名称", "正在设置AssetName名称...", 1f * i / allAssetBundleNames.Length);

            AssetDatabase.RemoveAssetBundleName(allAssetBundleNames[i], true);

        }
        EditorUtility.ClearProgressBar();

        number = 0;
        EditorUtility.DisplayProgressBar("清除文件夹", "正在清除文件夹...", 0f);

        EditorUtility.ClearProgressBar();

    }

    private static float number = 0;
    private static void CheckDirectories(string path)
    {
        number++;
        EditorUtility.DisplayProgressBar("清除文件夹", "正在清除文件夹...", number / Directory.GetFileSystemEntries(path).Length);
        if (Directory.Exists(path))
        {
            foreach (string i in Directory.GetFileSystemEntries(path))
            {
                if (File.Exists(i))
                {
                    File.Delete(i);
                }
                else
                {
                    CheckDirectories(i);
                }
            }
        }
        else
        {
            Directory.Delete(path);
        }
    }
    private static void DeleteAllField(string path)
    {
        if (!Directory.Exists(path))
        {
            Debug.Log(path + "不存在这个文件夹！");
            return;
        }

        foreach (var i in Directory.GetFileSystemEntries(path))
        {
            if (File.Exists(i))
            {
                File.Delete(i);
                Debug.Log(i);
            }
            else
            {
                DeleteAllField(path);
            }
        }



    }

    /// <summary>
    /// 设置名字
    /// </summary>
    /// <param name="name"></param>
    private static void SetAssetName(string name)
    {
        string path = Application.dataPath + "/" + mPathName + "/" + name + "/";

        if (!Directory.Exists(path)) { Debug.Log(path + "路径不存在"); return; }

        DirectoryInfo dirInfo = new DirectoryInfo(path);

        EditorUtility.DisplayProgressBar("设置" + name + "名称", "正在设置" + name + "名称中...", 0f);

        FileInfo[] fileInfo = dirInfo.GetFiles("*", SearchOption.AllDirectories);

        for (int i = 0; i < fileInfo.Length; i++)
        {
            FileInfo mFileInfo = fileInfo[i];

            EditorUtility.DisplayProgressBar("设置" + name + "名称", "正在设置" + name + "名称中...", i * 1f / fileInfo.Length);

            if (!mFileInfo.Name.EndsWith(".meta"))
            {
                string basePath = "Assets" + mFileInfo.FullName.Substring(Application.dataPath.Length);

                string assetName = mFileInfo.FullName.Substring(path.Length);

                assetName = assetName.Substring(0, assetName.LastIndexOf('.'));

                assetName = assetName.Replace('\\', '/');

                AssetImporter importer = AssetImporter.GetAtPath(basePath);

                if (importer && importer.assetBundleName != assetName)
                {
                    importer.assetBundleName = name + "/" + assetName;
                    importer.assetBundleVariant = "bytes";
                }

                KeyValuesNode mKeyValuesNode = new KeyValuesNode();
                mKeyValuesNode.mKey = assetName;
                mKeyValuesNode.mValues = name + "/" + assetName + ".bytes";
                mListKey.Add(mKeyValuesNode);

            }
        }
        mKeyValue.mKeyValuesInfo = mListKey;
        EditorUtility.ClearProgressBar();   //清除进度条
    }
    public static void Init()
    {
        string json = JsonUtility.ToJson(mKeyValue, true);
        Debug.Log("设置AssetBundle名字成功：" + Application.dataPath + "/" + mPathResourcesName + "/" + mPathInfo + ".json");
        File.WriteAllText(Application.dataPath + "/" + mPathResourcesName + "/" + mPathInfo + ".json", json);

    }



}


