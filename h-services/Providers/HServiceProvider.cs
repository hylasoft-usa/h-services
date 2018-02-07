using System.Collections.Generic;
using System.Collections.ObjectModel;
using Hylasoft.Extensions;
using Hylasoft.Services.Interfaces;

namespace Hylasoft.Services.Providers
{
  public class HServiceProvider : IHServicesProvider
  {
    private readonly string _serviceName;
    private readonly ILogger _logger;
    private readonly Collection<IHService> _services; 

    public string ServiceName { get { return _serviceName; } }
    
    public ILogger Logger { get { return _logger; } }

    public Collection<IHService> Services { get { return _services; }}

    public HServiceProvider(string serviceName, ILogger logger, params IHService[] services)
      : this(serviceName, logger, (IEnumerable<IHService>) services)
    {
    }

    public HServiceProvider(string serviceName, ILogger logger, IEnumerable<IHService> services)
    {
      _serviceName = serviceName;
      _logger = logger;

      _services = services == null
        ? new Collection<IHService>()
        : services.ToCollection();
    }
  }
}
