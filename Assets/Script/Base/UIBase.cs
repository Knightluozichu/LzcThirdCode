using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
 * @author LuoZichu
 * @time 2019/7/1
 */

namespace RedRedJiang.Unity
{
    public class UIBase : IBaseNotify
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
            UISystem.Instance.Add(this, mListEventMa.ToArray());
        }

        public void Cancel()
        {
            UISystem.Instance.Remove(this, mListEventMa.ToArray());
            mListEventMa.Clear();
        }

        public void Cancel(params int[] _EventMa)
        {
            for (int i = 0; i < _EventMa.Length; i++)
            {
                UISystem.Instance.Remove(this, _EventMa[i]);
                mListEventMa.Remove(_EventMa[i]);
            }
        }

        public void SendMsg(string _Area, int _EventMa, object _Message = null)
        {
            GameFacade.Instance.SendMsg(_Area, _EventMa, _Message);
        }

        #endregion

        public UIBase(string mUIBaseFromName)
        {
            this.mUIBaseFromName = mUIBaseFromName;
        }

        protected IUICtrlBase mUICtrtlBaseRef;
        protected UIDataBase mUIDataBaseRef;
        #region UI

        protected string mUIBaseFromName;
        public string UIBaseFromName { get { return mUIBaseFromName;} }

        protected UIFormPos mUIForm_Pos;
        public UIFormPos UIForm_Pos { get { return mUIForm_Pos; } }

        private GameObject mGojPanel;
        public GameObject GojPanel { get { return mGojPanel; } set { mGojPanel = value; } }

        private Dictionary<int, DelExtueHandle> mDicEventDelegate = new Dictionary<int, DelExtueHandle>();
        public Dictionary<int, DelExtueHandle> DicEventDelegate { get => mDicEventDelegate; set => mDicEventDelegate = value; }


        public void ShowUIForm(string name,GameObject Gob = null)
        {
            switch(UIForm_Pos)
            {
                case UIFormPos.Normal:
                    NormalShowUIFrom(name);
                    break;
                case UIFormPos.Reversal:
                    ReversalShowUIFrom();
                    break;
                case UIFormPos.Pop:
                    break;
            }

            GojPanel.SetActive(true);
        }

        private void NormalShowUIFrom(string name)
        {
            BgDefaultSystem.Instance.Open_Bg_Trans_Default(name + "Bg");
        }

        private void ReversalShowUIFrom()
        {
            //初始一下状态
            GojPanel.transform.localScale = Vector3.zero;

            SendMsg(
                CommonClass.mAnimationSystemName,
                (int)AnimationEventMa.Make_Aniamtion_Smple,
                new AnimationInfo(0.2f, GojPanel.transform, AnimationXState.ScaleTo,
                new AnimationFloat(0, 1.2f),
             
                p =>{
                     SendMsg(CommonClass.mAnimationSystemName,
                     (int)AnimationEventMa.Make_Aniamtion_Smple,
                     new AnimationInfo(0.2f, GojPanel.transform, AnimationXState.ScaleTo,
                     new AnimationFloat(1.2f, 1f),m =>  CallBackOfInit()));
                 }
             ));
        }

        private void ReversalCloseUIFrom()
        {
            SendMsg(CommonClass.mAnimationSystemName, (int)AnimationEventMa.Make_Aniamtion_Smple,
                new AnimationInfo(0.2f, GojPanel.transform, AnimationXState.ScaleTo,
                new AnimationFloat(1,1.2f), p =>
                {
                    SendMsg(CommonClass.mAnimationSystemName, (int)AnimationEventMa.Make_Aniamtion_Smple,
                new AnimationInfo(0.2f, GojPanel.transform, AnimationXState.ScaleTo,
                new AnimationFloat(1.2f, 0), x => { CallBackOfEnd(); UnityEngine.Object.Destroy(GojPanel); UIEnd(); }));
                }));
        }

        public void CloseUIFrom(GameObject gob = null)
        {
            switch (UIForm_Pos)
            {
                case UIFormPos.Normal:
                    UnityEngine.Object.Destroy(GojPanel);
                    //mGojPanel = null;
                    UIEnd();
                    break;
                case UIFormPos.Reversal:
                    ReversalCloseUIFrom();
                    break;
                case UIFormPos.Pop:
                    UnityEngine.Object.Destroy(GojPanel);
                    //mGojPanel = null;
                    UIEnd();
                    break;
            }
        }

        public void Close(string _UIName = null)
        {
            UISystem.Instance.CloseUIFrom(_UIName == null ? mGojPanel.name: _UIName);
        }

        public UIBase Open(string name)
        {
            return UISystem.Instance.OpenUIFrom(name);
        }

        public void AgainAnimationRotaSnap(float time, float org, float end, float againCount, AnimationXState axs, Transform trans,Action callback = null)
        {

            SendMsg(CommonClass.mAnimationSystemName, (int)AnimationEventMa.Make_Aniamtion_Smple, new AnimationInfo(time, trans, axs, new AnimationFloat(org, end), (p) =>
            {
                againCount--;

                while (againCount <= 0) { if (callback != null) callback(); return; }

                 org /= 2f; end /= 2f;

/*                Debug.Log("AgainAnimationRotaSnap--->" + againCount);*/
                AgainAnimationRotaSnap(time, end, org, againCount, axs, trans, callback);
            }));
        }

        protected void ScaleTo(float time, Transform trans,float start,float end,Action callback = null)
        {
            SendMsg(CommonClass.mAnimationSystemName, (int)AnimationEventMa.Make_Aniamtion_Smple, new AnimationInfo(time, trans, AnimationXState.ScaleTo, new AnimationFloat(start, end),x => { if(callback != null) callback(); }));
        }

        protected void HiddenToImg(float time, Transform trans, float start, float end, Action callback = null)
        {
            SendMsg(CommonClass.mAnimationSystemName, (int)AnimationEventMa.Make_Aniamtion_Smple, new AnimationInfo(time, trans.GetComponent<Image>(), AnimationXState.HiddenToImg, new AnimationFloat(start, end), x => { if (callback != null) callback(); }));
        }

        protected void WRectToImg(float time, RectTransform image, float start, float end, Action callback = null)
        {
            SendMsg(CommonClass.mAnimationSystemName, (int)AnimationEventMa.Make_Aniamtion_Smple, new AnimationInfo(time, image, AnimationXState.WRectTo, new AnimationFloat(start, end), x => { if (callback != null) callback(); }));
        }

        /// <summary>
        /// 延时调用
        /// </summary>
        /// <param name="time"></param>
        /// <param name="callback"></param>
        public void Invoke(float time ,Action callback = null)
        {
            SendMsg(CommonClass.mAnimationSystemName, (int)AnimationEventMa.Make_Aniamtion_Smple, new InvokeTimeInfo(time, x => callback()));
        }

       //private bool mIsAwake = false;
       /// <summary>
       ///  预加载数据
       /// </summary>
        public virtual void UIInit()
        {
            DicEventDelegate = new Dictionary<int, DelExtueHandle>();
        }

        /// <summary>
        /// 初始化
        /// </summary>
        public virtual void CallBackOfInit(){}
        public virtual void CallBackOfEnd(Action callBack = null) { }

        /// <summary>
        /// 刷新
        /// </summary>
        public virtual void UIpdate() { }

        /// <summary>
        /// 结束
        /// </summary>
        public virtual void UIEnd()
        {
            Cancel();
            DicEventDelegate.Clear();
            DicEventDelegate = null;
        }

        /// <summary>
        /// 延时打开界面
        /// </summary>
        /// <param name="uibase"></param>
        protected void InvokeOpenUIFrom(string uibase)
        {
            Invoke(0.2f, () =>
            {
                Open(uibase);
            });
        }

        protected void InvokeCLoseUIFrom(string _PanelName)
        {
          
            Invoke(0.2f, () =>
            {
                Close(_PanelName);
            });
        }

        /// <summary>
        /// 请求面板数据
        /// </summary>
        /// <param name="_PanelData"></param>
        /// <param name="_SubEventMa"></param>
        protected void RequiredData(ModelEventMa _SubEventMa,object msg = null)
        {
            SendMsg(CommonClass.mModelSystemName, (int)_SubEventMa,msg);
        }

        /// <summary>
        /// 处理数据回调
        /// </summary>
        /// <param name="_SubEventMa">数据事件</param>
        /// <param name="obejctFunction">事件</param>
        protected void ExctureModelEvent(ModelEventMa _SubEventMa,DelExtueHandle obejctFunction)
        {
            DicEventDelegate.Add((int)_SubEventMa, obejctFunction);
        }

        /// <summary>
        /// 向Model更新数据
        /// </summary>
        /// <param name="_SubEventMa"></param>
        /// <param name="msg"></param>
        protected void UpdateModelData(ModelEventMa _SubEventMa,object msg)
        {
            SendMsg(CommonClass.mModelSystemName, (int)_SubEventMa, msg);
        }

        /// <summary>
        /// 请求资源引用
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="assetsName"></param>
        /// <returns></returns>
        protected T ResAssets<T>(string assetsName) where T:UnityEngine.Object
        {
            return ResourcesSystem.Instance.ResObj<T>(assetsName, true);
        }

        /// <summary>
        /// 加载图片
        /// </summary>
        /// <param name="_Name"></param>
        /// <returns></returns>
        public Sprite CreateSprite(string _Name)
        {
            Texture2D t2d = null;
            t2d = ResAssets<Texture2D>(_Name);
            return Sprite.Create(t2d, new Rect(0, 0, t2d.width, t2d.height), Vector2.zero);
        }

        public void FacdeUI(float time,Image img,float org,float end,Action callBack=null)
        {
            SendMsg(CommonClass.mAnimationSystemName, (int)AnimationEventMa.Make_Aniamtion_Smple,
                new AnimationInfo(time, img, AnimationXState.BarTo, new AnimationFloat(org, end), (x) => { callBack(); }));
        }

        public void UICtrlInit()
        {
            mUICtrtlBaseRef.UICtrtlInit();
        }

        public void UICtrlEnd()
        {
            mUICtrtlBaseRef.UICtrlEnd();
        }


        #endregion

    }
}
