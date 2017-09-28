using System;

namespace Pollenalarm.Frontend.Forms.Abstractions
{
    public interface IEnvironmentService
    {
        bool IsRunningInRealWorld();
    }
}
