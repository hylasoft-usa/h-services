namespace Hylasoft.Services.Interfaces.Services
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
