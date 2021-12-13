using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
 * @author LuoZichu
 * @time 2019/7/1
 */

namespace RedRedJiang.Unity
{
    public class ExcuteAnimation : AnimationBase
    {

        #region #Field

        #region Private

        private List<AnimationInfo> mListAnimationInfo = new List<AnimationInfo>();
        private List<InvokeTimeInfo> mListInvokeTimeInfo = new List<InvokeTimeInfo>();

        #endregion

        #region Public

        #endregion

        #endregion

        private FingerEffect mFingerEffect;

        #region Method

        #region Public

        public ExcuteAnimation()
        {
            mFingerEffect = new FingerEffect();
            mFingerEffect.IsOnFingerEffect = true;
        }

        public void AnimationInit()
        {
            Register((int)AnimationEventMa.Make_Aniamtion_Smple);
            DicEventDelegate.Add((int)AnimationEventMa.Make_Aniamtion_Smple, AddAnimation);
        }

        public void AnimationUpdate()
        {
            mFingerEffect.Update();

            if (mListAnimationInfo != null && mListAnimationInfo.Count > 0)
            {
                for (int i = 0; i < mListAnimationInfo.Count; i++)
                {
                    switch (mListAnimationInfo[i].CurrentState)
                    {
                        case AnimationXState.TransTo:
                            TransTo(mListAnimationInfo[i]);
                            break;
                        case AnimationXState.ShadeHiddenToSpriteRender:
                            ShadeHiddenToSpriteRender(mListAnimationInfo[i]);
                            break;
                        case AnimationXState.TransSpriteTo:
                            TransSpriteTo(mListAnimationInfo[i]);
                            break;
                        case AnimationXState.ShadeHiddenToText://
                            ShadeHiddenToText(mListAnimationInfo[i]);
                            break;
                        case AnimationXState.BarTo:
                            BarToValue(mListAnimationInfo[i]);
                            break;
                        case AnimationXState.RateToZ:
                            RateToZ(mListAnimationInfo[i]);
                            break;
                        case AnimationXState.ScaleTo:
                            ScaleTo(mListAnimationInfo[i]);
                            break;
                        case AnimationXState.HiddenToImg:
                            HiddenToImg(mListAnimationInfo[i]);
                            break;
                        case AnimationXState.RendererMaterialFloatTo:
                            RendererMaterialFloatTo(mListAnimationInfo[i]);
                            break;
                        case AnimationXState.WRectTo:
                            WRectTo(mListAnimationInfo[i]);
                            break;
                        default:
                            Debug.Log("不存在的参数！");
                            break;
                    }
                }
            }

            if (mListInvokeTimeInfo.Count > 0)
            {
                for (int i = 0; i < mListInvokeTimeInfo.Count; i++)
                {
                    InvokeDelay(mListInvokeTimeInfo[i]);
                }
            }
        }

        public void AnimationEnd()
        {
            mListAnimationInfo.Clear();
            mListInvokeTimeInfo.Clear();
            mListInvokeTimeInfo = null;
            mListAnimationInfo = null;
        }
        #endregion

        #region Private

