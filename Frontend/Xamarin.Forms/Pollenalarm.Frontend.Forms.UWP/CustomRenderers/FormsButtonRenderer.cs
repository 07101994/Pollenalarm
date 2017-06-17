using Pollenalarm.Frontend.Forms.UWP.CustomRenderers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Xamarin.Forms.Platform.UWP;

[assembly: ExportRenderer(typeof(Xamarin.Forms.Button), typeof(FormsButtonRenderer))]
namespace Pollenalarm.Frontend.Forms.UWP.CustomRenderers
{
    public class FormsButtonRenderer : ViewRenderer<Xamarin.Forms.Button, Button>
    {
        private Button nativeButton;

        protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.Button> e)
        {
            base.OnElementChanged(e);

            if (e.OldElement != null || this.Element == null)
                return;

            nativeButton = new Button();
            nativeButton.Content = e.NewElement.Text;
            nativeButton.Click += delegate { ((Xamarin.Forms.IButtonController)Element).SendClicked(); };

            SetNativeControl(nativeButton);
        }
    }
}
