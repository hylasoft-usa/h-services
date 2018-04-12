using System.ServiceProcess;
using Hylasoft.Services.Interfaces.Providers;
using Hylasoft.Services.Interfaces.Services;

namespace Hylasoft.Services.Service
{
  public class HServiceRoot : ServiceBase
  {
    private readonly IHServiceRootRunner _runner;

    protected IHServiceRootRunner Runner { get { return _runner; } }

    public HServiceRoot(IHServicesProvider provider) : this(new HServiceRootRunner(provider))
    {
    }

    public HServiceRoot(IHServiceRootRunner runner)
    {
      _runner = runner;
      ServiceName = Runner.ServiceName;
    }

    #region ServiceBase Implementation
    protected override void OnStart(string[] args)
    {
      Runner.OnStart(args);
    }

    protected override void OnStop()
    {
      Runner.OnStop();
    }

    protected override void OnPause()
    {
      Runner.OnPause();
    }

    protected override void OnContinue()
    {
      Runner.OnContinue();
    }
    #endregion
  }
}
