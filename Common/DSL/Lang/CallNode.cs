using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DSL.Lang {
    public class CallNode : ExpressionNode {

        public CallNode(Token token, VarNode inSource, string inMethod, List<ExpressionNode> inArgs)
            : base(token) {
            method = inMethod;
            source = inSource;
            args = inArgs;
        }


        public VarNode source { get; private set; }
        public string method { get; private set; }
        public List<ExpressionNode> args { get; private set; }

        public override void Eval(IExecutionContext context) {
            source.Eval(context);
            List<object> lstArgs = new List<object>();
            foreach(var en in args ) {
                en.Eval(context);
                lstArgs.Add(en.result);
            }

            context.invoke(source.result.ToString(), method, lstArgs.ToArray());
        }
    }
}
