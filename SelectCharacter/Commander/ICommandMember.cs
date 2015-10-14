using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SelectCharacter.Commander {
    public interface ICommandMember : IInfoSource {
        string login { get; }
        string gameRefID { get; }
        string characterID { get; }
        bool exists { get; }

    }
}
