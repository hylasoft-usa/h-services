using System;
using Hylasoft.Resolution;
using Hylasoft.Services.Types;

namespace Hylasoft.Services.Interfaces
{
  public interface IHService : IServiceStatusElement
  {
    event EventHandler<Result> ErrorOccured;
  }
}
