using System;
using System.Windows.Input;
using Xamarin.Forms;

namespace Pollenalarm.Frontend.Forms.CustomControls
{
	public class HyperlinkLabel : Label
	{
		public static readonly BindableProperty UriProperty = BindableProperty.Create(nameof(Uri), typeof(string), typeof(HyperlinkLabel), null);
		public string Uri
		{
			get { return (string)GetValue(UriProperty); }
			set { SetValue(UriProperty, value); }
		}

        public static readonly BindableProperty CommandProperty = BindableProperty.Create(nameof(Command), typeof(ICommand), typeof(FormsFloatingActionButton), null);
        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        public static readonly BindableProperty CommandParameterProperty = BindableProperty.Create(nameof(CommandParameter), typeof(object), typeof(FormsFloatingActionButton), null);
        public object CommandParameter
        {
            get { return GetValue(CommandParameterProperty); }
            set { SetValue(CommandParameterProperty, value); }
        }

        public Action<object, EventArgs> Clicked { get; set; }
    

        public HyperlinkLabel()
		{
			// Set text color
			TextColor = Color.Accent;

			// Underlining is set by custom renderers
			// On Android and UWP only, as it is against the iOS design guidelines
            

			// Add interaction
			var tapGestureRecognizer = new TapGestureRecognizer();
			tapGestureRecognizer.Tapped += delegate
            {
                if (Uri != null)
                {
                    Device.OpenUri(new Uri(Uri));
                }
                else
                {
                    Clicked?.Invoke(this, null);
                    Command?.Execute(CommandParameter);
                }
            };

			GestureRecognizers.Add(tapGestureRecognizer);
		}
	}
}
