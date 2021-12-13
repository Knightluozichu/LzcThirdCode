

using Assets.Script.Notify;
using System;

/*
 * @author LuoZichu
 * @time 2019/7/1
 */

namespace Assets.Script.Single
{
    public class Singleton<T> where T : Singleton<T>
    {
        private static T _Instance;
        public static T Instance
        {
            get
            {
                if(_Instance == null)
                {
                    _Instance = Activator.CreateInstance<T>();
                }
                return _Instance;
            }
        }
    }
}
