using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DSL.Lang {
    public interface IExecutionContext {
        bool Equals(object left, object right);
        bool NotEquals(object left, object right);
        bool LessThan(object left, object right);
        bool LessThanEqual(object left, object right);
        bool GreaterThan(object left, object right);
        bool GreaterThanEqual(object left, object right);
        object Add(object left, object right);
        object Sub(object left, object right);
        object Mul(object left, object right);
        object Div(object left, object right);
        void assign(string var, object val);
        object invoke(string source, string method, object[] args);
    }
}
