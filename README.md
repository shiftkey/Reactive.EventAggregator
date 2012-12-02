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


### Some other notes

Stage 1:

The initial blog post used a ConcurrentDictionary to manage internal observables. This works excellently for the .NET 4+ but does not allow for portability to other platforms. So I switched it out for a ThreadSafeDictionary from [this post](http://devplanet.com/blogs/brianr/archive/2008/09/26/thread-safe-dictionary-in-net.aspx) and started exploring how far I could push it.

I got stuck on the lack of [ReaderWriterLockSlim](http://social.msdn.microsoft.com/Forums/et-EE/netfxbcl/thread/bdfe44d9-229f-4ce6-96a4-bc6c0d084c55) in WP7.5/SL4/SL5 - I could hack around it using something like [this](http://code.google.com/p/mongodb-silverlight-driver/source/browse/trunk/Bson/Added/ReaderWriterLockSlim.cs?r=2) but I'm not really interested in supporting this at the moment.

So this currently runs on .NET 4.5/WP8/Windows Store. I can't push support back to .NET 4.0.3 because it then chooses SL5 too (TODO: why?) and that invokes the ReaderWriterLockSlim issue as above.

Stage 2:

Jose pointed me to [a neat trick](http://machadogj.com/2011/3/yet-another-event-aggregator-using-rx.html) - use OfType<T> to do the filtering, rather than creating multiple Subjects for each message type. This made the whole ConcurrentDictionary issue redundant.

Now I'm stuck on the frameworks that the Reactive Extensions PCL library supports - the latest NuGet release (2.0.21114 as of writing) doesn't support WP8 so I'm stuck a version back. I need to eloquently describe the hell that is "picking the right configuration for consuming a PCL library" as choosing the right profile to install a package has been a nightmare I still haven't properly escaped.

