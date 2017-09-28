using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Pollenalarm.Frontend.Forms.iOS.CustomRenderers;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

//[assembly: ExportRenderer(typeof(NavigationPage), typeof(CustomNavigationRenderer))]
namespace Pollenalarm.Frontend.Forms.iOS.CustomRenderers
{
    /// <summary>
    /// Custom navigation renderer to move the 'Settings' button to the left on iOS
    /// </summary>
    public class CustomNavigationRenderer : NavigationRenderer
    {
        public override void PushViewController(UIKit.UIViewController viewController, bool animated)
        {
            base.PushViewController(viewController, animated);

            var rightList = new List<UIBarButtonItem>();
            var leftList = new List<UIBarButtonItem>();

            foreach (var item in TopViewController.NavigationItem.RightBarButtonItems)
            {
                // Get original Xamarin.Forms's ToolBar item through reflection
                var controller = (ToolbarItem)item.GetType().GetProperty("Controller",
                   BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).GetValue(item);

                // Check if it is a settings icon by comparing its AutomationId
                if (controller.AutomationId != null && controller.AutomationId.ToLower().Contains("settings"))
                    leftList.Add(item);
                else
                    rightList.Add(item);
            }

            if (leftList.Any())
                TopViewController.NavigationItem.LeftBarButtonItems = leftList.ToArray();

            if (rightList.Any())
                TopViewController.NavigationItem.RightBarButtonItems = rightList.ToArray();
        }
    }
}