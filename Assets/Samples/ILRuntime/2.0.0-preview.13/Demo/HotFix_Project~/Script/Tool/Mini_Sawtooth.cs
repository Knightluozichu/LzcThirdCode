using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*
 * @author LuoZichu
 * @time 2019/7/1
 */

namespace RedRedJiang.Unity
{
    /// <summary>
    /// 徐璐 网格裁剪
    /// </summary>
    public class Mini_Sawtooth : MonoBehaviour
    {
        public enum FallingDirection
        {
            Left,
            Right
        }

        public enum FallingSide
        {
            Left,
            Right,
            Up,
            Down
        }

        [Header("跌落边")]
        public FallingSide _FallingSide;
        [Header("跌落方向")]
        public FallingDirection _FallingDirection;
        [Header("跌落百分比")]
        [Range(0f, 1f)]
        public float fallingPercentage = 0.1f;
        [Header("跌落采样数")]
        [Range(0, 50)]
        public int fallingSampleCount = 20;
        [Header("速度")]
        public float runningSpeed = 0f;

        private MeshFilter meshFilterSelf;
        private MeshRenderer meshRendererSelf;
        private Mesh meshSelf;
        private List<Vector3> verticeList;
        private List<Vector2> uvList;
        private List<int> triangleList;
        private bool isFirstToRun;
        private Vector2 rollOffset;

        private void Awake()
        {
            meshFilterSelf = GetComponent<MeshFilter>();
            meshRendererSelf = GetComponent<MeshRenderer>();
            isFirstToRun = true;
            rollOffset = meshRendererSelf.material.mainTextureOffset;
            ChangeMesh();

        }

        private void Update()
        {
            //if (isFirstToRun)
            //{
            //    isFirstToRun = false;
            //    ChangeMesh();
            //}
            //else
            //{
            //    RollTexture();
            //}
        }

        private void ChangeMesh()
        {
            float xMin, xMax, yMin, yMax;
            GetExtremeVertice(meshFilterSelf.mesh, out xMin, out xMax, out yMin, out yMax);
            ClearAllMeshData();
            CalculateMeshData(xMin, xMax, yMin, yMax, _FallingSide, _FallingDirection);
            ApplyMeshData();
        }

        private void GetExtremeVertice(Mesh mesh, out float xMin, out float xMax, out float yMin, out float yMax)
        {
            if (mesh == null || mesh.vertexCount == 0)
            {
                xMin = 0;
                xMax = 0;
                yMin = 0;
                yMax = 0;
                return;
            }
            xMin = float.MaxValue;
            xMax = float.MinValue;
            yMin = float.MaxValue;
            yMax = float.MinValue;
            for (int i = 0; i < mesh.vertices.Length; i++)
            {
                xMin = Mathf.Min(xMin, mesh.vertices[i].x);
                xMax = Mathf.Max(xMax, mesh.vertices[i].x);
                yMin = Mathf.Min(yMin, mesh.vertices[i].y);
                yMax = Mathf.Max(yMax, mesh.vertices[i].y);
            }
        }

        private void ClearAllMeshData()
        {
            meshSelf = new Mesh();
            if (verticeList == null)
            {
                verticeList = new List<Vector3>();
            }
            else
            {
                verticeList.Clear();
            }
            if (uvList == null)
            {
                uvList = new List<Vector2>();
            }
            else
            {
                uvList.Clear();
            }
            if (triangleList == null)
            {
                triangleList = new List<int>();
            }
            else
            {
                triangleList.Clear();
            }
        }

