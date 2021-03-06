using UnityEngine;

/*
 * @author LuoZichu
 * @time 2019/7/1
 */

namespace RedRedJiang.Unity
{
    public class FingerEffect 
    {
        private bool mIsOnFingerEffect;
        public bool IsOnFingerEffect { get { return mIsOnFingerEffect; }set { mIsOnFingerEffect = value; } }

        private Transform mPopTrans;
        public Transform PopTrans { get { return mPopTrans = mPopTrans ?? UICanvas.Find("Pop"); } }

        private Transform mUICanvas;
        public Transform UICanvas { get { return mUICanvas = mUICanvas ?? UITool.GetUICanvas().transform; } }

        private Camera mUICamera;
        public Camera UICamera { get { return mUICamera = mUICamera ?? UITool.FindChild<Camera>(UICanvas.gameObject, "UI_Camera"); } }
        public void Update()
        {
           
#if UNITY_EDITOR
            //if(Input.GetMouseButtonDown(0) && IsOnFingerEffect)
            //{
            //    CreatePoint(Input.mousePosition);
            //}
#endif 
            //if (Input.touchCount == 1 && IsOnFingerEffect)
            //{
            //    CreatePoint(Input.GetTouch(0).position);
            //}
        }

        private void CreatePoint(Vector3 pos)
        {

            GameObject point =  UnityTool.CreateGameObject("Img_PointFinger", PopTrans);

            Vector3 worldpos;

            RectTransformUtility.ScreenPointToWorldPointInRectangle(UICanvas.GetComponent<RectTransform>(), pos, null, out worldpos);

            point.transform.position = worldpos;


            if (point)
                UnityEngine.Object.Destroy(point.gameObject, 0.6f);
        }
    }
}
