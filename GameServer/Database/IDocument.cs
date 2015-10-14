using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Space.Database {
    public interface IDocument {
        string CharacterId { get; }

        bool IsNewDocument { get; set; }
    }
}
