
using System.Collections.Generic;



namespace Common {
    public interface IFSM<T> {
        bool IsState(T state);
        bool GotoState(T newState);

        void AddState(IState<T> state);

        void ForceState(T stateName);

        //StateTemplate StateInitial { get; }
        IState<T> CurrentState { get; }
        IState<T> NewState { get; }
        void Update();

        Dictionary<T, IState<T>> States { get; }
    }


}
