using System;


namespace Common {
    public class FSMState<T> : IState<T> {
        private Action begin;
        private Action state;
        private Action end;
        private readonly T name;

        public FSMState(T name, Action begin, Action state, Action end) {
            this.name = name;
            this.Init(begin, state, end);
        }

        public FSMState(T name) {
            this.name = name;
        }


        public T Name {
            get { return this.name; }
        }

        public void ExecuteBeginState() {
            if (this.begin != null) {
                this.begin();
            }
        }

        public void ExecuteState() {
            if (this.state != null) {
                this.state();
            }
        }

        public void ExecuteEndState() {
            if (this.end != null) {
                this.end();
            }
        }

        public void Init(Action begin, Action state, Action end) {
            this.begin = begin;
            this.state = state;
            this.end = end;
        }
    }
}
