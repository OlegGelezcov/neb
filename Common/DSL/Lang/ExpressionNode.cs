using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DSL.Lang {
    public abstract class ExpressionNode : AST {

        public ExpressionNode(Token token)
            : base(token) { }


    }
}
