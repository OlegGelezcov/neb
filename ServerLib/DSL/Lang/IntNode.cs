using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DSL.Lang {
    public class IntNode : ExpressionNode {

        public IntNode(Token token)
            : base(token) { }

        public override void Eval(IExecutionContext context) {
            SetResult(int.Parse(token.text));
        }
    }
}
