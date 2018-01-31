using Common;
using Nebula.Engine;
using Nebula.Game.Components.Activators;
//using Nebula.Game.Components.Quests;
using Nebula.Server.Components;

namespace Nebula.Game.Activators {
    public class VariableActivator : ActivatorObject {

        private string m_VariableName;
        private object m_ActivatorValue;


        public override void Init(ActivatorComponentData data) {
            base.Init(data);
            VariableActivatorComponentData varData = data as VariableActivatorComponentData;
            m_VariableName = varData.variableName;
            m_ActivatorValue = varData.variableValue;
        }

        public override void OnActivate(NebulaObject source, out RPCErrorCode errorCode) {
            errorCode = RPCErrorCode.Ok;
            /*
            
            if(interactable) {
                if(IsDistanceValid(source)) {
                    var questMgr = source.GetComponent<QuestManager>();

                    if(questMgr != null ) {
                        switch (activatorType) {
                            case ActivatorType.BoolVarSet: {
                                    questMgr.SetBoolVariable(variableName, (bool)m_ActivatorValue);
                                }
                                break;
                            case ActivatorType.FloatVarSet: {
                                    questMgr.SetFloatVariable(variableName, (float)m_ActivatorValue);
                                }
                                break;
                            case ActivatorType.IntVarIncr: {
                                    questMgr.IncreaseInteger(variableName, (int)m_ActivatorValue);
                                }
                                break;
                            case ActivatorType.IntVarSet: {
                                    questMgr.SetIntegerVariable(variableName, (int)m_ActivatorValue);
                                }
                                break;
                        }
                    }
                    InteractOff();
                } else {
                    errorCode = RPCErrorCode.DistanceIsFar;
                }
            } else {
                errorCode = RPCErrorCode.ObjectNotInteractable;
            }*/

        }

        private string variableName {
            get {
                return m_VariableName;
            }
        }
    }
}
