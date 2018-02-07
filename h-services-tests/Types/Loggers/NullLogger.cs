using Hylasoft.Resolution;
using Hylasoft.Services.Interfaces;

namespace Hylasoft.Services.Tests.Types.Loggers
{
  public class NullLogger : ILogger
  {
    public void Log(Result result)
    {
    }
  }
}
