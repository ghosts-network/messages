﻿using System;
using GhostNetwork.Messages.Users;
using MongoDB.Bson.Serialization.Attributes;

namespace GhostNetwork.Messages.Integrations;

public class UserInfoEntity
{
    [BsonElement("id")]
    public Guid Id { get; set; }

    [BsonElement("fullName")]
    public string FullName { get; set; }

    [BsonElement("avatarUrl")]
    public string AvatarUrl { get; set; }

    public static explicit operator UserInfo(UserInfoEntity entity)
    {
        return entity == null
            ? null
            : new UserInfo(entity.Id, entity.FullName, entity.AvatarUrl);
    }

    public static explicit operator UserInfoEntity(UserInfo userInfo)
    {
        return userInfo == null
            ? null
            : new UserInfoEntity
            {
                Id = userInfo.Id,
                FullName = userInfo.FullName,
                AvatarUrl = userInfo.AvatarUrl
            };
    }
}