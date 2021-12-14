using UnityEngine.UI;

/*
 * @author LuoZichu
 * @time 2019/7/1
 */

namespace RedRedJiang.Unity
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
        public StartPanelView() : base(CommonClass.mStartPanelName)
        {
            mUIForm_Pos = UIFormPos.Normal;
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
