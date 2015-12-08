using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DSL.Lang {
    public class StatementListNode : AST {

        public StatementListNode() {
            elements = new List<StatementNode>();
        }

        public StatementListNode(List<StatementNode> inStatements)
            : base(new Token(Token.TokenType.STAT_LIST)) {
            SetElements(inStatements);
        }

        public List<StatementNode> elements { get; private set; }

        public void SetElements(List<StatementNode> inElements) {
            elements = inElements;
        }

        public void Add(StatementNode element) {
            elements.Add(element);
        }

        public override void Eval(IExecutionContext context) {
            foreach(var st in elements) {
                st.Eval(context);
            }
        }
    }
}
