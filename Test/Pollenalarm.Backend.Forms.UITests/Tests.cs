using System;
using System.IO;
using System.Linq;
using System.Threading;
using NUnit.Framework;
using Xamarin.UITest;
using Xamarin.UITest.Queries;

namespace Pollenalarm.Frontend.Forms.UITests
{
    [TestFixture(Platform.Android)]
    [TestFixture(Platform.iOS)]
    public class Tests
    {
        IApp app;
        Platform platform;

        public Tests(Platform platform)
        {
            this.platform = platform;
        }

        [SetUp]
        public void BeforeEachTest()
        {
            app = AppInitializer.StartApp(platform);
        }

        [Test]
        public void AppLaunches()
        {
            Thread.Sleep(1000);
            app.Screenshot("First screen.");
        }

        [Test]
        public void CanAddNewPlace()
        {
            app.Tap(x => x.Marked("AddButton"));
            app.Screenshot("Add Page should be shown.");
            app.WaitForElement("New Place");

            app.EnterText(x => x.Marked("NameEntry"), "Langenfeld");
            app.EnterText(x => x.Marked("ZipEntry"), "40764");
            Thread.Sleep(1000);
            app.Screenshot("Entries should have been made.");
            app.Tap(x => x.Marked("AddButton"));

            app.WaitForElement("Langenfeld");
            app.Screenshot("New place should have been added.");
        }
    }
}
