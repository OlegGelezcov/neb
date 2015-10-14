using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DSL.Lang {
    public class IfNode  : StatementNode {

        public IfNode(Token token, BoolLogicExpressionNode inCheck, StatementListNode inStatements, List<ElseIfNode> inElseIfs, StatementListNode inElseStatements) 
            : base(token) {
            check = inCheck;
            statementList = inStatements;
            elseifStatements = inElseIfs;
            elseStatements = inElseStatements;
        }
        public BoolLogicExpressionNode check { get; private set; }
        public StatementListNode statementList { get; private set; }
        public List<ElseIfNode> elseifStatements { get; private set; }
        public StatementListNode elseStatements { get; private set; }

        public override void Eval(IExecutionContext context) {
            check.Eval(context);
            bool anyComplted = false;

            if((bool)check.result) {
                statementList.Eval(context);
                anyComplted = true;
            } else {
                foreach(var ei in elseifStatements) {
                    ei.Eval(context);
                    if((bool)ei.result) {
                        anyComplted = true;
                        break;
                    }
                }
            }


            if(!anyComplted) {
                elseStatements.Eval(context);
            }

        }
    }
}
