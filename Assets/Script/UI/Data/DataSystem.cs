using Assets.Script.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Script.UI.Data
{
    public class DataSystem : SystemBase
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
