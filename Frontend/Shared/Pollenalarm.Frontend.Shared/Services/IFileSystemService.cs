using System.Threading.Tasks;

namespace Pollenalarm.Frontend.Shared.Services
{
	public interface IFileSystemService
	{
		Task SaveObjectToFileAsync(string fileName, object content);
		Task<T> ReadObjectFromFileAsync<T>(string fileName);
	}
}
