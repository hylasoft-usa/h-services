using System;
using Hylasoft.Resolution;

namespace Hylasoft.Services.Interfaces.Services.Base
{
  public interface IHService : IServiceStatusElement
  {
    event EventHandler<Result> ErrorOccured;
  }
}
