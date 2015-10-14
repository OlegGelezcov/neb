using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nebula.Mmo.Items {
    public interface IItemProperty {
        bool TryGetProperty<T>(byte name, out T value);
        T GetProperty<T>(byte name);
        bool ContainsProperty(byte name);
    }
}
