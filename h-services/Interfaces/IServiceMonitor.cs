using System;
using Hylasoft.Resolution;
using Hylasoft.Services.Types;

namespace Hylasoft.Services.Interfaces
{
	public interface IServiceMonitor<in TServiceItemSet, TServiceItem, TAction>
		where TAction : class
		where TServiceItem : ServiceItem<TAction>
		where TServiceItemSet : ServiceItemSet<TServiceItem, TAction>
	{
		Result Start(TServiceItemSet set);

		Result Stop();

		event EventHandler<TServiceItem> EventTriggered;
	}
}
