using System.Collections.Generic;
using UnityEngine;

/*
 * @author LuoZichu
 * @time 2019/7/1
 */

namespace RedRedJiang.Unity
{
    public class PoolManager : Singleton<PoolManager>
    {
        /// <summary>
        /// 主的池子
        /// </summary>
        private Dictionary<string, SubPool> mDicHostPool = new Dictionary<string, SubPool>();

        /// <summary>
        /// 注册池子
        /// </summary>
        /// <param name="_Name"></param>
        private void RegisterSubPool(string _Name)
        {
            SubPool subPool = new SubPool();
            subPool.PoolName = _Name;
            subPool.InitPool();
            mDicHostPool.Add(_Name, subPool);
        }

        /// <summary>
        /// 从池子取对象
        /// </summary>
        /// <param name="_Name"></param>
        /// <returns></returns>
        public GameObject Spawn(string _Name)
        {
            if (!mDicHostPool.ContainsKey(_Name))
            {
                RegisterSubPool(_Name);
            }

            SubPool subPool = mDicHostPool[_Name];

            return subPool.Spawn();
        }

        /// <summary>
        /// 把对象放回池子里
        /// </summary>
        public void UnSpwan(string _Name, GameObject _Gob)
        {
            if (string.IsNullOrEmpty(_Name) || null == _Gob) { return; }

            SubPool subpool = mDicHostPool[_Name];

            if (null == subpool) { return; }

            if (subpool.ObjectsInSubPool(_Gob))
            {
                subpool.UnSpawn(_Gob);
            }

        }

        /// <summary>
        /// 把所有的池子复原之初始化
        /// </summary>
        public void UnSpwanAllObject()
        {
            foreach (var i in mDicHostPool)
            {
                i.Value.UnSpwanAllObject();
            }

            mDicHostPool.Clear();
        }
    }
}
