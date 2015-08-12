using System;
using System.Linq;
using NUnit.Framework;
using Xamarin.UITest;
using Xamarin.UITest.Android;
using Xamarin.UITest.Queries;

namespace Pollenalarm.Droid.UITests
{
	[TestFixture]
	public class Tests
	{
		AndroidApp app;

		const string testCityName   = "Langenfeld";
		const string testCityZip    = "40764";
		const string testPollenName = "Ambrosia";

		[SetUp]
		public void BeforeEachTest()
		{
			// Start the application
            app = ConfigureApp.Android.StartApp();

			// Create a new city
			app.WaitForElement(x => x.Id("action_add_city"));
			app.Tap(x => x.Id("action_add_city"));
			app.EnterText(x => x.Id("addEditCityName"), testCityName);
			app.EnterText(x => x.Id("addEditCityZip"), testCityZip);
			app.Tap(x => x.Id("action_add_city_confirm"));

			// Check if city has been created successfully
			var results = app.Query(x => x.Text(testCityName));
			Assert.IsTrue(results.Any());
		}

		[TearDown]
		public void AfterEachTest()
		{
			// Start the application
			app = ConfigureApp.Android.StartApp();

			// Delete recently created city if still existing
			// Depending on your test configuration and environment, ConfigureApp.Android.StartApp(); sometimes
			// deployes a complete new package of the app. In this case, no cleanup work is needed.
			app.WaitForElement(x => x.Id("textView1"));
			var city = app.Query(x => x.Text(testCityName)).FirstOrDefault();
			if (city != null)
			{
				// Delete the recently created city
				app.Tap(x => x.Text(testCityName));
				app.WaitForElement(x => x.Class("OverflowMenuButton"));
				app.Tap(x => x.Class("OverflowMenuButton"));
				app.Tap(x => x.Class("TextView").Index(0)); // Edit button

				app.WaitForElement(x => x.Id("action_delete_city"), "Delete city button never appeared...");
				app.Tap(x => x.Id("action_delete_city"));
				app.Tap(x => x.Id("button1"));

				// Check if city has been deleted successfully
				var results = app.Query(x => x.Text(testCityName));
				Assert.IsFalse(results.Any());
			}
		}

		[Test]
		public void DiscoverPollutions()
		{
			app.WaitForElement(x => x.Text(testCityName));
			app.Screenshot("Given that I am on the city list");

			// Select city
			app.Tap(x => x.Text(testCityName));

			// Pollutions are downloading so wait for them to load
			app.WaitForElement(x => x.Id("pollutionItem"));
			app.Screenshot("Then I select a city");

			// Count if more than 2 pollutions fit on the screen
			var pollutions = app.Query(x => x.Id("pollutionItem"));
			app.Screenshot("And then the pollutions are shown");
			Assert.IsTrue(pollutions.Count() > 2);

			// Test scrolling
			app.ScrollDown();
			app.Screenshot("Now I scrolled down on pollution overview");

			// Check tab selection
			app.Tap(x => x.Class("ScrollingTabContainerView").Child().Child().Index(1));
			app.Screenshot("Then I tapped the 'Tomorrow' tab");

			// Check if tab selection worked
			var firstItemOfNew = app.Query(x => x.Id("pollutionItem").Class("FrameLayout").Class("LinearLayout").Class("TextView")).First();		
			Assert.AreEqual(firstItemOfNew.Text, testPollenName);
		}

		[Test]
		public void ShowPollenDetails()
		{
			app.WaitForElement(x => x.Text(testCityName));
			app.Screenshot("Given that I am on the city list");

			// Select city
			app.Tap(x => x.Text(testCityName));
			app.Screenshot("Then I select a city");

			// Pollutions are downloading so wait for them to load
			app.WaitForElement(x => x.Id("pollutionItem"));
			app.Screenshot("And then the pollutions are shown");

			// Select pollen item
			app.Tap(x => x.Text(testPollenName));
			app.WaitForElement(x => x.Id("pollenDetailsDescription"));
			app.Screenshot("Now I opended the pollen description");

			// Check if correct pollen item has been loaded
			var title = app.Query(x => x.Text("Ambrosia")).First();
			Assert.IsNotNull(title);

			// Check if description is set
			var description = app.Query(x => x.Id("pollenDetailsDescription")).First().Text;
			Assert.IsTrue(description.Length > 0);
		}
	}
}