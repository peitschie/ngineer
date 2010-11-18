using System;

namespace NGineer.Internal
{
    public sealed class DisposableAction : IDisposable
    {
        private readonly Action _callback;

        public DisposableAction(Action callback)
        {
            _callback = callback;
        }

        public void Dispose()
        {
            _callback();
        }
    }
}