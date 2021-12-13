using UnityEngine;
using UnityEngine.EventSystems;


/*
 * @author LuoZichu
 * @time 2019/7/1
 */

namespace RedRedJiang.Unity
{

    /// <summary>
    /// 组件自定义EventTriggerListener
    /// </summary>
    public class CustomEventTriggerListener : UnityEngine.EventSystems.EventTrigger
    {
        #region Field
        public delegate void VoidDelegate(GameObject go);
        public delegate void VoidDelegatePED(PointerEventData eventData);
        public VoidDelegate onClick;
        public VoidDelegate onDown;
        public VoidDelegate onEnter;
        public VoidDelegate onExit;
        public VoidDelegate onUp;
        public VoidDelegate onSelect;
        public VoidDelegate onUpdateSelect;
        public VoidDelegatePED onBeginDrag, onDrag, onEndDrag;

        #endregion

        #region Method
        #region Public

        /// <summary>
        /// 得到“监听器”组件
        /// </summary>
        /// <param name="go">监听的游戏对象</param>
        /// <returns>
        /// 监听器
        /// </returns>
        public static CustomEventTriggerListener Get(GameObject go)
        {
            CustomEventTriggerListener lister = go.GetComponent<CustomEventTriggerListener>();
            if (lister == null)
            {
                lister = go.AddComponent<CustomEventTriggerListener>();
            }
            return lister;
        }
        public override void OnBeginDrag(PointerEventData eventData)
        {
            base.OnBeginDrag(eventData);
            if (onBeginDrag != null)
            {
                onBeginDrag(eventData);
            }
        }
        public override void OnDrag(PointerEventData eventData)
        {
            base.OnDrag(eventData);
            if (onDrag != null)
            {
                onDrag(eventData);
            }
        }
        public override void OnEndDrag(PointerEventData eventData)
        {
            base.OnEndDrag(eventData);
            if (onEndDrag != null)
            {
                onEndDrag(eventData);
            }
        }
        public override void OnPointerClick(PointerEventData eventData)
        {
            if (onClick != null)
            {
                onClick(gameObject);
            }
        }

        public override void OnPointerDown(PointerEventData eventData)
        {
            if (onDown != null)
            {
                onDown(gameObject);
            }
        }

        public override void OnPointerEnter(PointerEventData eventData)
        {
            if (onEnter != null)
            {
                onEnter(gameObject);
            }
        }

        public override void OnPointerExit(PointerEventData eventData)
        {
            if (onExit != null)
            {
                onExit(gameObject);
            }
        }

        public override void OnPointerUp(PointerEventData eventData)
        {
            if (onUp != null)
            {
                onUp(gameObject);
            }
        }

        public override void OnSelect(BaseEventData eventBaseData)
        {
            if (onSelect != null)
            {
                onSelect(gameObject);
            }
        }

        public override void OnUpdateSelected(BaseEventData eventBaseData)
        {
            if (onUpdateSelect != null)
            {
                onUpdateSelect(gameObject);
            }
        }

        #endregion
        #endregion

    }//Class_end
}