        private void RendererMaterialFloatTo(AnimationInfo _AnimationInfo)
        {
            Renderer trans = _AnimationInfo.ObjectX as Renderer;

            AnimationFloat af = _AnimationInfo.AnimationPro as AnimationFloat;

            if (!_AnimationInfo.IsFinish && trans != null)
            {
                _AnimationInfo.EndTime -= Time.deltaTime;

                if (_AnimationInfo.EndTime <= 0)
                {
                    _AnimationInfo.SetFinish();
                }

                trans.material.mainTextureOffset = Vector3.Lerp(new Vector2(af.EndFloat, 0), new Vector2(af.OriginFloat, 0), _AnimationInfo.EndTime * 1f / _AnimationInfo.StartTime);
            }
            else
            {
                _AnimationInfo.CallBack();

                if (_AnimationInfo.IsActionFinish)
                {
                    mListAnimationInfo.Remove(_AnimationInfo);
                    return;
                }
            }
        }
        private void TransTo(AnimationInfo _AnimationInfo)
        {
            Transform trans = _AnimationInfo.ObjectX as Transform;

            AnimationVec3 af = _AnimationInfo.AnimationPro as AnimationVec3;

            if (!_AnimationInfo.IsFinish && trans)
            {
                _AnimationInfo.EndTime -= Time.deltaTime;

                if (_AnimationInfo.EndTime <= 0)
                {
                    _AnimationInfo.SetFinish();
                }

                trans.position = Vector3.Lerp(af.EndVec3, af.OrignVec3, _AnimationInfo.EndTime * 1f / _AnimationInfo.StartTime);
            }
            else
            {
                _AnimationInfo.CallBack();

                if (_AnimationInfo.IsActionFinish)
                {
                    mListAnimationInfo.Remove(_AnimationInfo);
                    return;
                }
            }
        }
        private void ScaleTo(AnimationInfo _AnimationInfo)
        {
            Transform trans = _AnimationInfo.ObjectX as Transform;

            AnimationFloat af = _AnimationInfo.AnimationPro as AnimationFloat;

            if (!_AnimationInfo.IsFinish && trans != null)
            {
                _AnimationInfo.EndTime -= Time.deltaTime;

                if (_AnimationInfo.EndTime <= 0)
                {
                    _AnimationInfo.SetFinish();
                }

                trans.localScale = Vector3.Lerp(Vector3.one * af.EndFloat, Vector3.one * af.OriginFloat, _AnimationInfo.EndTime * 1f / _AnimationInfo.StartTime);
            }
            else
            {
                _AnimationInfo.CallBack();

                if (_AnimationInfo.IsActionFinish)
                {
                    mListAnimationInfo.Remove(_AnimationInfo);
                    return;
                }
            }
        }
        private void WRectTo(AnimationInfo _AnimationInfo)
        {
            RectTransform trans = _AnimationInfo.ObjectX as RectTransform;

            AnimationFloat af = _AnimationInfo.AnimationPro as AnimationFloat;

            if (!_AnimationInfo.IsFinish && trans != null)
            {
                _AnimationInfo.EndTime -= Time.deltaTime;

                if (_AnimationInfo.EndTime <= 0)
                {
                    _AnimationInfo.SetFinish();
                }

                trans.sizeDelta = Vector3.Lerp(new Vector2(af.EndFloat, trans.sizeDelta.y), new Vector2(af.OriginFloat, trans.sizeDelta.y), _AnimationInfo.EndTime * 1f / _AnimationInfo.StartTime);
            }
            else
            {
                _AnimationInfo.CallBack();

                if (_AnimationInfo.IsActionFinish)
                {
                    mListAnimationInfo.Remove(_AnimationInfo);
                    return;
                }
            }
        }
        private void BarToValue(AnimationInfo _AnimationInfo)
        {
            Image trans = _AnimationInfo.ObjectX as Image;

            if (!_AnimationInfo.IsFinish && trans != null)
            {
                _AnimationInfo.EndTime -= Time.deltaTime;

                if (_AnimationInfo.EndTime <= 0)
                {
                    _AnimationInfo.SetFinish();
                }

                trans.fillAmount = _AnimationInfo.EndTime * 1f / _AnimationInfo.StartTime;
            }
            else
            {
                _AnimationInfo.CallBack();

                if (_AnimationInfo.IsActionFinish)
                {
                    mListAnimationInfo.Remove(_AnimationInfo);
                    return;
                }
            }
        }
        private void RateToZ(AnimationInfo _AnimationInfo)
        {
            Transform trans = _AnimationInfo.ObjectX as Transform;

            AnimationFloat af = _AnimationInfo.AnimationPro as AnimationFloat;

            if (!_AnimationInfo.IsFinish && trans)
            {
                _AnimationInfo.EndTime -= Time.deltaTime;

                if (_AnimationInfo.EndTime <= 0)
                {
                    _AnimationInfo.SetFinish();
                }

                trans.rotation = Quaternion.Euler(Vector3.Lerp(new Vector3(0, 0, af.OriginFloat), new Vector3(0, 0, af.EndFloat), _AnimationInfo.EndTime * 1f / _AnimationInfo.StartTime));
            }
            else
            {
                _AnimationInfo.CallBack();

                if (_AnimationInfo.IsActionFinish)
                {
                    mListAnimationInfo.Remove(_AnimationInfo);
                    return;
                }
            }
        }
        private void ShadeHiddenToText(AnimationInfo _AnimationInfo)
        {
            Text trans = _AnimationInfo.ObjectX as Text;

            AnimationFloat af = _AnimationInfo.AnimationPro as AnimationFloat;

            if (!_AnimationInfo.IsFinish && trans.gameObject.activeInHierarchy)
            {
                _AnimationInfo.EndTime -= Time.deltaTime;

                if (_AnimationInfo.EndTime <= 0)
                {
                    _AnimationInfo.SetFinish();
                }

                trans.color = new Color(trans.color.r, trans.color.g, trans.color.b, Mathf.Abs(af.EndFloat - _AnimationInfo.EndTime * 1f / _AnimationInfo.StartTime));
            }
            else
            {
                _AnimationInfo.CallBack();

                if (_AnimationInfo.IsActionFinish)
                {
                    mListAnimationInfo.Remove(_AnimationInfo);
                    return;
                }
            }
        }
        private void AddAnimation(object _Message)
        {
            if (mListAnimationInfo == null)
            {
                mListAnimationInfo = new List<AnimationInfo>();
            }

            if (mListInvokeTimeInfo == null)
            {
                mListInvokeTimeInfo = new List<InvokeTimeInfo>();
            }

            if (_Message is AnimationInfo)
            {
                mListAnimationInfo.Add(_Message as AnimationInfo); return;
            }
            if (_Message is InvokeTimeInfo)
            {
                mListInvokeTimeInfo.Add(_Message as InvokeTimeInfo); return;
            }
        }
        private void TransSpriteTo(AnimationInfo _AnimationInfo)
        {
            Transform trans = _AnimationInfo.ObjectX as Transform;

            AnimationVec3 af = _AnimationInfo.AnimationPro as AnimationVec3;

            if (!_AnimationInfo.IsFinish && trans.gameObject)
            {
                _AnimationInfo.EndTime -= Time.deltaTime;

                if (_AnimationInfo.EndTime <= 0)
                {
                    _AnimationInfo.SetFinish();
                }

                trans.position = Vector2.Lerp(af.EndVec3, af.OrignVec3, _AnimationInfo.EndTime * 1f / _AnimationInfo.StartTime);
            }
            else
            {
                _AnimationInfo.CallBack();

                if (_AnimationInfo.IsActionFinish)
                {
                    mListAnimationInfo.Remove(_AnimationInfo);
                    return;
                }
            }
        }

