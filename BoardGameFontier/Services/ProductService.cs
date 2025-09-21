using BoardGameFontier.Models.ViewModels;
using BoardGameFontier.Repostiory;
using BoardGameFontier.Repostiory.DTOS;
using BoardGameFontier.Repostiory.Entity;

namespace BoardGameFontier.Services
{
    //開發步驟5-1 建立相對應的服務 如果有了可以忽略
    public class ProductService : IProductService
    {
        //開發步驟5-2 相依性注入 如果有了可以忽略
        private readonly ApplicationDbContext _context;
        public ProductService(ApplicationDbContext context)
        {
            _context = context;
        }
        public List<LoadMarketDataViewModel> GetAllProduct()
        {
            var data = _context.TradeItems.ToList().Select(static d => new LoadMarketDataViewModel
            {
                Name = d.Name,
                Price = d.Price.ToString(),
                ImageUrl = d.ImageUrls==null ? "":d.ImageUrls.Split(',')[0],
                Id = d.Id.ToString(),
                Description = d.Description,
                sellerId = d.SellerId,
                updateAt = d.UpdatedAt.ToString(),
                Category = d.Category,
                Stock = d.StockQuantity
            });
            return data.ToList();
        }

        //開發步驟5-3 (廚房)建立需要的服務(拿ID查詢產品明細) 回傳則使用開發步驟4建立的ViewModel
        public GetProductDetailViewModel GetProductDetail(string Id)
        {
            var data = _context.TradeItems
               .Where(d => d.Id.ToString() == Id)
                .Join(_context.UserProfiles,
                      product => product.SellerId,
                      profile => profile.Id,
                      (product, profile) => new GetProductDetailViewModel
                      {
                          ProductName = product.Name,
                          Price = product.Price.ToString(),
                          ImageUrl = product.ImageUrls,
                          StockQuantity = product.StockQuantity.ToString(),
                          Description = product.Description,
                          Category = product.Category.ToString(),
                          ProductOwner = profile.DisplayName,
                          ProductOwnerPic = profile.ProfilePictureUrl,
                          UserName = profile.UserName
                      }).FirstOrDefault();

            return data;
        }

        public List<LoadMarketDataViewModel> GetAllProductByUser(string userId)
        {
            var data = _context.TradeItems
                            .Where(d => d.SellerId == userId)
                            .ToList()
                            .Select(d => new LoadMarketDataViewModel
                            {
                                Name = d.Name,
                                Price = d.Price.ToString(),
                                ImageUrl = d.ImageUrls == null ? "" : d.ImageUrls.Split(',').FirstOrDefault(),
                                Id = d.Id.ToString()
                            }).ToList();

            return data;
        }

        public bool UpdateProductDetail(UpdateProductDetailDto dto, IEnumerable<string> images)
        {
            try
            {
                // 1. 從資料庫找出對應商品
                var product = _context.TradeItems
                    .FirstOrDefault(p => p.Id.ToString() == dto.ProductId);

                if (product == null)
                {
                    throw new Exception("找不到對應的商品");
                }

                // 2. 更新欄位
                product.ImageUrls = string.Join(",", images);            // 如果是多張圖可以再拆成 List 存
                product.Name = dto.ProductName;
                product.Price = dto.Price;                   // Price 已經是 int
                product.StockQuantity = int.Parse(dto.StockQuantity); // 如果資料庫是 int
                product.Description = dto.Description;
                product.Category = int.Parse(dto.Category);  // 如果 Category 是 enum/int

                product.UpdatedAt = DateTime.UtcNow;         // 建議記錄更新時間
                _context.TradeItems.Update(product);
                // 3. 存回資料庫
                return _context.SaveChanges() > 0;
            }
            catch (Exception ex)
            {
                throw new Exception($"更新商品明細時發生錯誤: {ex.Message}", ex);
            }
        }


        public bool DeleteProductDetail(DeleteProductDetailDto dto)
        {
            try
            {
                // 1. 從資料庫找出對應商品
                var product = _context.TradeItems
                    .FirstOrDefault(p => p.Id.ToString() == dto.Id);

                if (product == null)
                {
                    throw new Exception("找不到對應的商品");
                }
                _context.TradeItems.Remove(product);
                // 3. 存回資料庫
                return _context.SaveChanges() > 0;
            }
            catch (Exception ex)
            {
                throw new Exception($"更新商品明細時發生錯誤: {ex.Message}", ex);
            }
        }

        public bool AddProductDetail(string userId, CreateProductDto dto, IEnumerable<string> images)
        {
            try
            {
                var newProduct = new TradeItem
                {
                    Name = dto.ProductName,
                    Price = Convert.ToDecimal(dto.Price),
                    ImageUrls = string.Join(",", images),
                    StockQuantity = dto.StockQuantity,
                    Description = dto.Description,
                    Category = int.Parse(dto.Category),
                    SellerId = userId,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                };
                _context.TradeItems.Add(newProduct);
                return _context.SaveChanges() > 0;
            }
            catch (Exception ex)
            {
                throw new Exception($"新增商品明細時發生錯誤: {ex.Message}", ex);
            }
        }
    }
}
