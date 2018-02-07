using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Hylasoft.Extensions;

namespace Hylasoft.Services.Types
{
	public class ServiceItemSet<TServiceItem, TAction> : IEnumerable<TServiceItem>
		where TAction : class
		where TServiceItem : ServiceItem<TAction>
	{
		private readonly Collection<TServiceItem> _items;

		public Collection<TServiceItem> Items
		{
			get { return _items; }
		}

		public ServiceItemSet(IEnumerable<TServiceItem> items)
		{
		  _items = items == null
		    ? new Collection<TServiceItem>()
		    : items.ToCollection();
		}

		public Collection<TServiceItem> FailedItems
		{
			get
			{
				return Items.Where(item => item != null
				                           && item.State != ServiceItemStates.Pending
				                           && item.State != ServiceItemStates.Halted
				                           && item.State != ServiceItemStates.Running)
					.ToCollection();
			}
		}

		public Collection<TServiceItem> PendingItems
		{
			get { return Items.Where(item => item != null && item.State == ServiceItemStates.Pending).ToCollection(); }
		}

		public IEnumerator<TServiceItem> GetEnumerator()
		{
			return Items.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}
