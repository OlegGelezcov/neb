using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DSL.Lang {
    public abstract class StatementNode : AST {

        public StatementNode() { }

        public StatementNode(Token token)
            : base (token) { }

    }
}
