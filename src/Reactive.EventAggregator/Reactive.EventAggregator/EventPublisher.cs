using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace Reactive.EventAggregator
{
    public class EventPublisher : IEventPublisher
    {
        readonly ThreadSafeDictionary<Type, object> subjects = new ThreadSafeDictionary<Type, object>();

        public IObservable<TEvent> GetEvent<TEvent>()
        {
            ISubject<TEvent> subject;

            if (subjects.ContainsKey(typeof (TEvent)))
            {
                subject = (ISubject<TEvent>)subjects[typeof (TEvent)];
            }
            else
            {
                subject = new Subject<TEvent>();
                subjects.Add(typeof(TEvent), subject);
            }
            
            return subject.AsObservable();
        }

        public void Publish<TEvent>(TEvent sampleEvent)
        {
            object subject;
            if (subjects.TryGetValue(typeof (TEvent), out subject))
            {
                ((ISubject<TEvent>)subject).OnNext(sampleEvent);
            }
        }
    }
}