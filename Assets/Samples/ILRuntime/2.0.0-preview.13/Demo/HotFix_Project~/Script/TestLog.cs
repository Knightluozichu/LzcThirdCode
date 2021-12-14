using UnityEngine;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using UnityEngine.UI;

/*
 * @author LuoZichu
 * @time 2019/7/1
 */

public class TestLog : MonoBehaviour
{
    //[SerializeField]
    //private Image _Image;
    //private GameObject mCanvas;

    //private void Start()
    //{
    //    TestLoadWWW();
    //}

    //private void TestLoadWWW()
    //{
    //    mCanvas = GameObject.Find("Canvas");

    //    string bundleName = "prefabs/cube";

    //    Object obj = LoadAssetRes(bundleName);

    //    GameObject gob = Instantiate(obj) as GameObject;

    //    //_Image.sprite = Instantiate<Sprite>(LoadAssetRes("textures/beijing") as Sprite);
    //    //加载预设
    //    string bundNameimage1 = "prefabs/image";
    //    Object image1 = LoadAssetRes(bundNameimage1);
    //    GameObject gob1 = Instantiate(image1) as GameObject;
    //    gob1.transform.SetParent(mCanvas.transform);
    //    gob1.transform.localPosition = Vector3.zero;

    //    //图片单独加载
    //    Texture2D t2d = LoadAssetRes("textures/beijing") as Texture2D;

    //    Sprite spr = Sprite.Create(t2d, new Rect(0, 0, t2d.width, t2d.height), Vector2.one * 1 / 2);

    //    _Image.sprite = spr;
    //}

    //private Object LoadAssetRes(string bundleName)
    //{
       
    //    string path = Application.streamingAssetsPath +"/";

    //    AssetBundle assetBundle = AssetBundle.LoadFromFile(System.IO.Path.Combine(path, "StreamingAssets"));

    //    AssetBundleManifest manifest = assetBundle.LoadAsset<AssetBundleManifest>("AssetBundleManifest");

    //    assetBundle.Unload(false);
    //    assetBundle = null;

    //    string[] dependencies = manifest.GetAllDependencies(bundleName); //Pass the name of the bundle you want the dependencies for.

    //    foreach (string dependency in dependencies)
    //    {
    //        AssetBundle.LoadFromFile(Path.Combine(path, dependency));
    //    }

    //    var bundle = AssetBundle.LoadFromFile(System.IO.Path.Combine(path, bundleName));

    //    string[] two = bundleName.Split('/');

    //    Object asset = bundle.LoadAsset(two[1]);

    //    return asset ;

    //}
 
}
