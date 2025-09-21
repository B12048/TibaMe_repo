using BoardGameFontier.Repostiory.DTOS;
using BoardGameFontier.Repostiory.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using System.Text.Json;

namespace BoardGameFontier.Controllers.API
{
    [Route("api/Mall/[action]")]
    [ApiController]
    [OutputCache]
    public class MallAPIController : ControllerBase
    {
        private readonly MerchandiseRepository _repo;
        public MallAPIController(MerchandiseRepository repo)
        {
            _repo = repo;
        }

        [HttpGet]
        public async Task<List<MerchandiseDTO>> GetMerchandises()
        {
            return await _repo.GetMerchandiseAsync();
        }

        [HttpPost]
        public IActionResult aa([FromForm] string MerchantID, [FromForm] string RqHeader, [FromForm] string Data)
        {
            var rqHeaderObj = JsonSerializer.Deserialize<RqHeaderModel>(RqHeader);
            var dataObj = JsonSerializer.Deserialize<ECPayDataModel>(Data);

            Console.WriteLine("MerchantID: " + MerchantID);
            Console.WriteLine("RqHeader: " + JsonSerializer.Serialize(rqHeaderObj));
            Console.WriteLine("Data: " + JsonSerializer.Serialize(dataObj));

            return Content("1|OK", "text/plain");
        }


        public class ECPayNotificationModel
        {
            public string MerchantID { get; set; }
            public RqHeaderModel RqHeader { get; set; }
            public ECPayDataModel Data { get; set; }
        }

        public class RqHeaderModel
        {
            public string Timestamp { get; set; }
            public string Revision { get; set; }
        }

        public class ECPayDataModel
        {
            public string MerchantTradeNo { get; set; }
            public int RtnCode { get; set; }
            public string RtnMsg { get; set; }
            public string TradeNo { get; set; }
            public int TradeAmt { get; set; }
            public string PaymentDate { get; set; }
            public string PaymentType { get; set; }
            public decimal PaymentTypeChargeFee { get; set; }
            public string TradeDate { get; set; }
            public int SimulatePaid { get; set; }
            public string CustomField1 { get; set; }
            public string CustomField2 { get; set; }
            public string CustomField3 { get; set; }
            public string CustomField4 { get; set; }
            public string CheckMacValue { get; set; }
        }
    }
}