        /// <summary>
        /// 计算网格数据
        /// </summary>
        /// <param name="xMin">横坐标最小值</param>
        /// <param name="xMax">横坐标最大值</param>
        /// <param name="yMin">纵坐标最小值</param>
        /// <param name="yMax">纵坐标最大值</param>
        /// <param name="fallingSide">朝向落下的边</param>
        /// <param name="fallingDir">落下的方向</param>
        private void CalculateMeshData(float xMin, float xMax, float yMin, float yMax, FallingSide fallingSide, FallingDirection fallingDir)
        {
            #region Rect
            Vector3 ldPos, rdPos, ruPos, luPos;
            Vector2 ldUV, rdUV, ruUV, luUV;
            switch (fallingSide)
            {
                case FallingSide.Left:
                    ldPos = new Vector3(xMin, yMax, 0);
                    rdPos = new Vector3(xMin, yMin, 0);
                    ruPos = new Vector3(xMax, yMin, 0);
                    luPos = new Vector3(xMax, yMax, 0);
                    ldUV = Vector2.up;
                    rdUV = Vector2.zero;
                    ruUV = Vector2.right;
                    luUV = Vector2.one;
                    break;
                case FallingSide.Right:
                    ldPos = new Vector3(xMax, yMin, 0);
                    rdPos = new Vector3(xMax, yMax, 0);
                    ruPos = new Vector3(xMin, yMax, 0);
                    luPos = new Vector3(xMin, yMin, 0);
                    ldUV = Vector2.right;
                    rdUV = Vector2.one;
                    ruUV = Vector2.up;
                    luUV = Vector2.zero;
                    break;
                case FallingSide.Up:
                    ldPos = new Vector3(xMax, yMax, 0);
                    rdPos = new Vector3(xMin, yMax, 0);
                    ruPos = new Vector3(xMin, yMin, 0);
                    luPos = new Vector3(xMax, yMin, 0);
                    ldUV = Vector2.one;
                    rdUV = Vector2.up;
                    ruUV = Vector2.zero;
                    luUV = Vector2.right;
                    break;
                case FallingSide.Down:
                    ldPos = new Vector3(xMin, yMin, 0);
                    rdPos = new Vector3(xMax, yMin, 0);
                    ruPos = new Vector3(xMax, yMax, 0);
                    luPos = new Vector3(xMin, yMax, 0);
                    ldUV = Vector2.zero;//new Vector2(0.5f,0.5f);
                    rdUV = Vector2.right;//new Vector2(0.5f, 0.5f);
                    ruUV = Vector2.one;//new Vector2(0.5f, 0.5f);
                    luUV = Vector2.up;// new Vector2(0.5f, 0.5f);
                    break;
                default:
                    ldPos = rdPos = ruPos = luPos = Vector3.zero;
                    ldUV = rdUV = ruUV = luUV = Vector2.zero;
                    break;
            }
            Vector3 cuPos, cdPos, thirdPos;
            Vector2 cuUV, cdUV, thirdUV;
            int centerIndex = 0;
            int startingIndex = verticeList.Count;
            switch (fallingDir)
            {
                case FallingDirection.Left:
                    cuPos = Vector3.Lerp(luPos, ruPos, fallingPercentage);
                    cdPos = Vector3.Lerp(ldPos, rdPos, fallingPercentage);
                    cuUV = Vector2.Lerp(luUV, ruUV, fallingPercentage);
                    cdUV = Vector2.Lerp(ldUV, rdUV, fallingPercentage);
                    verticeList.Add(cuPos / 2);
                    verticeList.Add(cdPos / 2);
                    verticeList.Add(rdPos / 2);
                    verticeList.Add(ruPos / 2);
                    uvList.Add(cuUV);
                    uvList.Add(cdUV);
                    uvList.Add(rdUV);
                    uvList.Add(ruUV);
                    thirdPos = ldPos;
                    thirdUV = ldUV;
                    centerIndex = startingIndex + 1;
                    break;
                case FallingDirection.Right:
                    cuPos = Vector3.Lerp(ruPos, luPos, fallingPercentage);
                    cdPos = Vector3.Lerp(rdPos, ldPos, fallingPercentage);
                    cuUV = Vector2.Lerp(ruUV, luUV, fallingPercentage);
                    cdUV = Vector2.Lerp(rdUV, ldUV, fallingPercentage);
                    verticeList.Add(luPos);
                    verticeList.Add(ldPos);
                    verticeList.Add(cdPos);
                    verticeList.Add(cuPos);
                    uvList.Add(luUV);
                    uvList.Add(ldUV);
                    uvList.Add(cdUV);
                    uvList.Add(cuUV);
                    thirdPos = rdPos;
                    thirdUV = rdUV;
                    centerIndex = startingIndex + 2;
                    break;
                default:
                    cuPos = cdPos = thirdPos = Vector3.zero;
                    cuUV = cdUV = thirdUV = Vector2.zero;
                    break;
            }
            triangleList.Add(startingIndex);
            triangleList.Add(startingIndex + 2);
            triangleList.Add(startingIndex + 1);
            triangleList.Add(startingIndex);
            triangleList.Add(startingIndex + 3);
            triangleList.Add(startingIndex + 2);
            startingIndex += 4;
            #endregion

            if (fallingPercentage <= 0 || fallingSampleCount <= 1)
            {
                return;
            }

            #region HalfCircle
            float a = cuPos.y - cdPos.y;
            float b = -a / ((thirdPos.x - cdPos.x) * (thirdPos.x - cdPos.x));
            float xTemp, yTemp, uTemp, vTemp, radio;
            for (float i = 0; i < fallingSampleCount; i++)
            {
                radio = i / (fallingSampleCount - 1);
                xTemp = Mathf.Lerp(cdPos.x, thirdPos.x, radio);
                yTemp = a + b * (xTemp - cdPos.x) * (xTemp - cdPos.x) + cdPos.y;
                uTemp = Mathf.Lerp(cdUV.x, thirdUV.x, radio);
                vTemp = cuUV.y;
                verticeList.Add(new Vector3(xTemp, yTemp, 0));
                uvList.Add(new Vector2(uTemp, vTemp));
                switch (fallingDir)
                {
                    case FallingDirection.Left:
                        triangleList.Add(startingIndex - 1);
                        triangleList.Add(centerIndex);
                        triangleList.Add(startingIndex);
                        break;
                    case FallingDirection.Right:
                        triangleList.Add(startingIndex - 1);
                        triangleList.Add(startingIndex);
                        triangleList.Add(centerIndex);
                        break;
                    default:
                        break;
                }
                startingIndex++;
            }
            #endregion
        }

        public Renderer RollTexture()
        {
            //rollOffset += Vector2.right * runningSpeed * Time.deltaTime;
            //if (rollOffset.x > 1)
            //{
            //    rollOffset.x--;
            //}
            //if (rollOffset.x < 0)
            //{
            //    rollOffset.x++;
            //}
            // meshRendererSelf.material.mainTextureOffset = rollOffset;
            return meshRendererSelf;
        }

        private void ApplyMeshData()
        {
            meshSelf.vertices = verticeList.ToArray();
            meshSelf.uv = uvList.ToArray();
            meshSelf.triangles = triangleList.ToArray();
            meshFilterSelf.mesh = meshSelf;
        }
    }

}

