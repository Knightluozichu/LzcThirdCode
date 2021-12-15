using System.Collections.Generic;
using UnityEngine;

/*
 * @author LuoZichu
 * @time 2019/7/1
 */

namespace RedRedJiang.Unity
{
    /// <summary>
    /// 音源管理器
    /// 除BGM和BTN外 每个声音自身带音源和音乐片段 还有key
    /// </summary>
    public class AudioSystem : SystemBase<AudioSystem>, ISystem
    {
        //#region 单例
        //private static AudioSystem _Instance;
        //public static AudioSystem Instance
        //{
        //    get
        //    {
        //        if (_Instance == null)
        //        {
        //            _Instance = Activator.CreateInstance<AudioSystem>();
        //        }

        //        return _Instance;
        //    }
        //}
        //#endregion

        private const string mBGMAudioSourceName = "BGM";
        private const string mBtnAudioSourceName = "BTN";

        /// <summary>
        /// 按钮和bgm 音源
        /// </summary>
        private List<AudioSource> mAllAudioSource;// BGM 0，BTN 1

        public List<AudioSource> AllAudioSource
        {
            get
            {
                if (null == mAllAudioSource)
                {
                    mAllAudioSource = new List<AudioSource>(UISystem.Instance.UICanvas.Find("AudioMgr").GetComponents<AudioSource>());
                }

                return mAllAudioSource;
            }
        }

        private Dictionary<string, AudioSource> mDicNameOfAudioSourceMap;
        public Dictionary<string, AudioSource> DicNameOfAudioSourceMap
        {
            get
            {
                if (null == mDicNameOfAudioSourceMap)
                {
                    mDicNameOfAudioSourceMap = new Dictionary<string, AudioSource>();

                    mDicNameOfAudioSourceMap.Add(mBGMAudioSourceName, AllAudioSource[0]);

                    mDicNameOfAudioSourceMap.Add(mBtnAudioSourceName, AllAudioSource[1]);
                }

                return mDicNameOfAudioSourceMap;
            }
        }

        /// <summary>
        /// 加载音源片段
        /// </summary>
        /// <param name="name">音源片段名字</param>
        /// <returns></returns>
        private AudioClip ResAudioClip(string name)
        {
            return ResourcesSystem.Instance.ResObj<AudioClip>(name, true);
        }

        public AudioSystem()
        {
            mSystemName = CommonClass.mAudioSystemName;
        }

        public void InitAwake()
        {
            m_ValueP = 1;
        }

        /// <summary>
        /// 设置音乐片段
        /// </summary>
        /// <param name="name"></param>
        public void ChangeAudioClip(string asName,string cpName)
        {
            AudioSource ase = InDicAudioSource(asName);
            ase.clip = ResAudioClip(cpName);
            PlayAudioSource(ase);
        }

        private bool HasThisAudioSourceInDic(string asName)
        {
            if(!string.IsNullOrEmpty(asName) && DicNameOfAudioSourceMap.ContainsKey(asName))
            {
                return true;
            }

            return false;
        }

        private AudioSource InDicAudioSource(string asName)
        {
            if(HasThisAudioSourceInDic(asName))
            {
                return DicNameOfAudioSourceMap[asName];
            }

            return null;
        }

        /// <summary>
        /// 音量 总控制
        /// </summary>
        private float m_ValueP = 1;
        public float ValueP { get { return m_ValueP; } set { m_ValueP = value; SetAllVolue(); } }

        private bool mIsMute;

        /// <summary>
        /// 设置是否静音
        /// </summary>
        public bool IsMute { get { return mIsMute; } set { mIsMute = value; IsMuteAllAudioSource(); } }

        /// <summary>
        /// 增加单个音源
        /// </summary>
        /// <param name="asrece"></param>
        public void AddAudioSource(string asName, AudioSource asrece)
        {
            if (!string.IsNullOrEmpty(asName) && asrece != null && !DicNameOfAudioSourceMap.ContainsKey(asName))
            {
                PlayAudioSource(asrece);
                DicNameOfAudioSourceMap.Add(asName, asrece);
            }
        }

        /// <summary>
        /// 移除单个音源
        /// </summary>
        /// <param name="asrece"></param>
        public void RemoveAudioSource(string asName)
        {
            if (asName != null && DicNameOfAudioSourceMap.ContainsKey(asName))
            {
                DicNameOfAudioSourceMap.Remove(asName);
            }
        }

        /// <summary>
        ///设置所有的音量
        /// </summary>
        private void SetAllVolue()
        {
            foreach (var i in DicNameOfAudioSourceMap.Keys)
            {
                SetVolue(DicNameOfAudioSourceMap[i]);
            }
        }

        private void SetVolue(AudioSource asce)
        {
            if (asce != null && asce.clip != null)
            {
                asce.volume = ValueP;
            }
        }

        /// <summary>
        /// 音源 播放
        /// </summary>
        /// <param name="ausre"></param>
        private void PlayAudioSource(AudioSource ausre)
        {
            if (ausre != null && ausre.clip != null && !IsMute)
            {
                IsMuteAudioSource(ausre);
                SetVolue(ausre);
                ausre.Play();
            }
        }

        /// <summary>
        /// 单个音源是否静音
        /// </summary>
        /// <param name="susre"></param>
        private void IsMuteAudioSource(AudioSource susre)
        {
            if (susre != null && susre.clip != null && susre.mute != IsMute)
            {
                susre.mute = IsMute;
            }
        }

        /// <summary>
        /// 所有音源是否静音
        /// </summary>
        public void IsMuteAllAudioSource()
        {
            foreach(var i in DicNameOfAudioSourceMap.Keys)
            {
                IsMuteAudioSource(DicNameOfAudioSourceMap[i]);
            }
        }

        /// <summary>
        /// 暂停播放单个音源
        /// </summary>
        /// <param name="susre"></param>
        private void StopAudioSource(AudioSource susre)
        {
            if (susre != null && susre.clip != null && susre.isPlaying)
            {
                susre.Pause();
            }
        }

        /// <summary>
        /// 停止所有音源的播放
        /// </summary>
        public void StopAllAudioSource()
        {
            foreach (var i in DicNameOfAudioSourceMap.Keys)
            {
                StopAudioSource(DicNameOfAudioSourceMap[i]);
            }
        }

        /// <summary>
        /// 恢复所有音源的播放
        /// </summary>
        public void PlayAllAudioSource()
        {
            foreach (var i in DicNameOfAudioSourceMap.Keys)
            {
                PlayAudioSource(DicNameOfAudioSourceMap[i]);
            }
        }

    }
}
