using System;
using System.Xml.Serialization;
using Hylasoft.Resolution;

namespace Hylasoft.Services.Types
{
  [Serializable]
  [XmlType(TypeName = "Issue")]
  public class NetworkResultIssue
  {
    private class IssueInternal : ResultIssue
    {
      protected internal IssueInternal(string message, ResultIssueLevels level, long issueCode = 0) : base(message, level, issueCode)
      {
      }
    }

    [XmlElement(ElementName = "Level")]
    public ResultIssueLevels Level { get; set; }

    [XmlElement(ElementName = "IssueCode")]
    public long IssueCode { get; set; }

    [XmlElement(ElementName = "Message")]
    public string Message { get; set; }

    public ResultIssue ToIssue()
    {
      return new IssueInternal(Message, Level, IssueCode);
    }

    public static NetworkResultIssue FromIssue(ResultIssue issue)
    {
      return new NetworkResultIssue
      {
        Level = issue.Level,
        Message = issue.Message,
        IssueCode = issue.IssueCode
      };
    }
  }
}
