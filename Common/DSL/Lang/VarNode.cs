using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DSL.Lang {
    public class VarNode : ExpressionNode {

        public VarNode(Token token)
            : base(token) { }

        public override void Eval(IExecutionContext context) {
            SetResult(token.text);
        }
    }
}
