using Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class ProductService : IProductService
    {
        public async Task<ProductValidationResult> ValidateProductsAsync(IEnumerable<Guid> productIds)
        {
            return new ProductValidationResult(true, new Dictionary<Guid, decimal>(), new List<string>());
        }
    }
}
