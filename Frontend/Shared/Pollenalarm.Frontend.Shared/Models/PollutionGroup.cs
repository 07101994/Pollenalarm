using System;
using System.Collections.ObjectModel;
namespace Pollenalarm.Frontend.Shared.Models
{
    public class PollutionGroup : ObservableCollection<Pollution>
    {
        public string Title { get; set; }

        public PollutionGroup(string title)
        {
            this.Title = title;
        }

        public static ObservableCollection<Pollution> All { get; private set; }

    }
}
