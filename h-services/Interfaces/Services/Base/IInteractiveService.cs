using System;
using System.Collections.Generic;
using Hylasoft.Resolution;

namespace Hylasoft.Services.Interfaces.Services.Base
{
  public interface IInteractiveService
  {
    Result SetServices(IEnumerable<IHService> services);
  }
}
