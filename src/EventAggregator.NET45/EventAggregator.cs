using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace Reactive.EventAggregator
{
    // based on http://joseoncode.com/2010/04/29/event-aggregator-with-reactive-extensions/
    // and http://machadogj.com/2011/3/yet-another-event-aggregator-using-rx.html
    public class EventAggregator : IEventAggregator
    {
        readonly ISubject<object> subject = new Subject<object>();

        public IObservable<TEvent> GetEvent<TEvent>()
        {
            return subject.OfType<TEvent>().AsObservable();
        }

        public void Publish<TEvent>(TEvent sampleEvent)
        {
            subject.OnNext(sampleEvent);
        }
    }
}