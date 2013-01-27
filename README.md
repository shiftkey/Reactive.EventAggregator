## Reactive.EventAggreator

This is an update of a blog post from José F. Romaniello about using Reactive Extensions to implement an event aggregator. [source](http://joseoncode.com/2010/04/29/event-aggregator-with-reactive-extensions/)

### Usage

To install it, just run this from the Package Manager Console:

    Install-Package Reactive.EventAggregator

### Why bring it back?

Three reasons:

 - I wanted a simple event aggregator, without taking a dependency on an MVVM framework
 - It should be available on NuGet
 - Demonstrate Portable Class Libaries and targetting different platforms from one codebase

### Portable Class Libraries All The Things (kinda)

I use this project as an excuse to investigate how to use PCL with NuGet, but hit some issues which made the experience less-than-appealing.

Yes, NuGet supports installing packages into PCL projects. 

While the NuGet docs only mention two profiles, I've seen at least three different ones in the wild:

 - `portable-windows8+net45` - a small profile encompassing  the modern APIs
 - `portable-sl4+wp71+windows8` - the biggest range of profiles, and thus the smallest set of available APIs
 - `portable-win+net40+sl50+wp8` - this is what the Autofac 3.0 beta supports

Why does this matter? Because if you try and install something from NuGet into a PCL project, you may see an error like this:

    Could not install package 'Rx-Interfaces 2.0.21114'. You are trying to install this package into a project that targets 
    'portable-win+net45+sl50+wp80', but the package does not contain any assembly references or content files that are 
    compatible with that framework. For more information, contact the package author.

Hopefully in a future version of NuGet it gets better at indicating to the user which PCL profiles a package supports - so that it doesn't even show you these incompatible packages...

Anyway, where were we? Ah yes, Rx. 

The Reactive Extensions are a dependency for this package and only supports the first profile (`portable-windows8+net45`) and has separate profiles for the other platforms. If you look inside the package for `Rx-Linq` you see it supports these profiles:

 - `Net40`
 - `Net45`
 - `Portable-Net45-WinRT45`
 - `SL4-WindowsPhone71`
 - `SL5`
 - `WinRT45`

So the solution structure for this project becomes:

 - `Portable-Net45-WinRT45`
 - `Net40`
 - `SL4-WindowsPhone71`
 - `SL5`

Which generates these profiles:

 - `Net40`
 - `Net45` (reusing portable profile)
 - `Portable-Net45-WinRT45`
 - `SL4-WindowsPhone71`
 - `SL5`
 - `WinRT45` (reusing portable profile)

And that's it!

**Footnote:** You may notice the lack of WP8 in that list - when NuGet cannot find a specific `windowsphone8` profile in a package, it will fall back to the Windows Phone 7.1 (Mango) profile `SL4-WindowsPhone71` instead. I tested `Rx-Linq` and `Reactive.EventAggregator` in the WP8 simulator and didn't spot any issues. Please log an issue here if you find something odd.

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
