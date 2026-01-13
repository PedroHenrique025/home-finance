using System.Reflection;
using Microsoft.EntityFrameworkCore;
using home_finance.API.Data;
using home_finance.API.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// Configura Conexão com o banco de dados SQLite
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<ITransactionService, TransactionService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IPersonService, PersonService>();

// Configura Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    // Informações da API
    options.SwaggerDoc("v1", new()
    {
        Version = "v1",
        Title = "Home Finance API",
        Description = "API para controle de gastos residenciais",
    });

    // Incluir XML comments no Swagger
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

    if (File.Exists(xmlPath))
    {
        options.IncludeXmlComments(xmlPath);
    }
});

// Configuração de CORS para permitir requisições do frontend
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:3000") // URL do frontend
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    // Aplica migrations pendentes automaticamente
    dbContext.Database.Migrate();
}

if (app.Environment.IsDevelopment())
{
    // Habilita Swagger UI
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Home Finance API V1");
        options.RoutePrefix = string.Empty; // Swagger na raiz
        options.DocumentTitle = "Home Finance API - Documentação";

        // Configurações de UI
        options.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.List);
        options.DefaultModelsExpandDepth(2);
        options.DisplayRequestDuration();
        options.InjectStylesheet("/swagger-ui/custom.css");
    });
}

app.UseCors("AllowFrontend");
app.UseAuthorization();
app.UseStaticFiles();
app.MapControllers();

app.Run();