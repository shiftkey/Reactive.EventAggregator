## Reactive.EventAggreator

This is an update of a blog post from José F. Romaniello about using Reactive Extensions to implement an event aggregator. [source](http://joseoncode.com/2010/04/29/event-aggregator-with-reactive-extensions/)


### Why bring it back?

Three reasons:

 - I want a lean, mean event aggregator for apps - without taking a dependency on an existing framework
 - It should be on NuGet
 - Using it as a demonstration of Portable Class Libaries and targetting different platforms from the one codebase.

### Usage

To install it, just run this from the Package Manager Console:

    Install-Package Reactive.EventAggregator

Here's some samples:

Subscribing to an event

    // arrange
    var eventWasRaised = false;
    var eventPublisher = new EventAggregator();

    // act
    eventPublisher.GetEvent<SampleEvent>()
        .Subscribe(se => eventWasRaised = true);

    eventPublisher.Publish(new SampleEvent());
    
    // assert
    eventWasRaised.ShouldBe(true);

Disposing of the event

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

Selectively subscribing to an event

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


### Portable Class Libraries What Even Is It

So I used this library as an excuse to investigate how to use PCL with NuGet, but hit some issues which made the experience less-than-appealing.

First off, I was at the mercy of upstream packages - in this case `Rx-Linq`. If you look inside the package for Rx-Linq you see this folder structure:

 - Net40
 - Net45
 - Portable-Net45-WinRT45
 - SL4-WindowsPhone71
 - SL5
 - WinRT45

Depending on which PCL profile you choose, NuGet will look for one of these folders:

 - portable-windows8+net45
 - portable-sl4+wp71+windows8

As Rx-Linq only has a library for the first profile, we need to adhere to that profile too.

So in the end, this is my project structure:

 - Portable-Net45-WinRT45
 - Net40
 - SL4-WindowsPhone71
 - SL5

Which generates these binaries:

 - Net40
 - Net45 (reusing portable profile)
 - Portable-Net45-WinRT45
 - SL4-WindowsPhone71
 - SL5
 - WinRT45 (reusing portable profile)

And we're good to go!