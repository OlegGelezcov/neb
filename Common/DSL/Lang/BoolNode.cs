﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DSL.Lang {
    public class BoolNode : ExpressionNode {
        public BoolNode(Token token)
            : base(token) { }

        public override void Eval(IExecutionContext context) {
            SetResult(bool.Parse(token.text));
        }
    }
}