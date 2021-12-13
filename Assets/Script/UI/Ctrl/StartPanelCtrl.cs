using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Script.Base;
using Assets.Script.UI.Data;

namespace Assets.Script.UI.Ctrl
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
                sp.ButtonOnClick.onClick.AddListener(() => { sp.Open(Common.CommonClass.mSecondPanelName); sp.Close(); });
            }
        }
    }
}
