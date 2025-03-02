using GIGANTECLIENTCORE.Context;
using Google.Cloud.Storage.V1;
using Microsoft.AspNetCore.Http;

namespace GIGANTECLIENTCORE.Utils;

public class AdminCurriculumMedia
{
    private readonly MyDbContext _context;
    private readonly string _bucketName = "giganteimages";
    private readonly string _folderPath = "ImageGigante/Curriculums/";

    public AdminCurriculumMedia(MyDbContext context)
    {
        _context = context;
    }

    public async Task<object> Upload(IFormFile file, string cedula)
    {
        try
        {
            if (file.Length > (10 * 1024 * 1024)) // 10MB limit
            {
                return new { success = false, message = "El archivo no puede sobrepasar los 10MB." };
            }

            string extension = Path.GetExtension(file.FileName).ToLower();
            if (extension != ".pdf")
            {
                return new { success = false, message = "Solo se permiten archivos PDF." };
            }

            string fileName = $"{_folderPath}{Guid.NewGuid()}_{SanitizeFileName(cedula)}.pdf";
            var storageClient = await StorageClient.CreateAsync();

            using (var stream = file.OpenReadStream())
            {
                await storageClient.UploadObjectAsync(
                    _bucketName,
                    fileName,
                    "application/pdf",
                    stream);
            }

            return new { success = true, fileName = fileName };
        }
        catch (Exception ex)
        {
            return new { success = false, message = $"Error al subir archivo: {ex.Message}" };
        }
    }

    private string SanitizeFileName(string fileName)
    {
        return Path.GetInvalidFileNameChars()
            .Aggregate(fileName, (current, c) => current.Replace(c, '_'));
    }
}