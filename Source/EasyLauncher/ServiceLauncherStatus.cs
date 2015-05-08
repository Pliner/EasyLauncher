using System.Threading;

namespace EasyLauncher
{
    public enum ServiceLauncherState
    {
        Stopped,
        Starting,
        Started,
        Stopping
    }

    public interface IServiceLauncherStatus
    {
        bool TryChangeState(ServiceLauncherState fromState, ServiceLauncherState toState);
        void ChangeState(ServiceLauncherState state);
        bool HasState(ServiceLauncherState state);
    }

    public sealed class ServiceLauncherStatus : IServiceLauncherStatus
    {
        private int internalState = (int)ServiceLauncherState.Stopped;

        public bool TryChangeState(ServiceLauncherState fromState, ServiceLauncherState toState)
        {
            return Interlocked.CompareExchange(ref internalState, (int)fromState, (int)toState) == (int) fromState;
        }

        public void ChangeState(ServiceLauncherState state)
        {
            Interlocked.Exchange(ref internalState, (int) state);
        }

        public bool HasState(ServiceLauncherState state)
        {
            return (ServiceLauncherState)Interlocked.CompareExchange(ref internalState, (int)state, (int)state) == state;
        }
    }
}