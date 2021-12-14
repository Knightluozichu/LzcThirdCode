using System.Collections.Generic;
using UnityEngine;

/*
 * @author LuoZichu
 * @time 2019/7/1
 */

namespace RedRedJiang.Unity
{
    public class GameBase :IBaseNotify
    {
        #region Excute

        public  void Excute(int _evenMa, object _Message = null)
        {
            if (DicEventDelegate.Count > 0)
            {
                if (DicEventDelegate.ContainsKey(_evenMa))
                {
                    DicEventDelegate[_evenMa](_Message);
                }
                else
                {
                    Debug.Log("mEventDelegate 不包含" + _evenMa + "这个事件码。");
                }
            }
            else
            {
                Debug.Log("mEventDelegate 长度为 0");
            }
        }

        #endregion

        #region information
        //缓存自身的时间集合
        private List<int> mListEventMa = new List<int>();

        public void Register(params int[] _EventMa)
        {
            mListEventMa.AddRange(_EventMa);
            GameSystem.Instance.Add(this, mListEventMa.ToArray());
        }

        public void Cancel()
        {
/*            Debug.Log("Cancel this - >" + this);*/
            GameSystem.Instance.Remove(this, mListEventMa.ToArray());
            mListEventMa.Clear();
        }

        public void Cancel(params int[] _EventMa)
        {
            for (int i = 0; i < _EventMa.Length; i++)
            {
                GameSystem.Instance.Remove(this, _EventMa[i]);
                mListEventMa.Remove(_EventMa[i]);
            }
        }

        public void SendMsg(string _Area,int _EventMa, object _Message)
        {
            GameFacade.Instance.SendMsg(_Area, _EventMa, _Message);
        }

        #endregion
        protected AudioSource mAudioSourceCharacter;

        protected CharacterProperty mCProperty;
        public CharacterProperty CProperty
        {
            get
            {
                return mCProperty;
            }
        }
        private Dictionary<int, DelExtueHandle> mDicEventDelegate = new Dictionary<int, DelExtueHandle>();
        public Dictionary<int, DelExtueHandle> DicEventDelegate { get => mDicEventDelegate; set => mDicEventDelegate = value; }


        public GameBase(CharacterProperty mCProperty = null)
        {
            this.mCProperty = mCProperty;
        }
    }

    /// <summary>
    /// 基础属性
    /// </summary>
    public class CharacterProperty
    {
        private int mBlood;//血量
        private int mAttackATK;//攻击
        private int mMoveSpeed;//移动速度
        private float mAttackSpeed;//攻击速度
        private CharacterType mCType;//类型
        private UnityEngine.GameObject mGameObj;//自身物体
        private Transform mBirthTrans;

        public int Blood { get { return mBlood; } set { mBlood = value; } }
        public int AttackATK { get { return mAttackATK; }set { mAttackATK = value; } }
        public int MoveSpeed { get { return mMoveSpeed; } set { mMoveSpeed = value; } }
        public float AttackSpeed { get { return mAttackSpeed; } set { mAttackSpeed = value; } }
        public CharacterType CType { get { return mCType; }set { mCType = value; } }
        public GameObject GameObj { get { return mGameObj; }set { mGameObj = value; } }
        public Transform BirthTrans { get { return mBirthTrans; }set { mBirthTrans = value; } }
    }

    /// <summary>
    /// 角色类型
    /// </summary>
    public enum CharacterType { Male,Female,Monster_1, Monster_2, Monster_3, Monster_4, Monster_5 }
}
