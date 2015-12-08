using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Common
{
    public interface IBaseSaver
    {
        void SaveModified();
    }

    public interface ISaver<T>
    {
        void Update(T obj);
        
    }
}
