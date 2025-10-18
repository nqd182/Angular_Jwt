using Jwt.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Jwt.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly MyDbContext _context;
        public ProductController(MyDbContext context)
        {
            _context = context;

        }
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var products = _context.Products.ToList();
            try
            {
                return Ok(new ApiResponse
                {
                    Success = true,
                    Message = "Lấy danh sách sản phẩm thành công",
                    Data = products
                });
            }
            catch
            {
                return Unauthorized(new ApiResponse
                {
                    Success = false,
                    Message = "Lấy danh sách sản phẩm không thành công",
                    Data = products
                });
            }
           
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var product = _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound(new ApiResponse
                {
                    Success = false,
                    Message = "Sản phẩm không tồn tại"
                });
            }
            return Ok(new ApiResponse
            {
                Success = true,
                Message = "Lấy sản phẩm thành công",
                Data = product
            });
        }
        [HttpPost]
        public async Task<IActionResult> Create(ProductModel product)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Dữ liệu không hợp lệ",
                    Data = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)
                });
            }
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            return Ok(new ApiResponse
            {
                Success = true,
                Message = "Tạo sản phẩm thành công",
                Data = product
            });
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, ProductModel updatedProduct)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null) return NotFound();

            product.Name = updatedProduct.Name;
            product.Description = updatedProduct.Description;
            product.Price = updatedProduct.Price;
            product.ImageUrl = updatedProduct.ImageUrl;

            await _context.SaveChangesAsync();
            return Ok(new ApiResponse
            {
                Success = true,
                Message = "Cập nhật sản phẩm thành công",
                Data = product
           
            });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null) return NotFound();

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return Ok();
        }
    }
}
