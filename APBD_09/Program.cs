using APBD_09.Controller;
using APBD_09.Context;
using APBD_09.Service;

class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddControllers().AddXmlSerializerFormatters();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        builder.Services.AddScoped<ApiController>();
        builder.Services.AddScoped<ApiService>();
        builder.Services.AddScoped<Apbd09Context>();

        var app = builder.Build();

// Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();
        
        app.MapControllers();
        
        app.Run();
    }
}