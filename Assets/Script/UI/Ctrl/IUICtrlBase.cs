using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Script.UI.Ctrl
{
    public interface IUICtrlBase
    {
        void UICtrtlInit();
        void UICtrlUpdate();
        void UICtrlEnd();
    }
}
