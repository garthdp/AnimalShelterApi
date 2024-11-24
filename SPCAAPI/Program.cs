
using Google.Api;
using Microsoft.EntityFrameworkCore;
using SPCAAPI.Data;

namespace SPCAAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll",
                               builder => builder.AllowAnyOrigin()  
                              .AllowAnyMethod()  
                              .AllowAnyHeader());
            });
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddDbContext<WilDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("WilDb")));

            var app = builder.Build();

            app.UseCors("AllowAll");

            app.UseDeveloperExceptionPage();
            app.UseSwagger();
            app.UseSwaggerUI();

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
