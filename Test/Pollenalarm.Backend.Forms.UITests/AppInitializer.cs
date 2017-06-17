using System;
using System.IO;
using System.Linq;
using Xamarin.UITest;
using Xamarin.UITest.Queries;

namespace Pollenalarm.Frontend.Forms.UITests
{
	public class AppInitializer
	{
		public static IApp StartApp(Platform platform)
		{
			if (platform == Platform.Android)
			{
				return ConfigureApp
					.Android
					//.ApkFile ("../../../Droid/bin/Debug/xamarinforms.apk")
					.StartApp();
			}

			return ConfigureApp
				.iOS
				//.AppBundle ("../../../iOS/bin/iPhoneSimulator/Debug/XamarinForms.iOS.app")
				.StartApp();
		}
	}
}
