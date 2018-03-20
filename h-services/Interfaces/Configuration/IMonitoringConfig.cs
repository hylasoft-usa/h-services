namespace Hylasoft.Services.Interfaces.Configuration
{
	public interface IMonitoringConfig
	{
    int AbortTimeoutInSeconds { get; }

    int StartupTimeoutInSeconds { get; }

    int MonitorSleepInMilliseconds { get; }
	}
}
