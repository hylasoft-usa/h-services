using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Hylasoft.Extensions;
using Hylasoft.Resolution;
using Hylasoft.Services.Interfaces.Configuration;
using Hylasoft.Services.Interfaces.Monitoring;
using Hylasoft.Services.Interfaces.Validation;
using Hylasoft.Services.Monitoring.Base;
using Hylasoft.Services.Resources;
using Hylasoft.Services.Utilities;

namespace Hylasoft.Services.Monitoring
{
  public abstract class SetMonitor<TItem, TItemSpec> : HMonitor, ISetMonitor<TItem>
    where TItem : class
  {
    private readonly Dictionary<TItemSpec, TItem> _set;
    private readonly ItemSetComparer<TItemSpec> _comparer;

    protected IDictionary<TItemSpec, TItem> Set { get { return _set; } }

    protected ItemSetComparer<TItemSpec> Comparer { get { return _comparer; } }

    protected SetMonitor(IMonitoringConfig config = null, IServiceValidator serviceValidator = null) : base(config, serviceValidator)
    {
      _comparer = new ItemSetComparer<TItemSpec>(AreItemsEqual, GetItemSpecHash);
      _set = new Dictionary<TItemSpec, TItem>(Comparer);
    }

    #region ServiceBase Implementation
    protected override Result OnInitialize()
    {
      return UpdateSet();
    }

    protected override Result PerformServiceLoop()
    {
      return UpdateSet();
    }

    protected override Result CleanupOnShutdown()
    {
      return ClearSet();
    }

    public override string ServiceName
    {
      get { return MonitorName; }
    }

    protected override Result InitializeService()
    {
      return OnInitialize();
    }
    #endregion

    #region Abstract Methods
    public event EventHandler<TItem> ItemChanged;
    
    public event EventHandler<Collection<TItem>> ItemsAdded;

    public event EventHandler<Collection<TItem>> ItemsRemoved;

    protected abstract Result FetchSet(out IEnumerable<TItem> items);

    protected abstract bool HasItemChanged(TItem oldItem, TItem newItem);

    protected abstract TItemSpec GetSpecification(TItem item);

    protected abstract bool AreItemsEqual(TItemSpec a, TItemSpec b);

    protected abstract int GetItemSpecHash(TItemSpec spec);

    protected abstract string MonitorName { get; }
    #endregion

    #region Helper Methods
    protected Result UpdateSet()
    {
      Result update;
      IEnumerable<TItem> currentItems;
      if (!(update = FetchSet(out currentItems)) || currentItems == null)
        return update;

      var currentSet = currentItems.ToArray();
      var currentKeys = currentSet.Select(GetSpecification).ToArray();

      var newItemKeys = currentKeys.Where(key => !Set.ContainsKey(key));
      var existingItemKeys = currentKeys.Where(key => Set.ContainsKey(key));

      var newItems = currentSet.Where(item => newItemKeys.Contains(GetSpecification(item))).ToArray();
      var existingItems = currentSet.Where(item => existingItemKeys.Contains(GetSpecification(item))).ToArray();

      if (!(update += Result.Concat(AddItem, newItems)))
        return update;

      TriggerItemsAdded(newItems);

      if (!(update += Result.Concat(UpdateItem, existingItems)))
        return update;

      return update + ClearMissingItems(currentSet);
    }

    protected Result ClearSet()
    {
      Set.Clear();
      return Result.Success;
    }

    protected Result UpdateItem(TItem item)
    {
      // Don't bother with a null item.
      if (item == null)
        return Result.Success;

      var specification = GetSpecification(item);

      // Something went very wrong.
      if (!Set.ContainsKey(specification))
        return Result.SingleError(Errors.LogicalFallacy);

      var existingItem = Set[specification];
      // Update if existing item has changed.
      return HasItemChanged(existingItem, item)
        ? UpdateItem(specification, item)
        : Result.Success;
    }

    protected Result AddItem(TItem item)
    {
      try
      {
        var spec = GetSpecification(item);
        Set.Add(spec, item);

        // TODO: Consider adding a trace statement.
        return Result.Success;
      }
      catch (Exception e)
      {
        return Result.Error(e);
      }
    }

    protected Result UpdateItem(TItemSpec spec, TItem item)
    {
      try
      {
        Set[spec] = item;
        TriggerItemChanged(item);
        return Result.Success;
      }
      catch (Exception e)
      {
        return Result.Error(e);
      }
    }

    protected Result ClearMissingItems(IEnumerable<TItem> items)
    {
      try
      {
        var originalItems = Set.Values.ToCollection();
        var itemArr = items.ToArray();
        var currentItemSpecs = itemArr.Select(GetSpecification);
        var missingItemSpecs = Set.Keys.Except(currentItemSpecs).ToArray();

        if (!missingItemSpecs.Any())
          return Result.Success;

        foreach (var missingItemSpec in missingItemSpecs)
          Set.Remove(missingItemSpec);

        var missingItems = originalItems
          .Where(itms => missingItemSpecs
            .Any(mitms => AreItemsEqual(GetSpecification(itms), mitms))); 

        TriggerItemsRemoved(missingItems);
        return Result.Success;
      }
      catch (Exception e)
      {
        return Result.Error(e);
      }
    }

    private void TriggerItemChanged(TItem item)
    {
      if (ItemChanged != null)
        ItemChanged(this, item);
    }

    private void TriggerItemsAdded(IEnumerable<TItem> items)
    {
      Collection<TItem> addedItems;
      if (ItemsAdded != null && items != null && (addedItems = items.ToCollection()).Any())
        ItemsAdded(this, addedItems);
    }

    private void TriggerItemsRemoved(IEnumerable<TItem> items)
    {
      Collection<TItem> removedItems;
      if (ItemsRemoved != null && items != null && (removedItems = items.ToCollection()).Any())
        ItemsRemoved(this, removedItems);
    }
    #endregion
  }
}
