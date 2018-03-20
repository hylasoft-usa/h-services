using System;
using Hylasoft.Resolution;

namespace Hylasoft.Services.Interfaces.Services
{
  public interface IHService : IServiceStatusElement
  {
    event EventHandler<Result> ErrorOccured;
  }
}
