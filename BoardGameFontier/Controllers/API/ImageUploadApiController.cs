using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using BoardGameFontier.Extensions;
using BoardGameFontier.Repostiory;
using BoardGameFontier.Repostiory.Entity;
using BoardGameFontier.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Threading.Tasks;

[Route("api/[controller]")]
[ApiController]
public class ImageUploadApiController : ControllerBase
{
    private readonly ApplicationDbContext _db;
    private readonly ICommunityService _communityService;
    private readonly IUploadImageService _uploadImageService;
    private readonly ILogger<ImageUploadApiController> _logger;
    
    public ImageUploadApiController(ApplicationDbContext db, ICommunityService communityService, IUploadImageService uploadImageService, ILogger<ImageUploadApiController> logger)
    {
        _db = db;
        _communityService = communityService;
        _uploadImageService = uploadImageService;
        _logger = logger;
    }

    [HttpPost("upload")]
    public async Task<IActionResult> UploadImage(IFormFile file)
    {
        try
        {
            string publicUrl = await _uploadImageService.UploadImage(file);
            string userName = User.Identity.Name;
            var user = _db.UserProfiles
               .WhereActive()
               .FirstOrDefault(u => u.UserName == userName);
            if (user == null)
                return NotFound("找不到使用者");

            user.ProfilePictureUrl = publicUrl;
            _db.UserProfiles.Update(user);
            _db.SaveChanges();

            // ✅ 清除用戶快取，確保側邊欄能立即顯示新頭像
            await _communityService.ClearUserCacheAsync(user.Id);

            return RedirectToAction("IndexMember", "Member");
        }
        catch (ArgumentException ex) 
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { success = false, error = ex.Message });
        }
    }

    /// <summary>
    /// 多圖片上傳 API，用於貼文圖片上傳
    /// </summary>
    [HttpPost("upload-multiple")]
    public async Task<IActionResult> UploadMultipleImages(List<IFormFile> files)
    {
        try
        {
            if (files == null || !files.Any())
            {
                return BadRequest(new { success = false, error = "請選擇要上傳的檔案" });
            }

            // 限制最多上傳5張圖片
            if (files.Count > 5)
            {
                return BadRequest(new { success = false, error = "最多只能上傳5張圖片" });
            }

            var uploadedUrls = new List<string>();

            foreach (var file in files)
            {
                // 檢查檔案大小 (限制2MB)
                if (file.Length > 2 * 1024 * 1024)
                {
                    return BadRequest(new { success = false, error = $"檔案 {file.FileName} 大小超過2MB限制" });
                }

                var url = await _uploadImageService.UploadImage(file);
                uploadedUrls.Add(url);
            }

            // 將URL用分號連接，符合Post.ImageUrls的格式
            var imageUrlsString = string.Join(";", uploadedUrls);

            return Ok(new 
            { 
                success = true, 
                imageUrls = imageUrlsString,
                urls = uploadedUrls,
                count = uploadedUrls.Count
            });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { success = false, error = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "多圖片上傳失敗");
            return StatusCode(500, new { success = false, error = "上傳失敗，請稍後再試" });
        }
    }
}
