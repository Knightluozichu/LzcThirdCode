
using System.Collections.Generic;

/*
 * @author LuoZichu
 * @time 2019/7/1
 */

    /// <summary>
    /// 所有类的基类
    /// </summary>
namespace Assets.Script.Notify
{
    
    public class IBaseNotify
    {
        protected delegate void DelExtueHandle(object message = null);
        protected Dictionary<int, DelExtueHandle> mDicEventDelegate = new Dictionary<int, DelExtueHandle>();
        public virtual void  Excute(int _evenMa, object _Message = null) { }
    }
}
