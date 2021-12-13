namespace RedRedJiang.Unity
{
    public class StartPanelCtrl : UICtrlBase, IUICtrlBase
    {
        public StartPanelCtrl(UIBase mUIBaseRef, UIDataBase mUIDataBaseRef) : base(mUIBaseRef, mUIDataBaseRef)
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
            if (mUIBaseRef is StartPanelView)
            {
                StartPanelView sp = mUIBaseRef as StartPanelView;
                sp.ButtonOnClick.onClick.AddListener(() => { sp.Open(CommonClass.mSecondPanelName); sp.Close(); });
            }
        }
    }
}
