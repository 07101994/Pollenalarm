using GalaSoft.MvvmLight.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Pollenalarm.Frontend.Forms.Services
{
    public class NavigationService : INavigationService
    {
        private INavigation _Navigation;
        private Dictionary<string, Type> _Pages = new Dictionary<string, Type>();

        public NavigationService(INavigation navigation)
        {
            _Navigation = navigation;
        }

        public string CurrentPageKey
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public void GoBack()
        {

        }

        public void NavigateTo(string pageKey)
        {
            NavigateTo(pageKey, null);
        }

        public async void NavigateTo(string pageKey, object parameter)
        {
            if (!_Pages.ContainsKey(pageKey))
                throw new KeyNotFoundException();


            Type pageType = _Pages[pageKey];
            var x = Activator.CreateInstance(pageType);
            await _Navigation.PushAsync((Page)x);
        }

        public void Register(string pageKey, Type pageType)
        {
            if (_Pages.ContainsKey(pageKey))
                _Pages[pageKey] = pageType;
            else
                _Pages.Add(pageKey, pageType);
        }
    }
}
