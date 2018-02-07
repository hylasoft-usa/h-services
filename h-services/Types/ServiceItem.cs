using System;
using System.Threading;
using Hylasoft.Resolution;

namespace Hylasoft.Services.Types
{
	public abstract class ServiceItem<TAction>
	where TAction : class
	{
		private DateTime _lastRun;
		private ServiceItemStates _state;
		private readonly TAction _itemAction;
		private readonly string _methodName;

		public ServiceItemStates State { get { return _state; } }

		public DateTime LastRun { get { return _lastRun; } }

		protected TAction ItemAction { get { return _itemAction; } }

		protected Thread RunThread { get; private set; }

		public Result Errors { get; private set; }

		public string MethodName { get { return _methodName; } }

		protected ServiceItem(TAction itemAction, string methodName)
		{
			_itemAction = itemAction;
			_methodName = methodName;
			_lastRun = DateTime.Now;
			_state = ServiceItemStates.Pending;
			Errors = Result.Success;
		}

		public Thread Run(object args)
		{
			if (State != ServiceItemStates.Pending)
				return RunThread;

			RunThread = new Thread(RunItemMethod(args));
			_state = ServiceItemStates.Running;
			RunThread.Start();

			_lastRun = DateTime.Now;
			return RunThread;
		}

		protected abstract Action ToAction(TAction action, object args);

		protected virtual ThreadStart RunItemMethod(object args)
		{
			return () =>
			{
				try
				{
					ToAction(ItemAction, args)();
					_state = ServiceItemStates.Pending;
				}
				catch (Exception e)
				{
					Errors += Result.Error(e);
					_state = ServiceItemStates.Failed;
				}
			};
		}

	  protected Action PerformActionOnArgs<TArg>(Action<TArg> action, object args)
      where TArg: class
	  {
      var changedSet = args as TArg;
      return changedSet == null
        ? (Action)DoNothing
        : () => action(changedSet); 
	  }
    
	  protected Action PerformNothing()
	  {
	    return DoNothing;
	  }

    protected void DoNothing()
    {
    }
	}
}
