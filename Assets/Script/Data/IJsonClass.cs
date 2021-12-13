using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

/*
 * @author LuoZichu
 * @time 2019/7/1
 */

namespace Assets.Script.Data
{
    public abstract class IJsonClass
    {
        protected string JsonDataPath()
        {
            return Application.persistentDataPath; 
        }

        public abstract void SaveJson<T>(T _Class, string _PathName) where T : class, new();
        public abstract T LoadJson<T>(string _PathName) where T : class , new();
    }
}
