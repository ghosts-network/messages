using System;
using GhostNetwork.Messages.Api.Helpers;
using GhostNetwork.Messages.Api.Helpers.OpenApi;
using GhostNetwork.Messages.Chats;
using GhostNetwork.Messages.Messages;
using GhostNetwork.Messages.MongoDb;
using GhostNetwork.Profiles.Api;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpLogging;
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
        private const string DefaultDbName = "messages";

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        private IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddHttpLogging(x =>
            {
                x.LoggingFields = HttpLoggingFields.All;
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

            services.AddScoped<IChatsStorage, MongoChatStorage>();
            services.AddScoped<IChatsService, ChatsService>();
            services.AddScoped<IValidator<ChatContext>, ChatValidator>();

            services.AddScoped<IMessagesStorage, MongoMessageStorage>();
            services.AddScoped<IMessagesService, MessagesService>();
            services.AddScoped<IValidator<MessageContext>, MessageValidator>();

            services.AddScoped<IProfilesApi>(_ => new ProfilesApi(Configuration["PROFILES_ADDRESS"]));
            services.AddScoped<IUserProvider, ProfilesApiUserProvider>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IServiceProvider provider)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpLogging();
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