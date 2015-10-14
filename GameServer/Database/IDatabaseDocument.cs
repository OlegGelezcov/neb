using Space.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Space.Database {
    public interface IDatabaseDocument<T> : IDocument {
        void Set(T sourceObject);
        T SourceObject(IRes resource);
         
    }
}
