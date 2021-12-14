using System;
using System.Collections.Generic;


namespace RedRedJiang.Unity
{
    public class ArchivesSystem : Singleton<ArchivesSystem>
    {
        private Dictionary<string, ArchivesObject> mDicArchivesOfNameMap;
        public Dictionary<string, ArchivesObject> DicArchivesOfNameMap
        {
            get
            {
                if (null == mDicArchivesOfNameMap)
                {
                    mDicArchivesOfNameMap = new Dictionary<string, ArchivesObject>();
                }
                return mDicArchivesOfNameMap;
            }
        }

        public void AddArchives(string _Name, ArchivesObject ao)
        {
            if (string.IsNullOrEmpty(_Name) || null == ao || DicArchivesOfNameMap.ContainsKey(_Name))
            {
                return;
            }

            DicArchivesOfNameMap.Add(_Name, ao);
        }

        public ArchivesObject SubArchives(string _Name)
        {
            ArchivesObject ao = null;

            if (string.IsNullOrEmpty(_Name) || !DicArchivesOfNameMap.TryGetValue(_Name, out ao))
            {
                return null;
            }

            return ao;
        }

        private bool IsArchives(string _Name, out ArchivesObject arcObj)
        {
            arcObj = null;

            if (string.IsNullOrEmpty(_Name) || !DicArchivesOfNameMap.TryGetValue(_Name, out arcObj))
            {
                return false;
            }

            return true;
        }

        public string GetArchivesPath(string _Name)
        {
            ArchivesObject arcObj;

            if (!IsArchives(_Name, out arcObj))
            {
                return null;
            }

            return arcObj.ToString();
        }
    }
}
