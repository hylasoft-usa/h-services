namespace Hylasoft.Services.Interfaces
{
	public interface IMonitoringConfig
	{
		int ThreadCleanupIntervalInSeconds { get; }

    int AbortTimeoutInSeconds { get; }

    int MonitorSleepInMilliseconds { get; }
	}
}