        /// <summary>
        /// 只符合 呈现 两个效果
        /// </summary>
        /// <param name="_AnimationInfo"></param>
        private void ShadeHiddenToSpriteRender(AnimationInfo _AnimationInfo)
        {
            SpriteRenderer spr = _AnimationInfo.ObjectX as SpriteRenderer;

            AnimationFloat af = _AnimationInfo.AnimationPro as AnimationFloat;

            if (!_AnimationInfo.IsFinish && spr.gameObject)
            {
                _AnimationInfo.EndTime -= Time.deltaTime;

                if (_AnimationInfo.EndTime <= 0)
                {
                    _AnimationInfo.SetFinish();
                }
                spr.color = new Color(spr.color.r, spr.color.g, spr.color.b, Mathf.Abs(af.EndFloat - _AnimationInfo.EndTime * 1f / _AnimationInfo.StartTime));
            }
            else
            {
                _AnimationInfo.CallBack();

                if (_AnimationInfo.IsActionFinish)
                {
                    mListAnimationInfo.Remove(_AnimationInfo);
                    return;
                }
            }
        }

        private void HiddenToImg(AnimationInfo _AnimationInfo)
        {
            Image spr = _AnimationInfo.ObjectX as Image;

            AnimationFloat af = _AnimationInfo.AnimationPro as AnimationFloat;

            if (!_AnimationInfo.IsFinish && spr.gameObject.activeInHierarchy)
            {
                _AnimationInfo.EndTime -= Time.deltaTime;

                if (_AnimationInfo.EndTime <= 0)
                {
                    _AnimationInfo.SetFinish();
                }
                spr.color = new Color(spr.color.r, spr.color.g, spr.color.b, Mathf.Abs(af.EndFloat - _AnimationInfo.EndTime * 1f / _AnimationInfo.StartTime));
            }
            else
            {
                _AnimationInfo.CallBack();

                if (_AnimationInfo.IsActionFinish)
                {
                    mListAnimationInfo.Remove(_AnimationInfo);
                    return;
                }
            }
        }

        private void InvokeDelay(InvokeTimeInfo _InvokeInfo)
        {
            _InvokeInfo.DelayTime -= Time.deltaTime;

            if (_InvokeInfo.DelayTime <= 0 && !_InvokeInfo.isFinish)
            {
                _InvokeInfo.DoCallBack();
            }

            if (_InvokeInfo.isFinish)
            {
                mListInvokeTimeInfo.Remove(_InvokeInfo);
                return;
            }
        }

        #endregion

        public void Clean()
        {
           // mListAnimationInfo.Clear();
            mListInvokeTimeInfo.Clear();
        }

        #endregion
    }
}
