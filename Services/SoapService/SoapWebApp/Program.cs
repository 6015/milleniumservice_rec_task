
using SoapCore;

namespace SoapWebApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddSoapCore();
            builder.Services.AddScoped<Interfaces.IFibonacciService, Services.FibonacciService>();

            // Add services to the container.  
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            app.UseRouting();

            // Fix: Use the correct extension method on IApplicationBuilder instead of IEndpointRouteBuilder  
            app.UseSoapEndpoint<Interfaces.IFibonacciService>("/FibonacciService.svc", new SoapCore.SoapEncoderOptions());

            // Configure the HTTP request pipeline.  
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
