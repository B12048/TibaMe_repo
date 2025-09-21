using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;

namespace BoardGameFontier.Services
{
    public class UploadImageService: IUploadImageService
    {
        private const string BUCKET_NAME = "bgf-image-storage";
        private const string ACCESS_Key = "bcecff4efb184d8b77e156f73aa8e186";
        private const string SECRET_Key = "cc14c3bd2211ff96a47895287e0d47a8f386fd81b398f3a441c91f9a7cc92551";
        private const string SERVICE_URI = "https://11411b602ac2c8023e3dce7938c9bcea.r2.cloudflarestorage.com";
        private const string IMAGE_DOMAIN = "https://r2-image-proxy.mariomario258.workers.dev";
        private readonly IAmazonS3 _s3Client;
        private readonly ILogger<UploadImageService> _logger;

        public UploadImageService(ILogger<UploadImageService> logger)
        {
            _logger = logger;

            var credentials = new BasicAWSCredentials(ACCESS_Key, SECRET_Key);
            _s3Client = new AmazonS3Client(credentials, new AmazonS3Config
            {
                ServiceURL = SERVICE_URI,
                ForcePathStyle = true,
                //Ai說要寫，但其實下面這一行不能寫
                //RegionEndpoint = Amazon.RegionEndpoint.USEast1  
            });
        }

        public async Task<string> UploadImage(IFormFile file)
        {
            if (file == null || file.Length == 0) throw new ArgumentException("沒有上傳的檔案");

            var fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
            var extension = Path.GetExtension(file.FileName).ToLower();

            if (!file.ContentType.StartsWith("image/") || !allowedExtensions.Contains(extension))
            {
                throw new ArgumentException("只允許上傳圖片檔案（JPG、PNG、GIF、WEBP）");
            }

            using (var stream = file.OpenReadStream())
            {
                var putRequest = new PutObjectRequest
                {
                    BucketName = BUCKET_NAME,
                    Key = fileName,
                    InputStream = stream,
                    ContentType = file.ContentType,
                    AutoResetStreamPosition = false,
                    DisablePayloadSigning = true,  //記得加上這兩個以避免SHA認證問題
                    DisableDefaultChecksumValidation = true //記得加上這兩個
                };

                await _s3Client.PutObjectAsync(putRequest);
            }
            return $"{IMAGE_DOMAIN}/{fileName}";
        }
    }
}
