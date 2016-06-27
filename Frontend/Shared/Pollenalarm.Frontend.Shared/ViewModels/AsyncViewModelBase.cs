using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pollenalarm.Frontend.Shared.ViewModels
{
	public class AsyncViewModelBase : ViewModelBase
	{

		#region Properties

		private bool _IsLoading = false;
		public bool IsLoading
		{
			get { return _IsLoading; }
			set { _IsLoading = value; RaisePropertyChanged(); }
		}

		private bool _IsLoaded = false;
		public bool IsLoaded
		{
			get { return _IsLoaded; }
			set { _IsLoaded = value; RaisePropertyChanged(); }
		}

		#endregion

		public AsyncViewModelBase()
		{

		}
	}
}