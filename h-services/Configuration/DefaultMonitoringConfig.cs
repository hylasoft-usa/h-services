using Hylasoft.Services.Interfaces;

namespace Hylasoft.Services.Configuration
{
  public class DefaultMonitoringConfig : IMonitoringConfig
  {
    public int AbortTimeoutInSeconds { get { return 45; }}

    public int MonitorSleepInMilliseconds { get { return 500; } }
  }
}
