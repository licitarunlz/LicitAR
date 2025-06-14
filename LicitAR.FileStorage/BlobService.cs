using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace LicitAR.FileStorage
{
    public interface IBlobStorageService
    {
        Task<string> SubirArchivoAsync(IFormFile archivo);
    }

    public class BlobStorageService : IBlobStorageService
    {
        private readonly string _connectionString;
        private readonly string _containerName;

        public BlobStorageService(IConfiguration config)
        {
            _connectionString = config["AzureBlobStorage:ConnectionString"];
            _containerName = config["AzureBlobStorage:ContainerName"];
        }

        public async Task<string> SubirArchivoAsync(IFormFile archivo)
        {
            try
            {
                var blobClient = new BlobContainerClient(_connectionString, _containerName);
                await blobClient.CreateIfNotExistsAsync();

                var nombreUnico = Guid.NewGuid() + Path.GetExtension(archivo.FileName);
                var blob = blobClient.GetBlobClient(nombreUnico);

                using (var stream = archivo.OpenReadStream())
                {
                    await blob.UploadAsync(stream, true);
                }

                return blob.Uri.ToString(); // Esto lo guardás en la tabla
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al subir archivo: {ex.Message}");
            }
        }
    }
}
