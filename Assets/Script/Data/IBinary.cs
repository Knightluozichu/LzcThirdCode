using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Script.Data
{
    public interface IBinary
    {
        T GetBinaryObject<T>(string _Path) where T : class;
        void SaveBinaryObject<T>(string _Path, T _class) where T : class;
    }
}
