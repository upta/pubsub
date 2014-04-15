#PubSub PCL

An extremely light-weight, easy to use PCL pub/sub library

###The idea
* Provide the ability to do de-coupled communitcation without having to include some large (and often opinionated) framework when all you want is pubsub
* It should be portable so it can be used pretty much wherever
* Make it stupid simple to use with zero setup

###Get it on Nuget

	Install-Package PubSub

###How to use it
First, add a using.

	using PubSub;

Listen for stuff you might be interested in.

	public class Page
	{
		public Page()
		{
			this.Subscribe<Product>( product =>
			{
				// highly interesting things
			});
		}
	}
	
Tell others that interesting things happened.

	public class OtherPage
	{
		public void ProductPurchased()
		{
			this.Publish( new Product() );
		}
	}
	
Stop listening when you don't care anymore.

	public class Page
	{
		public void WereDoneHere()
		{
			this.Unsubscribe<Product>();
		}
	}
	
###Some explanation
To keep things simple, yet flexible, PubSub PCL is implemented using two core ideas:
* Different kinds of messages are delineated by CLR type.
	- This avoids the need to have a list of string constants (or just magic strings), enums, whatever to define what you want to listen for/send.
	- This gives us nice strongly-typed data that can be passed from our Publish methods to our Subscribe handlers (i.e. Product above)
* All interaction with the PubSub system is done through extention methods on System.Object (e.g. Publish, Subscribe and Unsubscribe)
	- This allows for a zero-setup system, you can simply Subscribe on any object and Publish away

###Questions? Thoughts?
Feel free to post stuff to the [issues](https://github.com/upta/pubsub/issues) page or hit me up on Twitter [@brianupta](https://twitter.com/brianupta)
