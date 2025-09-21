using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BoardGameFontier.Repostiory;
using BoardGameFontier.Repostiory.Entity;
using BoardGameFontier.Repostiory.DTOS;

namespace BoardGameFontier.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersAPIController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public OrdersAPIController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/OrdersAPI/Filter
        [HttpPost("Filter")]
        public async Task<IEnumerable<OrderDTO>> FilterOrders([FromBody] OrderDTO oDTO)
        {
            return _context.Orders.Where(o => o.CaseId == oDTO.CaseId ||            
            (o.ClientName != null && o.ClientName.Contains(oDTO.ClientName)) ||
            (o.Country != null && o.Country.Contains(oDTO.Country)) ||
            (o.City != null && o.City.Contains(oDTO.City)) ||
            (o.PostalCode != null && o.PostalCode.Contains(oDTO.PostalCode)) ||
            (o.OrderNo != null && o.OrderNo.Contains(oDTO.OrderNo)) ||
            (o.ProductName != null && o.ProductName.Contains(oDTO.ProductName)) ||
            (o.BundleName != null && o.BundleName.Contains(oDTO.BundleName)) ||
            (o.Price == oDTO.Price) ||
            (o.ShippingStatus != null && o.ShippingStatus.Contains(oDTO.ShippingStatus))

              )

                .Select(o => new OrderDTO
                {
                    CaseId = o.CaseId,
                    ClientName = o.ClientName,
                    Country = o.Country,
                    City = o.City,
                    PostalCode = o.PostalCode,
                    OrderNo = o.OrderNo,
                    ProductName = o.ProductName,
                    BundleName = o.BundleName,
                    Price = o.Price,
                    CreatedAt = o.CreatedAt,
                    PaidAt = o.PaidAt,
                    ShippingStatus = o.ShippingStatus
                });
        }
        /*  // GET: api/OrdersAPI
        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrderDTO>>> GetOrders()
        {
            return await _context.Orders.ToListAsync();
        }

        // GET: api/OrdersAPI/5
        [HttpGet("{id}")]
        public async Task<ActionResult<OrderDTO>> GetOrder(int id)
        {
            var order = await _context.Orders.FindAsync(id);

            if (order == null)
            {
                return NotFound();
            }

            return order;
        } */

        // PUT: api/OrdersAPI/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<ReturnType> PutOrder(int id, OrderDTO oDTO)
        {
            if (id != oDTO.CaseId)
            {
                return new ReturnType
                {
                    Ok = false,
                    Message = "更新報表失敗"
                };
            }

            Order order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return new ReturnType
                {
                    Ok = false,
                    Message = "更新報表失敗"
                };
            }

            //Map DTO to Entity
            order.ClientName = oDTO.ClientName;
            order.Country = oDTO.Country;
            order.City = oDTO.City;
            order.PostalCode = oDTO.PostalCode;
            order.OrderNo = oDTO.OrderNo;
            order.ProductName = oDTO.ProductName;
            order.BundleName = oDTO.BundleName;
            order.Price = (int)oDTO.Price;           
            order.ShippingStatus = oDTO.ShippingStatus;

            _context.Entry(oDTO).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OrderExists(id))
                {
                    return new ReturnType
                    {
                        Ok = false,
                        Message = "更新報表失敗"
                    };
                }
                else
                {
                    throw;
                }
            }

            return new ReturnType
            {
                Ok = true,
                Message = "更新報表成功"
            };
        }

        // POST: api/OrdersAPI
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ReturnType> PostOrder(OrderDTO order)
        {
            Order ord = new Order
            {
                ClientName = order.ClientName,
                Country = order.Country,
                City = order.City,
                PostalCode = order.PostalCode,
                OrderNo = order.OrderNo,
                ProductName = order.ProductName,
                BundleName = order.BundleName,
                Price = (int)order.Price,
                CreatedAt = DateTime.Now, // 設定建立時間為現在
                PaidAt = null, // 初始未付款
                ShippingStatus = order.ShippingStatus // 初始狀態
            };
            _context.Orders.Add(ord);
            await _context.SaveChangesAsync();
            order.CaseId = ord.CaseId; // 確保返回的 DTO 包含新訂單的 CaseId
            return new ReturnType
            {
                Ok = true,
                Message = "新增成功"
            };
        }

        // DELETE: api/OrdersAPI/5
        [HttpDelete("{id}")]
        public async Task<ReturnType> DeleteOrder(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return new ReturnType
                {
                    Ok = false,
                    Message = "報表刪除紀錄失敗"
                };
            }
            try
                {
                    _context.Orders.Remove(order);
                    await _context.SaveChangesAsync();
                }

             catch (DbUpdateException ex)
                {
                    return new ReturnType
                    {
                        Ok = false,
                        Message = "報表刪除紀錄失敗"
                    };
                }
            return new ReturnType()
            {
                Ok = true,
                Message = "報表刪除紀錄成功"
            };
        }

        private bool OrderExists(int id)
        {
            return _context.Orders.Any(e => e.CaseId == id);
        }
    }
}
