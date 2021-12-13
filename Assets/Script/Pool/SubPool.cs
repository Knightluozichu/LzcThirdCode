using System.Collections.Generic;
using UnityEngine;
using Assets.Script.Resources;

/*
 * @author LuoZichu
 * @time 2019/7/1
 */

namespace Assets.Script.Pool
{
    public class SubPool
    {
        #region Field
        private List<GameObject> mListGob = new List<GameObject>();//池子

        //private GameObject mGob;
        private const int mInitListGobSize = 10;//初始容量
        private const float mLoadFactor = 0.8f;//固定负载因子
        private const float mPlaceFactor = 0.6f;//固定放置因子
        private const int mRegulationCapacity = 10;//增减容量值

        private int mCurrentListGobSize;//当前容量
        private float mCurrentFactor;//当前负载因子
        private int mCurrentUserCount;//当前使用情况数量

        #endregion

        #region Property
        public string PoolName { get; set; }

        #endregion

        #region Public Method
        /// <summary>
        /// 预加载
        /// </summary>
        public void InitPool()
        {
            if(PoolName == null)
            {
                Debug.Log("PoolName 先赋值！");
                return;
            }

            //Dilatation(mInitListGobSize);
        }

        /// <summary>
        /// 从池中取对象
        /// </summary>
        /// <returns></returns>
        public GameObject Spawn()
        {
            GameObject gob = null;
            for(int i = 0; i< mListGob.Count;i++)
            {
                if(mListGob[i].activeSelf == false)
                {
                    mCurrentUserCount--;
/*                    Debug.Log(mCurrentUserCount);*/
                    gob = mListGob[i];
                    gob.SetActive(true);

                    if (JudgeFactor())
                    {
                        Dilatation(mListGob.Count + mRegulationCapacity);
                    }
                    return gob;
                }
            }
            return null;
        }

        /// <summary>
        /// 把对象放回池子
        /// </summary>
        /// <param name="_gob">对象</param>
        public void UnSpawn(GameObject _gob)
        {
            if(_gob == null || !mListGob.Contains(_gob))
            {
                Debug.Log(_gob + "check it!");
                return;
            }

            for(int i = 0;i< mListGob.Count;i++)
            {
                if(mListGob[i].activeSelf && _gob == mListGob[i])
                {
                    mListGob[i].transform.localPosition = Vector3.zero;
                    mListGob[i].transform.localRotation = Quaternion.identity;
                    mListGob[i].transform.localScale = Vector3.one;
                    mListGob[i].SetActive(false);
                    mCurrentUserCount++;

                    if(IsReducedCapacity())
                    {
                        ReducedCapacity(mRegulationCapacity);
                    }
                    return;
                }
            }
        }

        /// <summary>
        /// 判断对象是否在池子里
        /// </summary>
        /// <param name="_Gob">对象</param>
        /// <returns></returns>
        public bool ObjectsInSubPool(GameObject _Gob)
        {
            return mListGob.Contains(_Gob);
        }

        /// <summary>
        /// 复原池子
        /// </summary>
        public void UnSpwanAllObject()
        {
            foreach(var i in mListGob)
            {
               if(i.activeSelf)
                {
                    mCurrentUserCount++;
                    i.SetActive(false);
                    UnSpawn(i);
                }
            }

            ReducedCapacity(mRegulationCapacity);
        }

        #endregion

        #region Private Method

        /// <summary>
        /// 是否需要扩容
        /// </summary>
        /// <returns></returns>
        private bool JudgeFactor()
        {
/*            Debug.Log((mCurrentListGobSize - mCurrentUserCount) * 1f / mCurrentListGobSize + "****" + mLoadFactor);*/
            if((mCurrentListGobSize - mCurrentUserCount) * 1f / mCurrentListGobSize >= mLoadFactor)
            {
                return true;
            }
            return false; 
        }

        /// <summary>
        /// 是否需要降容
        /// </summary>
        /// <returns></returns>
        private bool IsReducedCapacity()
        {

            if(mCurrentListGobSize <= mInitListGobSize) { return false; }

            if (mCurrentUserCount * 1f / mCurrentListGobSize >= mLoadFactor)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 扩容
        /// </summary>
        private void Dilatation(int _Number)
        {
            GameObject gob = null;
            while (mListGob.Count <= _Number)
            {
                gob = Object.Instantiate(ResourcesSystem.Instance.ResObj<GameObject>(PoolName, true)) as GameObject;
                gob.SetActive(false);
                mListGob.Add(gob);
                mCurrentUserCount++;
            }

            mCurrentListGobSize = mInitListGobSize;
        }

        /// <summary>
        /// 降容
        /// </summary>
        /// <param name="_Count"></param>
        private void ReducedCapacity(int _Count)
        {
            int counter = 0;
            for(int i = 0; i < mListGob.Count; i++)
            {
                if (!mListGob[i].activeSelf)
                {
                    Object.Destroy(mListGob[i]);
                    mListGob.RemoveAt(i);
                    counter++;
                }

                if(counter == _Count)
                {
                    return;
                }
            }
        }
        #endregion

    }
}
