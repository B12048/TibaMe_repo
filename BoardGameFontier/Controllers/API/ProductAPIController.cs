using BoardGameFontier.Repostiory.DTOS;
using BoardGameFontier.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Security.Claims;

namespace BoardGameFontier.Controllers
{
    //開發步驟3-1 (開始處理後端) 新增API Controller 如果有了可以省略
    [ApiController]
    //開發步驟3-2 設定路由 代表 /api/product 會進到這個Controller 如果有了可以省略
    [Route("api/product")]
    public class ProductAPIController : ControllerBase
    {
        private readonly IProductService _service;
        private readonly ILogger<ProductAPIController> _logger;
        private readonly IUploadImageService _uploadImageService;

        //開發步驟3-3 相依性注入 須先到ServiceCollectionExtention.cs內註冊 如果有了可以省略
        public ProductAPIController(IProductService service, ILogger<ProductAPIController> logger, IUploadImageService uploadImageService)
        {
            _service = service;
            _logger = logger;
            _uploadImageService = uploadImageService;

        }

        [HttpPost("loadMarkeIndextData")]
        public IActionResult loadMarkeIndextData([FromBody] GetAllUsersDto dto)
        {
            try
            {
                var data = _service.GetAllProduct();
                return Ok(data);
            }
            catch (Exception ex)
            {
                _logger.LogError($"[GetAllUsers] Error: {ex}");
                return BadRequest("無法取得使用者資料");
            }
        }

        //開發步驟3-4 新增API入口 設定需與前端寫的路徑一致
        [HttpPost("getProductDetail")] //<== 代表uri /api/product/getProductDetail會進到這裡
        public IActionResult GetProductDetail([FromBody] GetProductDetailDto dto)//開發步驟3-5 帶入開發步驟2產出的DTO
        {
            try
            {
                //開發步驟7 呼叫服務的方法
                var data = _service.GetProductDetail(dto.Id);
                return Ok(data);
            }
            catch (Exception ex)
            {
                _logger.LogError($"[GetProductDetail] Error: {ex}");
                return BadRequest("無法取得產品資料");
            }
        }

        [HttpPost("updateProductDetail")] //<== 代表uri /api/product/getProductDetail會進到這裡
        public async Task<IActionResult> UpdateProductDetail([FromForm] UpdateProductDetailDto dto, [FromForm] List<IFormFile> Images)//開發步驟3-5 帶入開發步驟2產出的DTO
        {
            try
            {
                //開發步驟7 呼叫服務的方法
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                List<string> imageUrls = new List<string>();
                var oldImageList = dto.oldImages.Split(",");
                foreach (var image in oldImageList)
                {
                    imageUrls.Add(image);
                }

                foreach (var image in Images)
                {
                    imageUrls.Add(await _uploadImageService.UploadImage(image));
                }

                var update = _service.UpdateProductDetail(dto, imageUrls) ? "更新成功" : "失敗";
                var result = new
                {
                    Result = update
                };

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"[GetProductDetail] Error: {ex}");
                return BadRequest("無法取得產品資料");
            }
        }

        [HttpPost("deleteProductDetail")] //<== 代表uri /api/product/getProductDetail會進到這裡
        public IActionResult DeleteProductDetail([FromBody] DeleteProductDetailDto dto)
        {
            try
            {
                //開發步驟7 呼叫服務的方法
                var update = _service.DeleteProductDetail(dto) ? "刪除成功" : "失敗";
                var result = new
                {
                    Result = update
                };

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"[GetProductDetail] Error: {ex}");
                return BadRequest("無法取得產品資料");
            }
        }

        [HttpPost("createProduct")]
        public async Task<IActionResult> CreateProduct([FromForm] CreateProductDto dto, [FromForm] List<IFormFile> Images)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                List<string> imageUrls = new List<string>();
                foreach (var image in Images) 
                {
                    imageUrls.Add(await _uploadImageService.UploadImage(image));
                }
                
                _service.AddProductDetail(userId, dto, imageUrls);
                return Ok();
            }
            catch (Exception ex) 
            {
                _logger.LogError($"[CreateProduct] Error: {ex}");
                return BadRequest("無法建立產品");
            }
        }

    }
}
