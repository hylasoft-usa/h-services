using System;
using System.Collections.Generic;

namespace Hylasoft.Services.Utilities
{
  public class ItemSetComparer<TSpec> : IEqualityComparer<TSpec>
  {
    private readonly Func<TSpec, TSpec, bool> _equality;
    private readonly Func<TSpec, int> _hash;

    protected Func<TSpec, TSpec, bool> Equality { get { return _equality; } }

    protected Func<TSpec, int> Hash { get { return _hash; } }
 
    public ItemSetComparer(Func<TSpec, TSpec, bool> equality, Func<TSpec, int> hash)
    {
      _equality = equality;
      _hash = hash;
    }

    public bool Equals(TSpec x, TSpec y)
    {
      return Equality(x, y);
    }

    public int GetHashCode(TSpec obj)
    {
      return Hash(obj);
    }
  }
}
