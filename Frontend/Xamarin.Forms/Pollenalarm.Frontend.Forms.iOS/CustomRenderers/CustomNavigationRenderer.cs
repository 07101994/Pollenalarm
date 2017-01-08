
using System;
using System.Collections.Generic;
using Pollenalarm.Frontend.Forms.iOS.CustomRenderers;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

//[assembly: ExportRenderer(typeof(NavigationPage), typeof(CustomNavigationRenderer))]
namespace Pollenalarm.Frontend.Forms.iOS.CustomRenderers
{
	public class CustomNavigationRenderer : NavigationRenderer
	{

		public override void PushViewController(UIKit.UIViewController viewController, bool animated)
		{
			base.PushViewController(viewController, animated);

			var rightList = new List<UIBarButtonItem>();
			var leftList = new List<UIBarButtonItem>();

			if (TopViewController.NavigationItem.RightBarButtonItems.Length == 1)
				rightList.Add(TopViewController.NavigationItem.RightBarButtonItems[0]);
			else
			{
				var count = 0;
				foreach (var item in TopViewController.NavigationItem.RightBarButtonItems)
				{
					if (count % 2 != 0)
						rightList.Add(item);
					else
						leftList.Add(item);

					count++;
				}
			}

			TopViewController.NavigationItem.LeftBarButtonItems = leftList.ToArray();
			TopViewController.NavigationItem.RightBarButtonItems = rightList.ToArray();
		}
	}
}