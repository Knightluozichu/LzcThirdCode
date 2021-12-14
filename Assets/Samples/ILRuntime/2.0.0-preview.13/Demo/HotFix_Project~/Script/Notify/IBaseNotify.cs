
using System.Collections.Generic;

/*
 * @author LuoZichu
 * @time 2019/7/1
 */

/// <summary>
/// 所有类的基类
/// </summary>
namespace RedRedJiang.Unity
{
    public delegate void DelExtueHandle(object message = null);
    
    public interface IBaseNotify
    {
        Dictionary<int, DelExtueHandle> DicEventDelegate { get; set; }
        void Excute(int _evenMa, object _Message = null);
    }
}
