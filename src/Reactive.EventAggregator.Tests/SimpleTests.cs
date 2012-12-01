using System;
using System.Reactive.Linq;
using Xunit;
using Shouldly;

namespace Reactive.EventAggregator.Tests
{
    public class SimpleTests
    {
        class SampleEvent {
            public int Status { get; set; }
        }

        [Fact]
        public void testing_subscribe()
        {
            // arrange
            var eventWasRaised = false;
            var eventPublisher = new EventAggregator();

            // act
            eventPublisher.GetEvent<SampleEvent>()
                .Subscribe(se => eventWasRaised = true);

            eventPublisher.Publish(new SampleEvent());
            
            // assert
            eventWasRaised.ShouldBe(true);
        }

        [Fact]
        public void testing_unsubscribe()
        {
            // arrange
            var eventWasRaised = false;
            var eventPublisher = new EventAggregator();

            // act
            var subscription = eventPublisher.GetEvent<SampleEvent>()
                                             .Subscribe(se => eventWasRaised = true);

            subscription.Dispose();
            eventPublisher.Publish(new SampleEvent());

            // assert
            eventWasRaised.ShouldBe(false);
        }

        [Fact]
        public void testing_selective_subscribe()
        {
            // arrange
            var eventWasRaised = false;
            var eventPublisher = new EventAggregator();

            // act
            eventPublisher.GetEvent<SampleEvent>()
                .Where(se => se.Status == 1)
                .Subscribe(se => eventWasRaised = true);

            eventPublisher.Publish(new SampleEvent { Status = 1 });

            // assert
            eventWasRaised.ShouldBe(true);
        }

        [Fact]
        public void testing_selective_subscribe_ignored()
        {
            // arrange
            var eventWasRaised = false;
            var eventPublisher = new EventAggregator();

            // act
            eventPublisher.GetEvent<SampleEvent>()
                .Where(se => se.Status != 1)
                .Subscribe(se => eventWasRaised = true);

            eventPublisher.Publish(new SampleEvent { Status = 1 });

            // assert
            eventWasRaised.ShouldBe(false);
        }
    }
}
