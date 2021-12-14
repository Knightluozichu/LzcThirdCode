using UnityEngine;

/*
 * @author LuoZichu
 * @time 2019/7/1
 */

namespace RedRedJiang.Unity
{
    public class MonsterBase : GameBase
    {

        #region Field
        protected MonsterFSMSystem mMonsterFsm;//自身属性
        protected int mExperienceFall;//携带经验
        protected Color mColor;
        protected bool mIsChooised;
        protected Renderer mGobRenderer;
        private const float constX = 0.4f;
        private const float constXend = 0.45f;
        private float startX;
        private float mBloodStart;
        private SpriteRenderer mWarringTrans;
        private float mAttackSpeed;
        private Transform mGob_Blood;
        public Transform Gob_Blood { get { return mGob_Blood = mGob_Blood ?? UISystem.Instance.UICanvas.Find(@"Normal/GamePanel/Gob_Blood"); } }
        public float AttackSpeed { get { return mAttackSpeed; } protected set { mAttackSpeed = value; mCProperty.AttackSpeed = mAttackSpeed; } }
        public SpriteRenderer WarringTrans
        {
            get
            {
                if (mWarringTrans == null)
                {
                    mWarringTrans = mCProperty.GameObj.transform.Find("Warring").GetComponent<SpriteRenderer>();
                }
                /*mWarringTrans.color = new Color(1, 1, 1, 0);*/
                return mWarringTrans;
            }
        }
        protected Renderer GobRenderer { get { return mGobRenderer = mGobRenderer ?? mCProperty.GameObj.transform.Find("Blood").GetComponent<Renderer>(); } }
    
        protected bool mIsDead;
        public bool IsDead { get { return mIsDead; } set { mIsDead = value; } }
        public int ExperienceFall { get { return mExperienceFall; } }

        private UnityEngine.UI.Text mTxt_Blood;
        private UnityEngine.UI.Text mTxt_BloodConst;
        public UnityEngine.UI.Text Txt_Blood { get { return mTxt_Blood = mTxt_Blood ?? UnityTool.CreateGameObject("Txt_Blood", Gob_Blood).GetComponent<UnityEngine.UI.Text>(); } }
        public UnityEngine.UI.Text Txt_BloodConst { get { return mTxt_BloodConst = mTxt_BloodConst ?? UITool.FindChild<UnityEngine.UI.Text>(Txt_Blood.gameObject, "Txt_Const"); } }
        public AudioSource AudioSourceCharacter { get { return mAudioSourceCharacter = mAudioSourceCharacter ?? mCProperty.GameObj.GetComponent<AudioSource>(); } }
        #endregion

        #region Method
        #region Public
        public MonsterBase(CharacterProperty mCProperty, int mExperienceFall) : base(mCProperty)
        {
            this.mExperienceFall = mExperienceFall;
            mCProperty.GameObj.transform.SetParent(mCProperty.BirthTrans);
            mCProperty.GameObj.transform.localScale = UnityEngine.Vector3.one;
            mCProperty.GameObj.transform.localRotation = UnityEngine.Quaternion.identity;
            mCProperty.GameObj.transform.localPosition = UnityEngine.Vector3.zero;

            mBloodStart = mCProperty.Blood;
            startX = 0.05f;


            //Debug.Log(mBloodStart);
        }

        private void StartBlood(object msg)
        {
            Txt_Blood.rectTransform.position = RectTransformUtility.WorldToScreenPoint(Camera.main, mCProperty.GameObj.transform.Find("Blood").position);
            Txt_Blood.GetComponent<RectTransform>().anchoredPosition = new Vector2(Txt_Blood.GetComponent<RectTransform>().anchoredPosition.x + 5, Txt_Blood.GetComponent<RectTransform>().anchoredPosition.y - 8);
            Txt_Blood.text = Txt_BloodConst.text = mBloodStart.ToString();
        }

        public void MonsterInit()
        {

        }


        public void MonsterChangeFSM(StateTransition id)
        {

            mMonsterFsm.PerformTransition(id);

        }

        public void MonsterEnd()
        {

            mMonsterFsm.DeleteState(StateID.Attack);
            mMonsterFsm.DeleteState(StateID.Death);
            mMonsterFsm.DeleteState(StateID.Run);
            mMonsterFsm.DeleteState(StateID.Stand);


            Cancel();
            DicEventDelegate.Clear();
            DicEventDelegate = null;
        }






        #endregion
        #endregion
    }
}
