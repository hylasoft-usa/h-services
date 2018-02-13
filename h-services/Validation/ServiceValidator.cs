using System.Collections.ObjectModel;
using Hylasoft.Extensions;
using Hylasoft.Services.Interfaces;
using Hylasoft.Services.Types;

namespace Hylasoft.Services.Validation
{
	public class ServiceValidator : IServiceValidator
	{
	  private static readonly ServiceStatuses[] FailedTransitions =
	  {
      ServiceStatuses.Paused,
      ServiceStatuses.Started,
      ServiceStatuses.Starting,
      ServiceStatuses.Stopped,
      ServiceStatuses.Stopping,
      ServiceStatuses.Unknown
	  };

	  private static readonly ServiceStatuses[] StartingTransitions =
	  {
      ServiceStatuses.Paused,
		  ServiceStatuses.Stopped,
      ServiceStatuses.Failed,
		  ServiceStatuses.Unknown
	  };

	  private static readonly ServiceStatuses[] StoppingTransitions =
	  {
      ServiceStatuses.Paused,
		  ServiceStatuses.Started
	  };

		private static readonly ServiceStatuses[] StopTransitions =
		{
      ServiceStatuses.Stopping
		};

		private static readonly ServiceStatuses[] StartTransitions =
		{
      ServiceStatuses.Starting
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
        case ServiceStatuses.Failed:
			    statuses = FailedTransitions;
			    break;

        case ServiceStatuses.Starting:
			    statuses = StartingTransitions;
			    break;

        case ServiceStatuses.Stopping:
			    statuses = StoppingTransitions;
			    break;

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
