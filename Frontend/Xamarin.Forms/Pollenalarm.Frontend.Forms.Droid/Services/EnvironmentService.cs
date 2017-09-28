using Android.OS;
using Pollenalarm.Frontend.Forms.Abstractions;
using Pollenalarm.Frontend.Forms.Droid.Services;

[assembly: Xamarin.Forms.Dependency(typeof(EnvironmentService))]
namespace Pollenalarm.Frontend.Forms.Droid.Services
{
    public class EnvironmentService : IEnvironmentService
    {
        public bool IsRunningInRealWorld()
        {
#if DEBUG
            return false;
#endif

            if (Build.Fingerprint.Contains("vbox") ||
                Build.Fingerprint.Contains("generic") ||
                System.Environment.GetEnvironmentVariable("XAMARIN_TEST_CLOUD") != null)
            {
                return false;
            }

            return true;

            return true;
        }
    }
}
