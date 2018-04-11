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
