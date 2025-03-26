namespace Application.Interfaces;

public interface IProductService
{
    Task<ProductValidationResult> ValidateProductsAsync(IEnumerable<Guid> productIds);
}

public record ProductValidationResult(
    bool IsValid,
    Dictionary<Guid, decimal> Prices,
    List<string> Errors);
