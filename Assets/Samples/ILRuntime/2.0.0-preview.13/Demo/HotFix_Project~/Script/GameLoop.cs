using UnityEngine;

/*
 * @author LuoZichu
 * @time 2019/7/1
 */

namespace RedRedJiang.Unity
{
    public class GameLoop : MonoBehaviour
    {
        FlieLog fl;
        private void Awake()
        {
            fl = new FlieLog(true);
            LogMsg.Init(fl);
            DontDestroyOnLoad(this.gameObject);
            Application.targetFrameRate = 60;
            GameFacade.Instance.Init();
        }

        private void Start()
        {
           
            SceneStateController.Instance.SetState(new TheFrontLine_1(), false);
            
        }

        //private void OnGUI()
        //{
        //    GUIStyle style = new GUIStyle
        //    {
        //        border = new RectOffset(10, 10, 10, 10),
        //        fontSize = 25,
        //        fontStyle = FontStyle.BoldAndItalic,
        //    };
        //    // normal:Rendering settings for when the component is displayed normally.
        //    style.normal.textColor = new Color(200 / 255f, 180 / 255f, 150 / 255f);    // 需要除以255，因为范围是0-1
        //    //GUI.Label(new Rect(100, 100, 200, 80), fl.context, style);
        //    GUILayout.Label(fl.context, style);

        //    //GUI.Label(new Rect(Screen.width - 100, Screen.height - 100, 200, 80),
        //        //"<color=#00ff00><size=30>" + "aaa" + "</size></color>", style);    // 支持标记语言（什么富文本？
        //                                                                           // 只支持color,size,b,i
        //}

        private void Update()
        {
            if(SceneStateController.Instance != null)
            {
                SceneStateController.Instance.StateUpdate();
            }

        }

         

        private void OnDestroy()
        {
            LogMsg.UnLog();
            GameFacade.Instance.End();
          
        }

        private void OnApplicationQuit()
        {
            
        }

    }
}
