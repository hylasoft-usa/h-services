namespace Hylasoft.Services.Interfaces
{
  public interface IHServiceRootRunner
  {
    void OnStart(string[] args);

    void OnStop();

    void OnPause();

    void OnContinue();

    string ServiceName { get; }
  }
}
