using Microsoft.AspNetCore.Http;

namespace RealEstateApp.Core.Application.Interfaces.Services;

public interface IFileStorageService
{
    string UploadFile(IFormFile file, string directory, bool isEditMode = false, string imagePath = "");
    void DeleteFile(string basePath, string directory);
}
