using AuthServer.Core.DTOs;
using AuthServer.Core.Models;
using AuthServer.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AuthServer.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : CustomBaseController
    {
        private readonly IServiceGeneric<Product, ProductDto> productService;

        public ProductController(IServiceGeneric<Product, ProductDto> productService)
        {
            this.productService = productService;
        }

        [HttpGet]

        public async Task<IActionResult> GetProducts()
        {
             return ActionResultInstance(await productService.GetAllAsync());
        }

        [HttpPost]

        public async Task<IActionResult> SaveProduct(ProductDto productDto)
        {
            return ActionResultInstance(await productService.AddAsync(productDto));
        }

        [HttpPut]
        public async Task<IActionResult> UpdateProduct(ProductDto productDto)
        {
            return ActionResultInstance(await productService.Update(productDto, productDto.Id));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            return ActionResultInstance(await productService.Remove(id));
        }
    }
}
