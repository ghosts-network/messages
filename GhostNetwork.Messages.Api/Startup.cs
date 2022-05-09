using System;
using GhostNetwork.Messages.Api.Domain;
using GhostNetwork.Messages.Api.Helpers.OpenApi;
using GhostNetwork.Messages.Api.Users;
using GhostNetwork.Messages.Chats;
using GhostNetwork.Messages.Integrations;
using GhostNetwork.Messages.Integrations.Chats;
using GhostNetwork.Messages.Integrations.Messages;
using GhostNetwork.Messages.Users;
using GhostNetwork.Profiles.Api;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using MongoDB.Driver;
using Swashbuckle.AspNetCore.Filters;

namespace GhostNetwork.Messages.Api
{
    public class Startup
    {
        private const string DefaultDbName = "messages";

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        private IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            services.AddLogging(x =>
            {
                x.ClearProviders();
                x.AddConsole();
            });

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
                var connectionString = Configuration["MONGO_CONNECTION"];
                var mongoUrl = MongoUrl.Create(connectionString);
                var client = new MongoClient(mongoUrl);
                return new MongoDbContext(client.GetDatabase(mongoUrl.DatabaseName ?? DefaultDbName));
            });

            services.AddScoped<IChatsStorage, MongoChatStorage>(provider =>
                new MongoChatStorage(provider.GetRequiredService<MongoDbContext>()));

            services.AddScoped<IMessagesStorage, MongoMessageStorage>(provider =>
                new MongoMessageStorage(provider.GetRequiredService<MongoDbContext>()));

            services.AddScoped<IUsersStorage, RestUsersStorage>(_ =>
                new RestUsersStorage(new ProfilesApi(Configuration["PROFILES_ADDRESS"])));
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IServiceProvider provider)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "GhostNetwork.Messages.Api v1");
                c.DisplayRequestDuration();
            });

            app.UseCors(x =>
            {
                x.AllowAnyHeader();
                x.AllowAnyMethod();
                x.AllowAnyOrigin();
            });

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}