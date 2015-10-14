using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DSL.Lang {
    public class ElseIfNode : StatementNode  {

        public ElseIfNode(Token token, BoolLogicExpressionNode boolExpression, StatementListNode statement) 
            : base(token ) {
            SetCheck(boolExpression);
            SetStatementList(statement);
        }

        public BoolLogicExpressionNode check { get; private set; }
        public StatementListNode statementList { get; private set; }

        public void SetCheck(BoolLogicExpressionNode inCheck) {
            check = inCheck;
        }

        public void SetStatementList(StatementListNode inStatement) {
            statementList = inStatement;
        }

        public override void Eval(IExecutionContext context) {
            check.Eval(context);
            if((bool)check.result) {
                statementList.Eval(context);
            }
            SetResult((bool)check.result);
        }
    }
}
