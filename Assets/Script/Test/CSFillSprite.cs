using UnityEngine;
using System.Collections;

/*
 * @author LuoZichu
 * @time 2019/7/1
 */

    /// <summary>
    /// 王者 血条
    /// </summary>
public class CSFillSprite : MonoBehaviour
{
    public Material mat;
    [SerializeField]
    [HideInInspector]
    private Texture2D mPicture;
    public UnityEngine.Texture2D Picture
    {
        get { return mPicture; }
        set
        {
            if (mPicture == value) return;
            mPicture = value;
            Fill();
        }
    }

    [SerializeField]
    [HideInInspector]
    private float mFillCount = 1;
    public float fillAmount
    {
        get { return mFillCount; }
        set
        {
            value = Mathf.Clamp01(value);
            if (mFillCount == value) return;
            mFillCount = value;
            Fill();
        }
    }

    [SerializeField]
    [HideInInspector]
    private Color mColor = Color.white;
    public UnityEngine.Color Color
    {
        get { return mColor; }
        set
        {
            if (mColor.Equals(value)) return;
            mColor = value;
            Fill();
        }
    }

    [SerializeField]
    [HideInInspector]
    private MeshFilter mMeshFilter;
    [SerializeField]
    [HideInInspector]
    static Vector3[] verts = null;
    static Vector2[] uvs = null;
    static Color[] colors = null;
    static int[] triangles = null;
    [SerializeField]
    [HideInInspector]
    private MeshRenderer mMeshRenderer;

    void Start()
    {
        Fill();
    }

    void Fill()
    {
        if (mat == null) return;
        if (mMeshFilter == null)
        {
            mMeshFilter = gameObject.AddComponent<MeshFilter>();
        }
        if (mMeshRenderer == null)
        {
            mMeshRenderer = gameObject.AddComponent<MeshRenderer>();
            mat.mainTexture = mPicture;
            mMeshRenderer.sharedMaterial = mat;
        }

        if (verts == null || verts.Length != 4) verts = new Vector3[4];
        if (uvs == null || uvs.Length != 4) uvs = new Vector2[4];
        if (colors == null || colors.Length != 4) colors = new Color[4];
        if (triangles == null || triangles.Length != 4) triangles = new int[6];

        verts[0].x = 0;
        verts[0].y = 0;
        verts[1].x = 0;
        verts[1].y = mPicture.height;
        verts[2].x = mPicture.width * fillAmount;
        verts[2].y = mPicture.height;
        verts[3].x = mPicture.width * fillAmount;
        verts[3].y = 0;

        uvs[0].x = 0;
        uvs[0].y = 0;
        uvs[1].x = 0;
        uvs[1].y = 1;
        uvs[2].x = mFillCount;
        uvs[2].y = 1;
        uvs[3].x = mFillCount;
        uvs[3].y = 0;

        triangles[0] = 0;
        triangles[1] = 1;
        triangles[2] = 2;
        triangles[3] = 0;
        triangles[4] = 2;
        triangles[5] = 3;

        Color col = Color;
        colors[0] = col;
        colors[1] = col;
        colors[2] = col;
        colors[3] = col;

        if (Application.isPlaying)
        {
            if (mMeshFilter.mesh == null) mMeshFilter.mesh = new Mesh();
            mMeshFilter.mesh.Clear();
            mMeshFilter.mesh.vertices = verts;
            mMeshFilter.mesh.colors = colors;
            mMeshFilter.mesh.uv = uvs;
            mMeshFilter.mesh.triangles = triangles;
            mMeshFilter.mesh.normals = verts;
        }
        else
        {
            if (mMeshFilter.sharedMesh == null) mMeshFilter.sharedMesh = new Mesh();
            mMeshFilter.sharedMesh.Clear();
            mMeshFilter.sharedMesh.vertices = verts;
            mMeshFilter.sharedMesh.colors = colors;
            mMeshFilter.sharedMesh.uv = uvs;
            mMeshFilter.sharedMesh.triangles = triangles;
            mMeshFilter.sharedMesh.normals = verts;
        }
    }
}
