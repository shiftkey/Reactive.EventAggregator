## Reactive.EventAggreator

This is an update of a blog post from José F. Romaniello about using Reactive Extensions to implement an event aggregator. [source](http://joseoncode.com/2010/04/29/event-aggregator-with-reactive-extensions/)


### Why bring it back?

Three reasons:

 - I want a lean, mean event aggregator for apps - without taking a dependency on an existing framework
 - It should be on NuGet
 - Using it as a demonstration of Portable Class Libaries and targetting different platforms from the one codebase.

### Portable Class Libraries And Upstream Dependencies

I use this project as an excuse to investigate how to use PCL with NuGet, but hit some issues which made the experience less-than-appealing.

So `Rx-Linq` is a dependency for this package. If you look inside the package for Rx-Linq you see it supports these profiles:

 - Net40
 - Net45
 - Portable-Net45-WinRT45
 - SL4-WindowsPhone71
 - SL5
 - WinRT45

And while NuGet supports installing packages into PCL profiles, it is limited to these profiles:

 - portable-windows8+net45 - a small profile encompassing  the modern APIs
 - portable-sl4+wp71+windows8 - the biggest range of profiles, and thus the smallest set of available APIs

As Rx-Linq only has a library for the first profile, we need to adhere to that profile too.

So this becomes my profile structure:

 - Portable-Net45-WinRT45
 - Net40
 - SL4-WindowsPhone71
 - SL5

Which generates these profiles:

 - Net40
 - Net45 (reusing portable profile)
 - Portable-Net45-WinRT45
 - SL4-WindowsPhone71
 - SL5
 - WinRT45 (reusing portable profile)

And that's it!


### Usage

To install it, just run this from the Package Manager Console:

    Install-Package Reactive.EventAggregator

Here's some samples:

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


