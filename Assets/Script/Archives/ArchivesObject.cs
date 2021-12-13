using System;
using System.IO;
using System.Text;
using UnityEngine;

/*
 * @author LuoZichu
 * @time 2019/7/1
 */

namespace Assets.Script.Archives
{
    public class ArchivesObject
    {
        //private const string mBytes = ".byte";
        private const string mWindowsSpritPath = "/";
        private string mPersistentDataPath = Application.persistentDataPath;

        /// <summary>
        /// 存档路径
        /// </summary>
        private StringBuilder mArchivePath;
        public StringBuilder ArchivePath
        {
            get
            {
                if (null == mArchivePath)
                {
                    mArchivePath = new StringBuilder(30);
                }
                return mArchivePath;
            }
        }
        public ArchivesObject(string mArchivePath)
        {
            ArchivePath.Append(mPersistentDataPath);
            ArchivePath.Append(mWindowsSpritPath);
            ArchivePath.Append(mArchivePath);
            //ArchivePath.Append(mBytes);
        }
        public ArchivesObject(string mArchivePath,string mPersistentDataPat)
        {
            this.mPersistentDataPath = mPersistentDataPat;
            ArchivePath.Append(mPersistentDataPath);
            ArchivePath.Append(mWindowsSpritPath);
            ArchivePath.Append(mArchivePath);
            //ArchivePath.Append(mBytes);
        }

        /// <summary>
        /// 检查路径
        /// </summary>
        /// <returns></returns>
        public bool CheckPath()
        {
            if (ArchivePath.Length == 0 || !File.Exists(mArchivePath.ToString()))
            {
                return false;
            }
            return true;
        }

        public override string ToString()
        {
            return ArchivePath.ToString();
        }
    }
}
