using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Views;
using Pollenalarm.Core;
using Pollenalarm.Core.Models;
using Pollenalarm.Frontend.Shared.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pollenalarm.Frontend.Shared.ViewModels
{
    public class MainViewModel : AsyncViewModelBase
    {
        private PollenService _PollenService;

        private ObservableCollection<Place> _Places;
        public ObservableCollection<Place> Places
        {
            get { return _Places; }
            set { _Places = value; RaisePropertyChanged(); }
        }

        private RelayCommand _RefreshCommand;
        public RelayCommand RefreshCommand
        {
            get
            {
                return _RefreshCommand ?? (_RefreshCommand = new RelayCommand(async () =>
                {
                    await RefreshAsync();
                }));
            }
        }


        public MainViewModel(PollenService pollenService)
        {
            _PollenService = new PollenService();

            Places = new ObservableCollection<Place>();
        }

        public async Task RefreshAsync()
        {
            IsLoading = true;

            // ...

            IsLoading = false;
            IsLoaded = true;
        }
    }
}
