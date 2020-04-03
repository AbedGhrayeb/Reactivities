using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Persistence;

namespace API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();
            //ensure to migrate database
            using(var scope= host.Services.CreateScope())
            {
                var services= scope.ServiceProvider;
                try
                {
                    var context= services.GetRequiredService<DataContext>();
                    //generate migration
                    context.Database.Migrate();
                    // seed data
                    Seed.SeedData(context);
                }
                catch (Exception ex)
                {
                    //if any error when apply migration
                    var logger=services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex,"An error occured during migration");
                }
            }
            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
