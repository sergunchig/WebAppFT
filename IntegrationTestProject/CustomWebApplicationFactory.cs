using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebAppFT.Model;

namespace IntegrationTestProject
{
    public class CustomWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram> where TProgram : class
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            // добавление конфигурации
            var config = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json").Build();
            //Подмена базы данных
            builder.ConfigureServices(
                services =>
                {
                    //удаление подключения к БД
                    var dbContextDescriptor = services.SingleOrDefault(x=> x.ServiceType 
                        == typeof(DbContextOptions<AppDbContext>));
                    services.Remove(dbContextDescriptor!);

                    var dbConnectionDescriptor = services.SingleOrDefault(
                        d => d.ServiceType == typeof(DbConnection));

                    services.Remove(dbConnectionDescriptor!);

                    // создание подключения к БД 
                    services.AddSingleton<DbConnection>(container =>
                    {
                        string connectionString = config.GetConnectionString("WeatherTestConnection")!;
                        var connection = new Npgsql.NpgsqlConnection(connectionString);
                        connection.Open();

                        return connection;
                    });

                    services.AddDbContext<AppDbContext>((container, options) =>
                    {
                        var connection = container.GetRequiredService<DbConnection>();
                        options.UseNpgsql(connection);
                    });
                });
            base.ConfigureWebHost(builder);
        }
    }
}
