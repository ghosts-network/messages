using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using GhostNetwork.Profiles.Api;
using GhostNetwork.Profiles.Client;
using GhostNetwork.Profiles.Model;
using Microsoft.Extensions.Logging;

namespace GhostNetwork.Messages.Api.Helpers;

public class ProfilesApiUserProvider : IUserProvider
{
    private readonly IProfilesApi profilesApi;
    private readonly ILogger logger;

    public ProfilesApiUserProvider(IProfilesApi profilesApi, ILogger<ProfilesApiUserProvider> logger)
    {
        this.profilesApi = profilesApi;
        this.logger = logger;
    }

    public async Task<UserInfo> GetByIdAsync(string id)
    {
        if (!Guid.TryParse(id, out var guid))
        {
            return null;
        }

        try
        {
            var result = await profilesApi.GetByIdAsync(guid);
            return result == null
                ? null
                : new UserInfo(result.Id, $"{result.FirstName} {result.LastName}", result.ProfilePicture);
        }
        catch (ApiException ex) when (ex.ErrorCode == (int)HttpStatusCode.NotFound)
        {
            logger.LogInformation("Method GetByIdAsync trowed new exception: {ex}", ex);
            return null;
        }
    }

    public async Task<IEnumerable<UserInfo>> SearchAsync(List<string> ids)
    {
        var usersIds = new List<Guid>();

        foreach (var id in ids)
        {
            if (Guid.TryParse(id, out var guid))
            {
                usersIds.Add(guid);
            }
        }

        if (!usersIds.Any())
        {
            return Enumerable.Empty<UserInfo>();
        }

        try
        {
            var profiles = await profilesApi.SearchByIdsAsync(new ProfilesQueryModel(usersIds));

            return profiles.Any() ? profiles.Select(x => new UserInfo(x.Id, $"{x.FirstName} {x.LastName}", x.ProfilePicture)).ToList() : Enumerable.Empty<UserInfo>();
        }
        catch (ApiException ex) when (ex.ErrorCode == (int)HttpStatusCode.NotFound)
        {
            logger.LogInformation("Method SearchAsync trowed new exception: {ex}", ex);
            return Enumerable.Empty<UserInfo>();
        }
    }
}