using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace LicitAR.FileStorage
{
    public class BlobService
    {
        private readonly BlobServiceClient _blobServiceClient;
        private readonly string _containerName;

        public BlobService(string connectionString, string containerName)
        {
            _blobServiceClient = new BlobServiceClient(connectionString);
            _containerName = containerName;
        }

        public async Task<string> UploadFileAsync(Stream fileStream, string fileName, string contentType)
        {
            try
            {
                var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
                await containerClient.CreateIfNotExistsAsync(PublicAccessType.Blob);

                var blobClient = containerClient.GetBlobClient(fileName);
                var headers = new BlobHttpHeaders { ContentType = contentType };

                await blobClient.UploadAsync(fileStream, headers);

                return blobClient.Uri.ToString(); // URL del archivo subido
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al subir archivo: {ex.Message}");
            }
        }
    }
}
