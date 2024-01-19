
using BetterCoding.Strapi.SDK.Core;
using BetterCoding.Strapi.SDK.Core.Server;
using Microsoft.Extensions.Configuration;
using Todo.Proxy.Repository;

namespace Todo.Proxy.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Host.ConfigureServices((hostContext, services) =>
            {
                var serverConfiguration = builder.Configuration.GetSection("Strapi").Get<StrapiServerConfiguration>();
                StrapiClient.AddServer(serverConfiguration);

                services.AddTransient<ITodoEntryRepository, TodoEntryRepository>();
                services.AddTransient<IAuthRepository, AuthRepository>();
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
