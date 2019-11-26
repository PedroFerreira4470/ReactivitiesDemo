using System;
using Domain;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
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
            using(var scope = host.Services.CreateScope()){
                var services = scope.ServiceProvider;
                try{
                    var context = services.GetRequiredService<DataContext>();
                    var userManager = services.GetRequiredService<UserManager<AppUser>>();
                    context.Database.Migrate();
                    Seed.SeedData(context, userManager).Wait();

                }catch (Exception e){
                    var logger =services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(e , "Error occur during Migration");
                }
            }  
            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args).ConfigureWebHostDefaults(p=>{
                p.UseStartup<Startup>();
            });
                //.UseSerilog((hc,lc) => 
                //    lc.ReadFrom.Configuration(hc.Configuration)
                //    .Enrich.FromLogContext()
                //    .WriteTo.Debug()
                //    .WriteTo.Console()
                //    .WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri("http://localhost:9200/")) { 
                //        AutoRegisterTemplate = true,
                //        AutoRegisterTemplateVersion = AutoRegisterTemplateVersion.ESv6
                //    })
                //)
    }
}
