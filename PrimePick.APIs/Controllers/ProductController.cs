using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using PrimePick.Core.Models;
using PrimePick.Core.Services.Contract;
using PrimePick.Core.Specifications.Products;
using System.Threading.Tasks;

namespace PrimePick.APIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        public ProductController(IProductService productService)
        {
            ProductService = productService;
        }

        public IProductService ProductService { get; }
        [HttpGet]
        [EnableRateLimiting("fixed")]
        [Authorize]
        public async Task<IActionResult> GetAllProduct([FromQuery] ProductParams productParams)
        {
            var result = await ProductService.GetAllProductAsync(productParams );
            return Ok(result);
        }

        [HttpGet("Brand")]
        public async Task<IActionResult> GetAllBrand()
        {
            var result = await ProductService.GetAllBrandAsync();
            return Ok(result);
        }

        [HttpGet("Type")]
        public async Task<IActionResult> GetAllType()
        {
            var result = await ProductService.GetAllTypeAsync();
            return Ok(result);
        }

        //[HttpGet("{id}")]
        //public async Task<IActionResult> GetProductById(int id)
        //{
        //    if (id == null)
        //    {
        //        return BadRequest("invalid id ");
        //    }
        //    var result = await ProductService.GetProductByIdAsync(id);
        //    return Ok(result);
        //}

        [HttpPost]

        public async Task<IActionResult> Add(Product product)
        {
            await ProductService.AddProduct(product);
            return Created();
        }


       
    }
}
