namespace Hylasoft.Services.Interfaces
{
	public interface IMonitoringConfig
	{
    int AbortTimeoutInSeconds { get; }

    int StartupTimeoutInSeconds { get; }

    int MonitorSleepInMilliseconds { get; }
	}
}
