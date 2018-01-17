using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Hylasoft.Resolution;
using Hylasoft.Services.Interfaces;
using Hylasoft.Services.Types;

namespace Hylasoft.Services.Monitoring
{
	public abstract class ServiceMonitor<TServiceItemSet, TServiceItem, TAction, TConfig> : IServiceMonitor<TServiceItemSet, TServiceItem, TAction>
		where TAction : class
		where TServiceItem : ServiceItem<TAction>
		where TServiceItemSet : ServiceItemSet<TServiceItem, TAction>
		where TConfig : IMonitoringConfig, new()
	{
		private DateTime _lastClean;
		private readonly TConfig _config;
		private readonly TimeSpan _cleanInterval;

		private readonly List<Thread> _runningThreads;
		private Thread _scheduleThread;
		private bool _keepAlive;
		private bool _isAlive;

		protected List<Thread> RunningThreads { get { return _runningThreads; } }

		protected TConfig Config { get { return _config; } }

		public Result Errors { get; private set; }

		protected TServiceItemSet CurrentSet { get; private set; }

		protected ServiceMonitor(TConfig config)
		{
			_config = config;
			_runningThreads = new List<Thread>();
			_isAlive = false;
			_keepAlive = false;

			_cleanInterval = new TimeSpan(0, 0, 0, Config.ThreadCleanupIntervalInSeconds);
			Errors = Result.Success;
		}

		public virtual Result Start(TServiceItemSet set)
		{
			try
			{
				_keepAlive = true;
				CurrentSet = set;
				_scheduleThread = new Thread(RunMonitorThread);
				_scheduleThread.Start();

				return Result.Success;
			}
			catch (Exception e)
			{
				return Result.Error(e);
			}
		}

		public virtual Result Stop()
		{
			// TODO: Add Timeout
			try
			{
				_keepAlive = false;
				while (_isAlive) Thread.Sleep(100);

				_scheduleThread.Join();
				return Result.Success;
			}
			catch (Exception e)
			{
				return Result.Error(e);
			}
		}

		public event EventHandler<TServiceItem> EventTriggered;

		protected abstract void RunMonitorLoop();

		protected abstract int SleepSeconds { get; }

		private void RunMonitorThread()
		{
			try
			{
				_isAlive = true;
				while (_keepAlive)
				{
					RunMonitorLoop();
					CheckThreads();
					Thread.Sleep(SleepSeconds * 1000);
				}
			}
			catch (Exception e)
			{
				Errors += Result.Error(e);
			}
			finally
			{
				_isAlive = false;
			}
		}

		private void CheckThreads()
		{
			var time = DateTime.Now;
			var span = time - _lastClean;
			if (span < _cleanInterval)
				return;

			_lastClean = time;
			CleanThreads();
		}

		private void CleanThreads()
		{
			// The where clause must be enumerated, otherwise this will result in modifying
			// the source of the enumeration.
			foreach (var thread in RunningThreads.Where(IsInactive).ToArray())
				CleanThread(thread);
		}

		private void CleanThread(Thread thread)
		{
			if (thread == null)
				return;

			thread.Abort();
			RunningThreads.Remove(thread);
		}

		protected virtual bool IsInactive(Thread thread)
		{
			return thread == null
				   || !thread.IsAlive;
		}

		protected void TriggerMonitoredEvent(TServiceItem item)
		{
			if (EventTriggered != null)
				EventTriggered(this, item);
		}
	}
}
