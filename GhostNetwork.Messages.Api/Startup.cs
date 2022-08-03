using System;
using System.Net.Mime;
using GhostNetwork.Messages.Api.Domain.Chats;
using GhostNetwork.Messages.Api.Domain.Messages;
using GhostNetwork.Messages.Api.Domain.Users;
using GhostNetwork.Messages.Api.Handlers.Messages;
using GhostNetwork.Messages.Api.Integrations;
using GhostNetwork.Messages.Api.Integrations.Chats;
using GhostNetwork.Messages.Api.Integrations.Users;
using GhostNetwork.Messages.Integrations;
using GhostNetwork.Messages.Integrations.Messages;
using GhostNetwork.Profiles.Api;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using MongoDB.Driver;

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
            services.AddRouting();
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "GhostNetwork.Messages",
                    Description = "Http client for GhostNetwork.Messages",
                    Version = "1.0.0",
                });
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
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "GhostNetwork.Messages.Api v1");
                c.DisplayRequestDuration();
            });

            app.UseHttpsRedirection();
            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints
                    .MapGet("/chats/{chatId}/messages/{messageId}", GetByIdHandler.HandleAsync)
                    .Produces<Message>(contentType: MediaTypeNames.Application.Json)
                    .Produces(StatusCodes.Status404NotFound, contentType: MediaTypeNames.Application.Json)
                    .WithName("Messages_GetById")
                    .WithTags("Messages");

                endpoints
                    .MapGet("/chats/{chatId}/messages", SearchHandler.HandleAsync)
                    .Produces<Message[]>(contentType: MediaTypeNames.Application.Json)
                    .WithName("Messages_Search")
                    .WithTags("Messages");

                endpoints
                    .MapPost("/chats/{chatId}/messages", CreateHandler.HandleAsync)
                    .Produces<Message>(StatusCodes.Status201Created, contentType: MediaTypeNames.Application.Json)
                    .ProducesValidationProblem(contentType: MediaTypeNames.Application.Json)
                    .WithName("Messages_Create")
                    .WithTags("Messages");

                endpoints
                    .MapPut("/chats/{chatId}/messages/{messageId}", UpdateHandler.HandleAsync)
                    .Produces(StatusCodes.Status204NoContent)
                    .ProducesValidationProblem(contentType: MediaTypeNames.Application.Json)
                    .Produces(StatusCodes.Status404NotFound, contentType: MediaTypeNames.Application.Json)
                    .WithName("Messages_Update")
                    .WithTags("Messages");

                endpoints
                    .MapDelete("/chats/{chatId}/messages/{messageId}", DeleteHandler.HandleAsync)
                    .Produces(StatusCodes.Status204NoContent, contentType: MediaTypeNames.Application.Json)
                    .Produces(StatusCodes.Status404NotFound, contentType: MediaTypeNames.Application.Json)
                    .WithName("Messages_Delete")
                    .WithTags("Messages");

                endpoints
                    .MapGet("/chats/{id}", Handlers.Chats.GetByIdHandler.HandleAsync)
                    .Produces<Chat>(contentType: MediaTypeNames.Application.Json)
                    .Produces(StatusCodes.Status404NotFound, contentType: MediaTypeNames.Application.Json)
                    .WithName("Chats_GetById")
                    .WithTags("Chats");

                endpoints
                    .MapGet("/chats", Handlers.Chats.SearchHandler.HandleAsync)
                    .Produces<Chat[]>(contentType: MediaTypeNames.Application.Json)
                    .WithName("Chats_Search")
                    .WithTags("Chats");

                endpoints
                    .MapPost("/chats", Handlers.Chats.CreateHandler.HandleAsync)
                    .Produces<Chat>(StatusCodes.Status201Created, contentType: MediaTypeNames.Application.Json)
                    .ProducesValidationProblem(contentType: MediaTypeNames.Application.Json)
                    .WithName("Chats_Create")
                    .WithTags("Chats");

                endpoints
                    .MapPut("/chats/{id}", Handlers.Chats.UpdateHandler.HandleAsync)
                    .Produces(StatusCodes.Status204NoContent)
                    .ProducesValidationProblem(contentType: MediaTypeNames.Application.Json)
                    .Produces(StatusCodes.Status404NotFound, contentType: MediaTypeNames.Application.Json)
                    .WithName("Chats_Update")
                    .WithTags("Chats");

                endpoints
                    .MapDelete("/chats/{id}", Handlers.Chats.DeleteHandler.HandleAsync)
                    .Produces(StatusCodes.Status204NoContent, contentType: MediaTypeNames.Application.Json)
                    .Produces(StatusCodes.Status404NotFound, contentType: MediaTypeNames.Application.Json)
                    .WithName("Chats_Delete")
                    .WithTags("Chats");
            });
        }
    }
}