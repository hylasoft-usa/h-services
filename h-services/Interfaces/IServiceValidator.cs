using System.Collections.ObjectModel;
using Hylasoft.Services.Types;

namespace Hylasoft.Services.Interfaces
{
	public interface IServiceValidator
	{
		Collection<ServiceStatuses> GetStatusTransitions(ServiceStatuses targetStatus);
	}
}
