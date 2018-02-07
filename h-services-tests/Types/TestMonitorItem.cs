﻿namespace Hylasoft.Services.Tests.Types
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
  }
}