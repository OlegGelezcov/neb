using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DSL.Lang {
    public class FloatNode : ExpressionNode  {

        public FloatNode(Token token)
            : base(token) { }

        public override void Eval(IExecutionContext context) {
            SetResult(float.Parse(token.text));
        }
    }
}
