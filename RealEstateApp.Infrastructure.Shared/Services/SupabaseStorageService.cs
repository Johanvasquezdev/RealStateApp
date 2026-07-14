using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using RealEstateApp.Core.Application.Interfaces.Services;
using RealEstateApp.Core.Domain.Settings;

namespace RealEstateApp.Infrastructure.Shared.Services;

public class SupabaseStorageService : IFileStorageService
{
    private readonly SupabaseSettings _settings;
    private readonly Supabase.Client _client;

    public SupabaseStorageService(IOptions<SupabaseSettings> settings)
    {
        _settings = settings.Value;
        var options = new Supabase.SupabaseOptions { AutoConnectRealtime = false };
        _client = new Supabase.Client(_settings.Url, _settings.Key, options);
        _client.InitializeAsync().Wait();
    }

    public string UploadFile(IFormFile file, string directory, bool isEditMode = false, string imagePath = "")
    {
        if (isEditMode)
        {
            if (file == null)
            {
                return imagePath;
            }
        }

        // Generate unique filename
        Guid guid = Guid.NewGuid();
        FileInfo fileInfo = new FileInfo(file.FileName);
        string fileName = guid + fileInfo.Extension;
        string fullPath = $"{directory}/{fileName}";

        using var memoryStream = new MemoryStream();
        file.CopyTo(memoryStream);
        byte[] bytes = memoryStream.ToArray();

        var bucket = _client.Storage.From(_settings.BucketName);

        // Upload to Supabase asynchronously but we block since the interface is synchronous
        bucket.Upload(bytes, fullPath).Wait();

        if (isEditMode && !string.IsNullOrEmpty(imagePath))
        {
            try
            {
                string[] oldImageParts = imagePath.Split("/");
                string oldImageName = oldImageParts[^1];
                bucket.Remove(new List<string> { $"{directory}/{oldImageName}" }).Wait();
            }
            catch
            {
                // Ignore if it fails to delete the old image
            }
        }

        // Return public URL
        return bucket.GetPublicUrl(fullPath);
    }

    public void DeleteFile(string basePath, string directory)
    {
        if (string.IsNullOrEmpty(basePath)) return;

        try
        {
            var bucket = _client.Storage.From(_settings.BucketName);
            string[] oldImageParts = basePath.Split("/");
            string oldImageName = oldImageParts[^1];
            bucket.Remove(new List<string> { $"{directory}/{oldImageName}" }).Wait();
        }
        catch
        {
            // Ignore if file doesn't exist
        }
    }
}
