using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Pollenalarm.Core.Models;
using Pollenalarm.Frontend.Shared.Models;
using Pollenalarm.Frontend.Shared.Services;

namespace Pollenalarm.Frontend.Shared.ViewModels
{
    public class PollenViewModel : AsyncViewModelBase
    {
        private IPollenService _PollenService;
        private ILocalizationService _LocalizationService;
        private SettingsService _SettingsService;

        public string _Name;
        public string Name
        {
            get { return _Name; }
            set { _Name = value; RaisePropertyChanged(); }
        }

        public string _Description;
        public string Description
        {
            get { return _Description; }
            set { _Description = value; RaisePropertyChanged(); }
        }

        public string _ImageName;
        public string ImageName
        {
            get { return _ImageName; }
            set { _ImageName = value; RaisePropertyChanged(); }
        }

        public string _ImageCredits;
        public string ImageCredits
        {
            get { return _ImageCredits; }
            set { _ImageCredits = value; RaisePropertyChanged(); }
        }

        public string _BloomTime;
        public string BloomTime
        {
            get { return _BloomTime; }
            set { _BloomTime = value; RaisePropertyChanged(); }
        }

        public string _ClinicalPollution;
        public string ClinicalPollution
        {
            get { return _ClinicalPollution; }
            set { _ClinicalPollution = value; RaisePropertyChanged(); }
        }

        public bool _IsSelected;
        public bool IsSelected
        {
            get { return _IsSelected; }
            set { _IsSelected = value; RaisePropertyChanged(); }
        }

        public PollenViewModel(IPollenService pollenService, ILocalizationService localizationService, SettingsService settingsService)
        {
            _PollenService = pollenService;
            _LocalizationService = localizationService;
            _SettingsService = settingsService;
        }

        public void Refresh()
        {
            IsBusy = true;
            IsLoaded = false;

            Name = _PollenService.CurrentPollen.Name;
            Description = _PollenService.CurrentPollen.Description;
            ImageName = _PollenService.CurrentPollen.ImageName;
            ImageCredits = _PollenService.CurrentPollen.ImageCredits;
            BloomTime = $"{_PollenService.CurrentPollen.BloomStart.ToString("MMMM")} {_LocalizationService.GetString("Until")} {_PollenService.CurrentPollen.BloomEnd.ToString("MMMM")}";
            IsSelected = _PollenService.CurrentPollen.IsSelected;

            switch (_PollenService.CurrentPollen.ClinicalPollution)
            {
                default:
                case 0:
                    ClinicalPollution = _LocalizationService.GetString("PollutionNameNone");
                    break;
                case 1:
                    ClinicalPollution = _LocalizationService.GetString("PollutionNameLow");
                    break;
                case 2:
                    ClinicalPollution = _LocalizationService.GetString("PollutionNameMedium");
                    break;
                case 3:
                    ClinicalPollution = _LocalizationService.GetString("PollutionNameStrong");
                    break;
                case 4:
                    ClinicalPollution = _LocalizationService.GetString("PollutionNameVeryStrong");
                    break;
            }

            IsBusy = false;
        }

        public async Task SaveChangesAsync()
        {
            IsBusy = true;

            _PollenService.CurrentPollen.IsSelected = IsSelected;
            await _PollenService.UpdatePollenAsync(_PollenService.CurrentPollen);

            IsBusy = false;
        }
    }
}
