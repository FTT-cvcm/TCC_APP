using System.Threading.Tasks;

namespace Receitando.Services
{
	public interface IMicrophoneService
	{
		Task<bool> GetPermissionAsync();
		void OnRequestPermissionResult(bool isGranted);
	}
}
