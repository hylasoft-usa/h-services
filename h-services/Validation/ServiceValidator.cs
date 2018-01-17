using System.Collections.ObjectModel;
using Hylasoft.Extensions;
using Hylasoft.Services.Interfaces;
using Hylasoft.Services.Types;

namespace Hylasoft.Services.Validation
{
	public class ServiceValidator : IServiceValidator
	{
		private static readonly ServiceStatuses[] StopTransitions =
		{
		  ServiceStatuses.Paused,
		  ServiceStatuses.Started
		};

		private static readonly ServiceStatuses[] StartTransitions =
		{
		  ServiceStatuses.Paused,
		  ServiceStatuses.Stopped,
		  ServiceStatuses.Unknown
		};

		private static readonly ServiceStatuses[] PauseTransitions =
		{
		  ServiceStatuses.Started
		};

		private static readonly ServiceStatuses[] DefaultTransitions = new ServiceStatuses[0];

		public Collection<ServiceStatuses> GetStatusTransitions(ServiceStatuses targetStatus)
		{

			ServiceStatuses[] statuses;
			switch (targetStatus)
			{
				case ServiceStatuses.Paused:
					statuses = PauseTransitions;
					break;

				case ServiceStatuses.Stopped:
					statuses = StopTransitions;
					break;

				case ServiceStatuses.Started:
					statuses = StartTransitions;
					break;

				default:
					statuses = DefaultTransitions;
					break;
			}

			return statuses.ToCollection();
		}
	}
}
