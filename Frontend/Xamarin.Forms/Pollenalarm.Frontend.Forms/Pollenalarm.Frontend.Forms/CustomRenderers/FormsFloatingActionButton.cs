using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace Pollenalarm.Frontend.Forms.CustomRenderers
{
    public class FormsFloatingActionButton : View
    {
        public static readonly BindableProperty ImageNameProperty = BindableProperty.Create<FormsFloatingActionButton, string>(p => p.ImageName, string.Empty);
        public string ImageName
        {
            get { return (string)GetValue(ImageNameProperty); }
            set { SetValue(ImageNameProperty, value); }
        }

        public static readonly BindableProperty CommandProperty = BindableProperty.Create<FormsFloatingActionButton, ICommand>(p => p.Command, null);
        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        public static readonly BindableProperty CommandParameterProperty = BindableProperty.Create<FormsFloatingActionButton, object>(p => p.CommandParameter, null);
        public object CommandParameter
        {
            get { return GetValue(CommandParameterProperty); }
            set { SetValue(CommandParameterProperty, value); }
        }

        public Action<object, EventArgs> Clicked { get; set; }
    }
}
