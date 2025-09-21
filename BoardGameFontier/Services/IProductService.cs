using BoardGameFontier.Models.ViewModels;
using BoardGameFontier.Repostiory.DTOS;

namespace BoardGameFontier.Services
{
    public interface IProductService
    {
        List<LoadMarketDataViewModel> GetAllProduct();
        //開發步驟6 (菜單)加入開發步驟5-3的方法
        GetProductDetailViewModel GetProductDetail(string Id);
        List<LoadMarketDataViewModel> GetAllProductByUser(string userId);
        bool UpdateProductDetail(UpdateProductDetailDto dto, IEnumerable<string> images);
        bool DeleteProductDetail(DeleteProductDetailDto dto);
        bool AddProductDetail(string userId, CreateProductDto dto, IEnumerable<string> images);

    }
}
