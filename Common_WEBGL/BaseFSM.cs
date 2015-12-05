
using System.Collections.Generic;


namespace Common {
    public class BaseFSM<T> : IFSM<T> {
        private Dictionary<T, IState<T>> states;
        private IState<T> currentState;
        private IState<T> nextState;

        public BaseFSM() {
            this.states = new Dictionary<T, IState<T>>();
        }

        public BaseFSM(IState<T> initialState) {
            this.states = new Dictionary<T, IState<T>>();
            if (initialState != null) {
                this.states.Add(initialState.Name, initialState);
            }
            this.currentState = initialState;
        }

        public bool IsState(T stateName) {
            if (this.currentState == null)
                return false;
            if (this.currentState.Name.Equals(stateName))
                return true;
            else
                return false;
        }

        public bool GotoState(T newState) {
            if (this.states.ContainsKey(newState)) {
                this.nextState = this.states[newState];
                return true;
            }
            return false;
        }

        public void AddState(IState<T> state) {
            if (state == null)
                return;

            if (this.states.ContainsKey(state.Name)) {
                this.states[state.Name] = state;
            } else {
                this.states.Add(state.Name, state);
            }
        }

        public IState<T> CurrentState {
            get { return this.currentState; }
        }

        public IState<T> NewState {
            get { return this.nextState; }
        }


        public void Update() {
            if (this.nextState != null) {
                if (false == this.currentState.Name.Equals(this.nextState.Name)) {
                    this.currentState.ExecuteEndState();
                    this.currentState = this.nextState;
                    this.nextState = null;
                    this.currentState.ExecuteBeginState();
                } else {
                    this.currentState = this.nextState;
                    this.nextState = null;
                }
            }
            if (this.currentState != null)
                this.currentState.ExecuteState();
        }

        public Dictionary<T, IState<T>> States {
            get { return this.states; }
        }


        public void ForceState(T stateName, bool executeBegin) {
            if (this.states.ContainsKey(stateName)) {
                this.currentState = this.states[stateName];

                if (executeBegin) {
                    if (this.currentState != null)
                        this.currentState.ExecuteBeginState();
                }
            }
        }


        public void ForceState(T stateName) {
            this.ForceState(stateName, false);
        }
    }
}
