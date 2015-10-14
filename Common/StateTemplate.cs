namespace Common
{

    using System;

    /*
    public class StateTemplate : State
    {
        

        protected Action _beginState;
        protected Action _state;
        protected Action _endState;

        public Action BeginState {
            get {
                return _beginState;
            }
            set {
                _beginState = value;
            }
        }

        public Action State {
            get {
                return _state;
            }
            set {
                _state = value;
            }
        }

        public Action EndState {
            get {
                return _endState;
            }
            set {
                _endState = value;
            }
        }

        public StateTemplate()
        {

        }

        public void Set(Action beginState, Action state, Action endState)
        {
            _beginState = beginState;
            _state = state;
            _endState = endState;
        }

        public override void ExecuteBeginState()
        {
            if (_beginState != null)
                _beginState();
        }

        public override void ExecuteState()
        {
            if (_state != null)
                _state();
        }

        public override void ExecuteEndState()
        {
            if (_endState != null)
                _endState();
        }
    }*/

}
