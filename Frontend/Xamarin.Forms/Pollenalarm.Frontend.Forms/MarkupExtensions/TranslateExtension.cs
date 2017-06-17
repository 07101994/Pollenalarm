using GalaSoft.MvvmLight.Ioc;
using Pollenalarm.Frontend.Shared.Services;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Pollenalarm.Frontend.Forms.MarkupExtensions
{
    [ContentProperty("Text")]
    public class TranslateExtension : IMarkupExtension
    {
        private ILocalizationService _LocalizationService;

        public TranslateExtension()
        {
            _LocalizationService = SimpleIoc.Default.GetInstance<ILocalizationService>();
        }

        public string Text { get; set; }
        public object ProvideValue(IServiceProvider serviceProvider)
        {
            if (Text == null)
                return "";

            return _LocalizationService.GetString(Text);
        }
    }
}
