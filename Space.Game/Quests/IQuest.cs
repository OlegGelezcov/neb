using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nebula.Quests {
    public interface IQuest {
        bool TryGetInteger(string name, out int value);
        bool TryGetFloat(string name, out float value);
        bool TryGetBool(string name, out bool value);
    }
}
