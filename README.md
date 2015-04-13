## Reactive.EventAggregator

This is an update of a blog post from José F. Romaniello about using Reactive Extensions to implement an event aggregator. [source](http://joseoncode.com/2010/04/29/event-aggregator-with-reactive-extensions/)

[![Build status](https://ci.appveyor.com/api/projects/status/b0yt9pn5njghikr2)](https://ci.appveyor.com/project/shiftkey/reactive-eventaggregator)

### Usage

To install it, just run this from the Package Manager Console:

    Install-Package Reactive.EventAggregator

### Why bring it back?

Three reasons:

 - I wanted a simple event aggregator, without taking a dependency on an MVVM framework
 - It should be available on NuGet
 - Demonstrate Portable Class Libaries and targetting different platforms from one codebase

### Samples

#### Subscribing to an event

    // arrange
    var eventWasRaised = false;
    var eventPublisher = new EventAggregator();

    // act
    eventPublisher.GetEvent<SampleEvent>()
                  .Subscribe(se => eventWasRaised = true);

    eventPublisher.Publish(new SampleEvent());
    
    // assert
    eventWasRaised.ShouldBe(true);

#### Disposing of the event

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

#### Selectively subscribing to an event

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
