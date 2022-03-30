using System;
using System.Threading.Tasks;
using GhostNetwork.Messages.Api.Helpers.OpenApi;
using GhostNetwork.Messages.Api.Hubs;
using GhostNetwork.Messages.MongoDb;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using MongoDB.Driver;
using Swashbuckle.AspNetCore.Filters;

namespace GhostNetwork.Messages.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        private IConfiguration configuration { get; }

        private const string DefaultDbName = "messages";

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddSignalR();

            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "GhostNetwork.Messages",
                    Description = "Http client for GhostNetwork.Messages",
                    Version = "1.0.0",
                });

                options.IncludeXmlComments(XmlPathProvider.XmlPath);
                options.OperationFilter<AddResponseHeadersFilter>();
            });

            services.AddScoped(_ =>
            {
                var connectionString = configuration["MONGO_CONNECTION"];
                var mongoUrl = MongoUrl.Create(connectionString);
                var client = new MongoClient(mongoUrl);
                return new MongoDbContext(client.GetDatabase(mongoUrl.DatabaseName ?? DefaultDbName));
            });

            services.AddScoped<IChatService, MongoChatStorage>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IServiceProvider provider)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "GhostNetwork.Messages.Api v1");
                    c.DisplayRequestDuration();

                    var chatService = provider.GetRequiredService<IChatService>();
                    SeedData(chatService).GetAwaiter().GetResult();
                });
            }

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<ChatHub>("/chat");
            });
        }

        private async Task SeedData(IChatService chatService)
        {
            var chatId = Guid.Parse("3fa85f64-5717-4562-b3fc-2c963f66afa6");
            var sender = Guid.Parse("3fa85f64-5717-4562-b3fc-2c963f66afa1");
            var receiver = Guid.Parse("3fa85f64-5717-4562-b3fc-2c963f66afa2");

            if (await chatService.GetExistChatByIdAsync(chatId) == Guid.Empty)
            {
                var newChat = Chat.NewChat(chatId, sender, receiver);

                await chatService.CreateNewChatAsync(newChat);
            }
        }
    }
}