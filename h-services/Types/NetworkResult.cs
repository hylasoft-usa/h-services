using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using Hylasoft.Resolution;

namespace Hylasoft.Services.Types
{
  [Serializable]
  [XmlType(TypeName = "Result")]
  public class NetworkResult
  {
    private class InternalResult : Result
    {
      protected internal InternalResult(IEnumerable<ResultIssue> issues) : base(issues)
      {
      }
    }

    [XmlElement(ElementName = "Issues")]
    public NetworkResultIssue[] Issues { get; set; }

    public NetworkResult()
    {
      Issues = new NetworkResultIssue[0];
    }

    public static NetworkResult FromResult(Result result)
    {
      result = result ?? Result.Success;
      var issues = result
        .Select(NetworkResultIssue.FromIssue)
        .ToArray();

      return new NetworkResult
      {
        Issues = issues
      };
    }

    public Result ToResult()
    {
      var issues = Issues ?? new NetworkResultIssue[0];
      return new InternalResult(issues.Select(i => i.ToIssue()));
    }
  }
}
