using Microsoft.EntityFrameworkCore;
using ZDZCode.Api.Data;
using ZDZCode.Api.Domain;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddControllers();

builder.Services.AddDbContext<AppDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
        ?? "Data Source=zdzcode.db";
    options.UseSqlite(connectionString);
});

var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>()
    ?? ["http://localhost:3000"];

builder.Services.AddCors(options =>
{
    options.AddPolicy("FrontendPolicy", policy =>
    {
        policy
            .WithOrigins(allowedOrigins)
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseCors("FrontendPolicy");
app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await dbContext.Database.EnsureCreatedAsync();
    await SeedSampleDataAsync(dbContext);
}

app.Run();

static async Task SeedSampleDataAsync(AppDbContext dbContext)
{
    var hasSeedData = await dbContext.Categories.AnyAsync()
        || await dbContext.Products.AnyAsync()
        || await dbContext.Customers.AnyAsync();

    if (hasSeedData)
    {
        return;
    }

    var electronics = new Category
    {
        Name = "Eletrônicos",
        Description = "Notebooks, periféricos e acessórios para uso corporativo"
    };

    var office = new Category
    {
        Name = "Escritório",
        Description = "Itens para rotina administrativa e produtividade"
    };

    dbContext.Categories.AddRange(electronics, office);
    await dbContext.SaveChangesAsync();

    var notebook = new Product
    {
        Name = "Notebook Corporativo 14",
        Description = "Notebook para equipe comercial com 16 GB de RAM",
        Sku = "NB-PRO-14",
        Price = 5699.90m,
        StockCurrent = 8,
        IsActive = true,
        CategoryId = electronics.Id
    };

    var keyboard = new Product
    {
        Name = "Teclado Mecânico ABNT2",
        Description = "Teclado mecânico para estações de trabalho",
        Sku = "KB-MEC-01",
        Price = 349.90m,
        StockCurrent = 20,
        IsActive = true,
        CategoryId = office.Id
    };

    dbContext.Products.AddRange(notebook, keyboard);

    var vipTag = new Tag
    {
        Name = "Destaque",
        IsActive = true
    };

    dbContext.Tags.Add(vipTag);
    await dbContext.SaveChangesAsync();

    dbContext.ProductTags.Add(new ProductTag
    {
        ProductId = notebook.Id,
        TagId = vipTag.Id
    });

    var customer = new Customer
    {
        Name = "Mariana Souza",
        Email = "mariana.souza@empresa.com.br",
        Phone = "11987654321",
        IsActive = true,
        CreatedAt = DateTime.UtcNow
    };

    dbContext.Customers.Add(customer);
    await dbContext.SaveChangesAsync();

    dbContext.DeliveryAddresses.Add(new DeliveryAddress
    {
        CustomerId = customer.Id,
        Street = "Rua das Flores",
        Number = "245",
        District = "Jardim Paulista",
        City = "São Paulo",
        State = "SP",
        ZipCode = "01419-001",
        Complement = "Conjunto 52"
    });

    var order = new Order
    {
        CustomerId = customer.Id,
        Status = "confirmed",
        Subtotal = notebook.Price,
        DiscountTotal = 0m,
        Total = notebook.Price,
        CreatedAt = DateTime.UtcNow,
        Note = "Pedido de demonstração para validação do fluxo de pagamentos"
    };

    dbContext.Orders.Add(order);
    await dbContext.SaveChangesAsync();

    dbContext.OrderItems.Add(new OrderItem
    {
        OrderId = order.Id,
        ProductId = notebook.Id,
        Quantity = 1,
        UnitPrice = notebook.Price,
        LineTotal = notebook.Price
    });

    dbContext.Payments.Add(new Payment
    {
        OrderId = order.Id,
        Method = "pix",
        Status = "approved",
        Value = order.Total,
        TransactionReference = "SEED-PIX-0001",
        CreatedAt = DateTime.UtcNow
    });

    dbContext.StockMovements.AddRange(
        new StockMovement
        {
            ProductId = notebook.Id,
            MovementType = "entrada",
            Quantity = 8,
            Reason = "Carga inicial do estoque",
            CreatedAt = DateTime.UtcNow
        },
        new StockMovement
        {
            ProductId = keyboard.Id,
            MovementType = "entrada",
            Quantity = 20,
            Reason = "Carga inicial do estoque",
            CreatedAt = DateTime.UtcNow
        }
    );

    await dbContext.SaveChangesAsync();
}
