using Hylasoft.Logging;
using Hylasoft.Resolution;

namespace Hylasoft.Services.Tests.Types.Loggers
{
  public class NullLogger : IResultLogger
  {
    private readonly string _id;

    public NullLogger(string id)
    {
      _id = id;
    }


    public Result Log(Result result)
    {
      return Result.Success;
    }

    public Result LogSynchronous(Result result)
    {
      return Result.Success;
    }

    public string Id { get { return _id; } }
  }
}
