using Assets.Script.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Script.UI.Ctrl
{
    public class UICtrlSystem : SystemBase
    {
        #region 单例
        private static UISystem _Instance;
        public static UISystem Instance
        {
            get
            {
                if (_Instance == null)
                {
                    _Instance = Activator.CreateInstance<UISystem>();
                }

                return _Instance;
            }
        }
        #endregion
    }
}
