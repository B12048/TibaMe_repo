namespace BoardGameFontier.Services
{
    public interface IUploadImageService
    {
        Task<string> UploadImage(IFormFile file);
    }
}
