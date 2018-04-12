using System;
using System.Xml.Serialization;
using Hylasoft.Resolution;

namespace Hylasoft.Services.Types
{
  [Serializable]
  [XmlType(TypeName = "ServiceStatusInfo")]
  public class ServiceStatusInformation
  {
    [XmlElement(ElementName = "ServiceName")]
    public string ServiceName { get; set; }

    [XmlElement(ElementName = "PriorStatus")]
    public ServiceStatuses PriorStatus { get; set; }

    [XmlElement(ElementName = "CurrentStatus")]
    public ServiceStatuses CurrentStatus { get; set; }

    [XmlElement(ElementName = "FailedAttempts")]
    public int FailedAttempts { get; set; }

    [XmlElement(ElementName = "LastResult")]
    public NetworkResult LastResult { get; set; }

    [XmlElement(ElementName = "LastAttempt")]
    public DateTime? LastAttempt { get; set; }

    [XmlElement(ElementName = "Revivals")]
    public int Revivals { get; set; }

    [XmlIgnore]
    public TimeSpan? CurrentWaitDuration { get; private set; }

    [XmlElement(ElementName = "CurrentWait")]
    public long? CurrentWaitTicks
    {
      get
      {
        return CurrentWaitDuration == null
          ? (long?) null
          : CurrentWaitDuration.Value.Ticks;
      }

      set
      {
        if (value != null)
          CurrentWaitDuration = new TimeSpan(value.Value);
      }
    }

    public ServiceStatusInformation()
    {
      Revivals = 0;
      FailedAttempts = 0;
      LastResult = Result.Success;
      PriorStatus = CurrentStatus = ServiceStatuses.Unknown;
      LastAttempt = DateTime.Now;
    }

    public ServiceStatusInformation(string name)
      : this()
    {
      ServiceName = name;
    }
  }
}
