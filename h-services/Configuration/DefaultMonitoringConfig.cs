using Hylasoft.Services.Interfaces.Configuration;

namespace Hylasoft.Services.Configuration
{
  public class DefaultMonitoringConfig : IMonitoringConfig
  {
    public int AbortTimeoutInSeconds { get { return 45; }}

    public int StartupTimeoutInSeconds { get { return 15; } }

    public int MonitorSleepInMilliseconds { get { return 500; } }
  }
}
