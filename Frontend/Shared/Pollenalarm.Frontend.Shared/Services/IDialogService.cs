using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pollenalarm.Frontend.Shared.Services
{
    public interface IDialogService
    {
        Task<bool> DisplayConfirmationAsync(string title, string message, string confirm, string cancel);
    }
}
