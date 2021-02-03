using DragonBlog.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DragonBlog.Utilities
{
    public class DataHelper
    {

        public static string GetConnectionString(IConfiguration configuration)
        {
            //Default connection string coming from appsettings like usual
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            //it will be automatically overwritten if running on heroku
            var databaseUrl = Environment.GetEnvironmentVariable("DATABASE_URL");
            return string.IsNullOrEmpty(databaseUrl) ? connectionString : BuildConnectionString(databaseUrl);
        }

        public static string BuildConnectionString(string databaseUrl)
        {
            //Provides an object representation of a uniform resource identifier (URI) and easy access to parts of the URI.
            var databaseUri = new Uri(databaseUrl);
            var userInfo = databaseUri.UserInfo.Split(':');

            //Provides a simple way to create and manage the contentson connection strings used by the NpgsqlConnection class
            var builder = new NpgsqlConnectionStringBuilder
            {
                Host = databaseUri.Host,
                Port = databaseUri.Port,
                Username = userInfo[0],
                Password = userInfo[1],
                Database = databaseUri.LocalPath.TrimStart('/')
            };


            return builder.ToString();
        }

        public static async Task ManageData(IHost host)
        {
            try
            {
                using var svcScope = host.Services.CreateScope();
                var svcProvider = svcScope.ServiceProvider;

                var dbContextSvc = svcProvider.GetRequiredService<ApplicationDbContext>();
                await dbContextSvc.Database.MigrateAsync();
            }


            catch (PostgresException ex)
            {
                Console.WriteLine($"Exception while running Manage Data => {ex}");
            }
        }


    }
}
