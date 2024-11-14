namespace Catalog.Api.Products.GetProducts;

public record GetProductsResult(
    IEnumerable<Product> Products
);

public record GetProductsQuery
    : IQuery<GetProductsResult>;

internal class GetProductsQueryHandler(
    IDocumentSession documentSession,
    ILogger<GetProductsQueryHandler> logger
)
    : IQueryHandler<GetProductsQuery, GetProductsResult>
{
    public async Task<GetProductsResult> Handle(GetProductsQuery query, CancellationToken cancellationToken)
    {
        logger.LogInformation("Querying for products with {@Query}", query);
        var products = await documentSession.Query<Product>().ToListAsync(cancellationToken);
        return new GetProductsResult(products);
    }
}