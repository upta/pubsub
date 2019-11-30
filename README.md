# PubSub .Net [![Build status](https://ci.appveyor.com/api/projects/status/q228h98xg9905ghc)](https://ci.appveyor.com/project/upta/pubsub)

An extremely light-weight, easy to use .Net pub/sub library 

### Breaking change in 4.0
In an effort to clean up some old bad habits, version 4.0 and beyond no longer has extension methods on object for Publish and Subscribe. The easiest migration solution would be to simply use those methods on the new Hub.Default static instance.

### The idea
* Provide the ability to do de-coupled communication without having to include some large (and often opinionated) framework when all you want is pubsub
* It should be portable so it can be used pretty much wherever
* Make it stupid simple to use with zero setup

### When is it useful?
In general, the publish/subscribe pattern is great when you want to communicate between pieces of your app without having those pieces being tightly dependent on each other.  You might find [this article on the subject](http://blog.mgechev.com/2013/04/24/why-to-use-publishsubscribe-in-javascript/) interesting (it talks specifically about javascript, but the concept applies)

A few cases where I have found this library useful (namely in mobile development):
* Persisting user settings to disk asynchronously
* Posting a message (i.e. Twitter) and using Subscribe to refresh lists, update counts, etc.
* Providing a nice, de-coupled communication medium between Activities and their child Fragments in a Xamarin.Android application.

There are lots of good applications for the publish/subscribe patterns.  Have a good one I didn't think of? [Let me know](https://github.com/upta/pubsub/issues)

### How to use it
First, add a using.
```c#
using PubSub;
```

Listen for stuff you might be interested in.

```c#
public class Page
{
    Hub hub = Hub.Default;
	
	public Page()
	{
		hub.Subscribe<Product>(this, product =>
		{
			// highly interesting things
		});
	}
}
```

Tell others that interesting things happened.

```c#
public class OtherPage
{
    Hub hub = Hub.Default;
	
	public void ProductPurchased()
	{
		hub.Publish( new Product() );
	}
}
```

Stop listening when you don't care anymore.

```c#
public class Page
{
	Hub hub = Hub.Default;
	
	public void WereDoneHere()
	{
		hub.Unsubscribe<Product>();
	}
}
```

### Some explanation
To keep things simple, yet flexible, PubSub PCL is implemented using core ideas:
* Different kinds of messages are delineated by CLR type.
	- This avoids the need to have a list of string constants (or just magic strings), enums, whatever to define what you want to listen for/send.
	- This gives us nice strongly-typed data that can be passed from our Publish methods to our Subscribe handlers (i.e. Product above)

### Get it on Nuget

	Install-Package PubSub

![nuget dialog image for PubSub PCL](http://i.imgur.com/jH6ONPg.png "Nuget dialog for PubSub PCL")

### Target Frameworks
* .Net Standard 1.1 / 2.0

### Currently supported platforms
* .Net Framwork 4.5 / 4.5.1
* Windows 8 / 8.1
* Windows Phone Silverlight 8 / 8.1
* Windows Phone 8.1 (WinRT)
* Silverlight 5
* Xamarin.Android
* Xamarin.iOS

### Questions? Thoughts?
Feel free to post stuff to the [issues](https://github.com/upta/pubsub/issues) page or hit me up on Twitter [@brianupta](https://twitter.com/brianupta)

