using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pollenalarm.Core.Models
{
    public class IInformation
    {
        string Id { get; set; }
        DateTime Date { get; set; }
        string Text { get; set; }
        string Language { get; set; }
    }
}
