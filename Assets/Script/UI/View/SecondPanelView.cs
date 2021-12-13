using UnityEngine.UI;

namespace RedRedJiang.Unity
{
    public class SecondPanelView : UIBase
    {
        public SecondPanelView() : base(CommonClass.mSecondPanelName)
        {

        }

        private Button mBtn_Switch;
        public Button Btn_Switch
        {
            get
            {
                return mBtn_Switch = mBtn_Switch ?? UITool.FindChild<Button>(GojPanel, "Button");
            }
        }

        public override void UIInit()
        {
            base.UIInit();
            mUIDataBaseRef = new SecondPanelData();
            mUICtrtlBaseRef = new SecondPanelCtrl(this, mUIDataBaseRef);
            mUICtrtlBaseRef.UICtrtlInit();
        }

        public override void UIEnd()
        {
            base.UIEnd();
            mUICtrtlBaseRef.UICtrlEnd();
            mBtn_Switch = null;
        }


    }
}
