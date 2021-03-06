using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using TelegramBotWebHook.Binder;
using TelegramBotWebHook.Controllers;
using TelegramBotWebHook.Data;

namespace TelegramBotWebHook
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            services.AddMediatR(Assembly.GetExecutingAssembly());

            services.AddMvc(
                config => config.ModelBinderProviders.Insert(0, new TelegramBotBinderProvider())
            );

            var http = new HttpClient();
            var bot = new TelegramBotClient("Api Token", http);

            services.AddSingleton(http);
            services.AddSingleton(bot);
            Task.Run(async () =>
            {
                await bot.SetWebhookAsync("https://5c11946c.ngrok.io/api/update");
            });
            
            InitialDatabase.Init();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            //app.UseHttpsRedirection();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
