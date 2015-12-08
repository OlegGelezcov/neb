using System;
namespace Common
{
    public interface IState<T>
    {
        T Name { get; }
        void ExecuteBeginState();
        void ExecuteState();
        void ExecuteEndState();

        void Init(Action begin, Action state, Action end);
    }
}

