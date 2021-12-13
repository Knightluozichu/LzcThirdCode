using Assets.Script.Base;
using UnityEngine;
using Assets.Script.Tool;
using UnityEngine.UI;
using Assets.Script.Game;
using Assets.Script.Enum;
using Assets.Script.Data;
using Assets.Script.Data.Model;
using Assets.Script.Audio;
using Assets.Script.UI.Ctrl;
using Assets.Script.UI.Data;

/*
 * @author LuoZichu
 * @time 2019/7/1
 */

namespace Assets.Script.UI
{
    public class StartPanelView : UIBase
    {

        #region Field

        #endregion

        #region UIComponent

        private Button mButtonOnClick;
        public Button ButtonOnClick { get { return mButtonOnClick = mButtonOnClick ?? UITool.FindChild<Button>(GojPanel, "Button"); } }

        #endregion

        #region Method

        #region Public 
        public override void UIEnd()
        {
            base.UIEnd();
            mUICtrtlBaseRef.UICtrlEnd();
            mButtonOnClick = null;
            mUICtrtlBaseRef = null;
            mUIDataBaseRef = null;
        }
        public StartPanelView() : base(Common.CommonClass.mStartPanelName)
        {
            mUIForm_Pos = Enum.UIFormPos.Normal;
        }

        public override void UIInit()
        {
            base.UIInit();

            mUIDataBaseRef = new StartPanelData();
            mUICtrtlBaseRef = new StartPanelCtrl(this, mUIDataBaseRef);

            UICtrlInit();
            //ButtonOnClick.onClick.AddListener(() => { Open("SecondPanel"); });
        }

        #region Private


        #endregion

        #endregion


        #endregion

    }
}
