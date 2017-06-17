using MvvmHelpers;
using System;
using System.Collections.ObjectModel;
namespace Pollenalarm.Frontend.Shared.Models
{
    public class PollutionGroup : ObservableRangeCollection<Pollution>
    {
        public string Title { get; set; }

        public PollutionGroup(string title)
        {
            this.Title = title;
        }

        public static ObservableRangeCollection<Pollution> All { get; private set; }

    }
}
