using System;
using System.Collections.Generic;
using System.Linq;

using Foundation;
using UIKit;
using Plugin.Connectivity;


namespace Pollenalarm.Frontend.Forms.iOS
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the
    // User Interface of the application, as well as listening (and optionally responding) to
    // application events from iOS.
    [Register("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {
        //
        // This method is invoked when the application has loaded and is ready to run. In this
        // method you should instantiate the window, load the UI into it and then make the window
        // visible.
        //
        // You have 17 seconds to return from this method, or iOS will terminate your application.
        //
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            UINavigationBar.Appearance.BarTintColor = UIColor.FromRGB(33, 150, 243);
            UINavigationBar.Appearance.TintColor = UIColor.White;
            UINavigationBar.Appearance.TitleTextAttributes = new UIStringAttributes() { ForegroundColor = UIColor.White };

#if ENABLE_TEST_CLOUD
				Xamarin.Calabash.Start();
#endif

            global::Xamarin.Forms.Forms.Init();

            // Disable Visual Studio Mobile Center Distribute for DEBUG mode
            //Distribute.DontCheckForUpdatesInDebug();

            LoadApplication(new App());

            return base.FinishedLaunching(app, options);
        }
    }
}
