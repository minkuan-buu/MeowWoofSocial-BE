using Google.Apis.Storage.v1;
using Google.Cloud.Storage.V1;
using Microsoft.AspNetCore.Http;
using Microsoft.Identity.Client.Extensions.Msal;
using System.Text.RegularExpressions;
using System.Text;
using MeowWoofSocial.Business.ApplicationMiddleware;

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
                var objectName = $"{filePath}/{TextConvert.ConvertToUnSign(file.FileName)}";
                _storageClient.UploadObject(BucketName, objectName, file.ContentType, stream);
                uploadUrl.Add($"https://storage.googleapis.com/{BucketName}/{objectName}");
            };
        }
        return uploadUrl;
    }

    public async Task<string> UploadSingleFile(IFormFile file, string filePath)
    {
        using (var stream = new MemoryStream())
        {
            await file.CopyToAsync(stream);
            stream.Seek(0, SeekOrigin.Begin);
            var objectName = $"{filePath}/{TextConvert.ConvertToUnSign(file.FileName)}";
            _storageClient.UploadObject(BucketName, objectName, file.ContentType, stream);
            return $"https://storage.googleapis.com/{BucketName}/{objectName}";
        }
    }

    public async Task DeleteFilesInPathAsync(string path)
    {
        try
        {
            var objects = _storageClient.ListObjects(BucketName, prefix: path);
            
            foreach (var obj in objects)
            {
                await _storageClient.DeleteObjectAsync(BucketName, obj.Name);
                Console.WriteLine($"File '{obj.Name}' deleted successfully.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred while deleting files: {ex.Message}");
        }
    }
}