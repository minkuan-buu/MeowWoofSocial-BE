using Google.Cloud.Storage.V1;
using Microsoft.AspNetCore.Http;

namespace MeowWoofSocial.Business.Services.CloudServices;

public class CloudStorage : ICloudStorage
{
    private readonly StorageClient _storageClient;
    private const string BucketName = "meowwoofsocial-75790.appspot.com";
    public CloudStorage(StorageClient storageClient)
    {
        _storageClient = storageClient;
    }

    public async Task<List<string>> UploadFile(List<IFormFile> files, string filePath)
    {
        List<string> uploadUrl = new();
        
        foreach(var file in files)
        {
            using (var stream = new MemoryStream())
            {
                await file.CopyToAsync(stream);
                stream.Seek(0, SeekOrigin.Begin);
                var objectName = $"{filePath}/{file.FileName}";
                _storageClient.UploadObject(BucketName, objectName, file.ContentType, stream);
                uploadUrl.Add($"https://storage.googleapis.com/{BucketName}/{objectName}");
            };
        }
        return uploadUrl;
    }
}