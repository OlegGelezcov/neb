﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DSL.Lang {
    public class GreaterThanEqualNode : BoolLogicExpressionNode {
        public GreaterThanEqualNode(Token token, ExpressionNode inLeft, ExpressionNode inRight)
            : base(token) {
            SetLeft(inLeft);
            SetRight(inRight);
        }

        public ExpressionNode left { get; private set; }
        public ExpressionNode right { get; private set; }

        public void SetLeft(ExpressionNode inLeft) {
            left = inLeft;
        }

        public void SetRight(ExpressionNode inRight) {
            right = inRight;
        }

        public override void Eval(IExecutionContext context) {
            left.Eval(context);
            right.Eval(context);
            if (context.GreaterThanEqual(left, right)) {
                SetResult(true);
            } else {
                SetResult(false);
            }
        }
    }
}
