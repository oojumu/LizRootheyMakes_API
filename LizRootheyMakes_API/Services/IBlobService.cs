namespace LizRootheyMakes_API.Services
{
	public interface IBlobService
	{

		Task<bool> DeleteBlob(string blobName, string containerName);
		Task<string> GetBlob(string blobName, string containerName);		
		Task<string> UpdateBlob(string blobName, string containerName, IFormFile file);

	}
}
