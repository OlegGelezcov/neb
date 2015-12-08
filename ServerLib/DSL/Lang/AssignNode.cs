using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DSL.Lang {
    public class AssignNode : StatementNode{

        public AssignNode(VarNode inVar, Token token, ExpressionNode inValue) 
            : base(token) {
            SetID(inVar);
            SetValue(inValue);
        }

        public VarNode id { get; private set; }
        public ExpressionNode value { get; private set; }

        public void SetID(VarNode inID) {
            id = inID;
        }

        public void SetValue(ExpressionNode inValue) {
            value = inValue;
        }

        public override void Eval(IExecutionContext context) {
            id.Eval(context);
            value.Eval(context);
            context.assign(id.result.ToString(), value.result);
        }

    }
}
