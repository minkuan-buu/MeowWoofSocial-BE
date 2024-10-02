using Microsoft.AspNetCore.Http;

namespace MeowWoofSocial.Business.Services.CloudServices;

public interface ICloudStorage
{
    public Task<string> UploadFile(IFormFile file, string filePath);
}