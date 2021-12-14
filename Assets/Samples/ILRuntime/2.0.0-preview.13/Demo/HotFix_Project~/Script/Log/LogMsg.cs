using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

/*
 * @author LuoZichu
 * @time 2019/7/1
 */

namespace RedRedJiang.Unity
{
    public class LogMsg
    {
        private static ILogger logger;

       public static void Init(ILogger _ILogger)
        {
            logger = _ILogger;
            Application.logMessageReceived += logger.Log;
        }

        public static void UnLog()
        {
            Application.logMessageReceived -= logger.Log;
        }


    }
}
