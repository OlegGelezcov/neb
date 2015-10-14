using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DSL.Lang {
    public abstract class AST {

        public AST parent { get; private set; }
        public Token token { get; private set; }

        private Token mToken;

        public AST() { }
        public AST(Token token) {
            mToken = token;
        }

        public void SetParent(AST ast) {
            parent = ast;
        }

        public override string ToString() {
            return mToken != null ? mToken.ToString() : "(null)";
        }

        public object result { get; private set; }

        public void SetResult(object res) {
            result = res;
        }

        public abstract void Eval( IExecutionContext context );
    }
}
