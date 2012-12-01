## Reactive.EventAggreator

This is an update of a blog post from José F. Romaniello about using Reactive Extensions to implement an event aggregator. [source](http://joseoncode.com/2010/04/29/event-aggregator-with-reactive-extensions/)


### Why bring it back?

Three reasons:

 - I want a lean, mean event aggregator for apps - without taking a dependency on an existing framework
 - It should be on NuGet
 - Using it as a demonstration of Portable Class Libaries and targetting different platforms from the one codebase.

### Usage

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


### Some other notes

The initial blog post used a ConcurrentDictionary to manage internal observables. This works excellently for the .NET 4+ but does not allow for portability to other platforms. So I switched it out for a ThreadSafeDictionary from [this post](http://devplanet.com/blogs/brianr/archive/2008/09/26/thread-safe-dictionary-in-net.aspx) and started exploring how far I could push it.

I got stuck on the lack of [ReaderWriterLockSlim](http://social.msdn.microsoft.com/Forums/et-EE/netfxbcl/thread/bdfe44d9-229f-4ce6-96a4-bc6c0d084c55) in WP7.5/SL4/SL5 - I could hack around it using something like [this](http://code.google.com/p/mongodb-silverlight-driver/source/browse/trunk/Bson/Added/ReaderWriterLockSlim.cs?r=2) but I'm not really interested in supporting this at the moment.

So this currently runs on .NET 4.5/WP8/Windows Store. I can't push support back to .NET 4.0.3 because it then chooses SL5 too (TODO: why?) and that invokes the ReaderWriterLockSlim issue as above.

