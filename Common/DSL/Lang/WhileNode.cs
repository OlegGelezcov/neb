using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DSL.Lang {
    public class WhileNode : StatementNode {

        public WhileNode(Token tokem, BoolLogicExpressionNode inCheck, StatementListNode inStatement) 
            : base(tokem) {
            SetCheck(inCheck);
            SetStatement(inStatement);
        }

        public BoolLogicExpressionNode check { get; private set; }
        public StatementListNode statement { get; private set; }

        public void SetCheck(BoolLogicExpressionNode inCheck) {
            check = inCheck;
        }

        public void SetStatement(StatementListNode inStatement) {
            statement = inStatement;
        }

        public override void Eval(IExecutionContext context) {
            while(true) {
                check.Eval(context);
                if (!(bool)check.result) {
                    break;
                }
                statement.Eval(context);
            }
        }
    }
}
