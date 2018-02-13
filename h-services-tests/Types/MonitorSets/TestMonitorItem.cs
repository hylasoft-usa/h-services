namespace Hylasoft.Services.Tests.Types.MonitorSets
{
  public class TestMonitorItem
  {
    public int Key { get; set; }

    public string Value { get; set; }

    public TestMonitorItem()
    {
    }

    public TestMonitorItem(int key, string value)
    {
      Key = key;
      Value = value;
    }

    public override string ToString()
    {
      return string.Format("[{0}] '{1}'", Key, Value);
    }
  }
}
