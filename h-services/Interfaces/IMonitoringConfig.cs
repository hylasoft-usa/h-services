namespace Hylasoft.Services.Interfaces
{
	public interface IMonitoringConfig
	{
    int AbortTimeoutInSeconds { get; }

    int MonitorSleepInMilliseconds { get; }
	}
}
