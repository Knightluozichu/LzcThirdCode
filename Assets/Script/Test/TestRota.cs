using RedRedJiang.Unity;
using UnityEngine;

/*
 * @author LuoZichu
 * @time 2019/7/1
 */

public class TestRota : MonoBehaviour
{
    AssetBundle assetbundle = null;

    private void Start()
    {
        //ResourcesSystem.Instance.InitAwake();

        //UnityEngine.Object[] arraySprite = ResourcesSystem.Instance.ResAltas<Object>(@"Skill/SkillAltas", true);

        //this.GetComponent<SpriteRenderer>().sprite = (Sprite)arraySprite[1];
        Strong_Item ay = new Strong_Item();
        Strong_Item ap = new Strong_Item();
        ap.num = 0;
        ap.name = "wo shi 0";

        BinarySystem.Instance.Serialize<Strong_Item>(Application.dataPath + @"/Resources/apply", ap);

        ay = BinarySystem.Instance.Deserialize<Strong_Item>(Application.dataPath + "/Resources/apply");

        Debug.Log(ay.name + ":" + ay.num);

        //TABLE.ITEMARRAY tableItemArray = BinarySystem.Instance.Deserialize<TABLE.ITEMARRAY>(Application.dataPath + "/Resources/item");

        //foreach (var i in tableItemArray.rows)
        //{
        //    Debug.Log(i.key + ":" + i.name);
        //}

        string strong_Item_Archive = "Strong_Item";

        Strong_Item sim = new Strong_Item();
        sim.name = "wangkai";
        sim.num = 1;

        BinarySystem.Instance.SerializeOfArchive<Strong_Item>(strong_Item_Archive, sim);

        Strong_Item simd = BinarySystem.Instance.DeserializeOfArchive<Strong_Item>(strong_Item_Archive);

        if (null == simd)
        {
            Debug.Log("反序列化失败！");
        }

        Debug.Log("name:" + simd.name + "--->" + simd.num);

        Debug.Log(Application.persistentDataPath);

        Debug.Log(Application.dataPath);

        Debug.Log(Application.streamingAssetsPath + "/" + "data" + "/" + "item");

        TABLE.ITEMARRAY tiy = ModelSystem.Instance.GetDataConst<TABLE.ITEMARRAY>("item");

        foreach (var i in tiy.rows)
        {
            Debug.Log("--->" + i.name + "--->" + i.key);
        }
    }

}

[System.Serializable, ProtoBuf.ProtoContract]
public class Strong_Item
{
    [ProtoBuf.ProtoMember(1)]
    public int num;
    [ProtoBuf.ProtoMember(2)]
    public string name;

}
