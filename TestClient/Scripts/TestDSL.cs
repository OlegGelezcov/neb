using Irony.Parsing;
using Irony.Samples.CSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Irony.Interpreter;
using Irony.Interpreter.Ast;
using Nebula.DSL;
using Common;

namespace TestClient.Scripts {
    public static class TestDSL {

        public static void Test() {
            Script script = new Script();
            ConditionContext context = new ConditionContext();
            script.Load(context, "turret.txt");
            Console.WriteLine(script.CheckStartConditions());
        }

    }

    public class ConditionContext : IConditionContext {
        public object GetVariable(string name) {
            if(name == "world_race") {
                return Race.Borguzands;
            }
            return null;
        }
    }
}
