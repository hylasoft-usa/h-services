using System.Reflection;

namespace Hylasoft.Services.Types
{
	public class ServiceMethodInfo<TAction>
	where TAction : class
	{
		public MethodInfo Method { get; private set; }

		public TAction Action { get; private set; }

		public ServiceMethodInfo(MethodInfo method, TAction action)
		{
			Action = action;
			Method = method;
		}

		public override string ToString()
		{
			return Method == null
			  ? base.ToString()
			  : Method.Name;
		}
	}
}
