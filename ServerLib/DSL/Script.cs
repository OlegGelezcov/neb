using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Common;

namespace Nebula.DSL {
    public class Script {
        private List<Condition> mStartConditions = new List<Condition>();
        private IConditionContext mConditionContext;

        public Script() { }

        public void Load(IConditionContext context, string scriptFile) {
            mConditionContext = context;
            string[] lines = File.ReadAllLines(scriptFile);

            bool startBlockFounded = false;
            foreach(var line in lines) {
                if(IsBeginStartLine(line)) {
                    startBlockFounded = true;
                    continue;
                }
                if(startBlockFounded) {
                    if(IsEndLine(line)) {
                        startBlockFounded = false;
                        continue;
                    }
                    var condition = ParseCondition(line);
                    if(condition != null ) {
                        mStartConditions.Add(condition);
                    }
                }
            }
        } 

        private bool IsBeginStartLine(string line) {
            string[] tokens = line.Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
            if(tokens[0].ToLower() == "begin" ) {
                if(tokens[1].ToLower() == "start") {
                    return true;
                }
            }
            return false;
        } 

        private bool IsEndLine(string line) {
            return line.Trim().ToLower() == "end";
        }

        private Condition ParseCondition(string line) {
            if(string.IsNullOrEmpty(line)) {
                return null;
            }
            string[] tokens = line.Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
            if(tokens.Length == 0) {
                return null;
            }

            switch(tokens[0]) {
                case "eq":
                    {
                        string varName = tokens[1];
                        switch(varName.ToLower()) {
                            case "world_race":
                                {
                                    Race race = (Race)Enum.Parse(typeof(Race), tokens[2]);
                                    return new RaceEqualCondition(mConditionContext, varName.ToLower(), race);
                                }
                        }
                    }
                    break;
                case "neq":
                    {
                        string varName = tokens[1];
                        switch (varName.ToLower()) {
                            case "world_race":
                                {
                                    Race race = (Race)Enum.Parse(typeof(Race), tokens[2]);
                                    return new RaceNotEqualCondition(mConditionContext, varName.ToLower(), race);
                                }
                        }
                    }
                    break;
            }
            return null;
        }

        public bool CheckStartConditions() {
            if(mStartConditions.Count == 0 ) {
                return true;
            }

            bool allTrue = true;
            foreach(var c in mStartConditions) {
                allTrue = allTrue && c.Check();
            }
            return allTrue;
        }
        
    }
}
