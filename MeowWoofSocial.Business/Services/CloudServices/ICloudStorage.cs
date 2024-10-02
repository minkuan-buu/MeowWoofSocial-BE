using Microsoft.AspNetCore.Http;

namespace MeowWoofSocial.Business.Services.CloudServices;

public interface ICloudStorage
{
    public Task<List<string>> UploadFile(List<IFormFile> files, string filePath);
}