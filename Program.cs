using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.File("logs/api-log.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog();

builder.Services.AddControllers();
builder.Services.AddSingleton<BookCatalog.Services.BookService>(provider => new BookCatalog.Services.BookService("Data/books.csv"));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var app = builder.Build();

// Global error handling middleware
app.UseMiddleware<BookCatalog.Middleware.ErrorHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseAuthorization();

app.UseHttpsRedirection();

app.MapControllers();

app.Run();

