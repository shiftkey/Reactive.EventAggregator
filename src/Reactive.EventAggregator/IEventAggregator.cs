using System;

namespace Reactive.EventAggregator
{
    public interface IEventAggregator : IDisposable
    {
        IObservable<TEvent> GetEvent<TEvent>();
        void Publish<TEvent>(TEvent sampleEvent);
    }
}
