using GalaSoft.MvvmLight;

namespace Pollenalarm.Frontend.Shared.ViewModels
{
	public class AsyncViewModelBase : ViewModelBase
	{
		private bool _IsBusy = false;
		public bool IsBusy
		{
			get { return _IsBusy; }
			set { _IsBusy = value; RaisePropertyChanged(); }
		}

		private bool _IsLoaded = false;
		public bool IsLoaded
		{
			get { return _IsLoaded; }
			set { _IsLoaded = value; RaisePropertyChanged(); }
		}
	}
}