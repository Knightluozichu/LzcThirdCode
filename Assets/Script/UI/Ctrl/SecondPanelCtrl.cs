namespace RedRedJiang.Unity
{
    public class SecondPanelCtrl : UICtrlBase, IUICtrlBase
    {
        public SecondPanelCtrl(UIBase mUIBaseRef, UIDataBase mUIDataBaseRef) : base(mUIBaseRef, mUIDataBaseRef)
        {
        }

        public void UICtrlEnd()
        {
            CtrlEnd();
        }

        public void UICtrlUpdate()
        {
        }

        public void UICtrtlInit()
        {
            if (mUIBaseRef is SecondPanelView)
            {
                SecondPanelView sp = mUIBaseRef as SecondPanelView;
                sp.Btn_Switch.onClick.AddListener(() => { sp.Open(CommonClass.mStartPanelName); sp.Close(); });
            }
        }
    }
}
