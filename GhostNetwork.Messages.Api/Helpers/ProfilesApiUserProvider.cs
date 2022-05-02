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
            logger.LogError("Method GetByIdAsync trowed new exception: {ex}", ex);
            return null;
        }
    }

    public async Task<List<UserInfo>> SearchAsync(List<string> ids)
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
            return default;
        }

        try
        {
            var result = await profilesApi.SearchByIdsAsync(new ProfilesQueryModel(usersIds));

            if (result.Any())
            {
                return result.Select(x => new UserInfo(x.Id, $"{x.FirstName} {x.LastName}", x.ProfilePicture)).ToList();
            }
        }
        catch (ApiException ex) when (ex.ErrorCode == (int)HttpStatusCode.NotFound)
        {
            logger.LogError("Method SearchAsync trowed new exception: {ex}", ex);
            return null;
        }

        return null;
    }
}