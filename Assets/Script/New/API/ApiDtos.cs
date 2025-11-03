// Assets/Script/New/API/ApiDtos.cs
using System;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectAvatar.API
{
    [Serializable]
    public struct Vec3 { public float x, y, z; }

    [Serializable]
    public struct Quat { public float x, y, z, w; }

    // ==== PERHATIAN ====
    // NAMANYA DIGANTI jadi ApiPlatformState supaya tidak tabrakan
    // dengan PlatformState milik SaveSystem/Level kamu.
    [Serializable]
    public class ApiPlatformState
    {
        public Vec3 position;
        public bool isMoving;
        public float moveDistance;
        public float moveSpeed;
        public bool movingRight;
        public float startPosX;
    }

    [Serializable]
    public class SaveCamera
    {
        public Vec3 position;
        public Quat rotation;
    }

    [Serializable]
    public class SaveLevel
    {
        public float spawnX;
        public List<ApiPlatformState> platforms;
    }

    [Serializable]
    public class SaveLoop
    {
        public float offsetX;
        public Vec3 worldPos;
    }

    [Serializable]
    public class SaveUpsertReq
    {
        public int version;
        public int score;
        public Vec3 playerPosition;
        public SaveCamera camera;
        public SaveLevel level;
        public SaveLoop loopBG;
    }

    [Serializable]
    public class SaveStateDto : SaveUpsertReq { }

    // ===== leaderboard =====

    [Serializable]
    public class SubmitScoreReq
    {
        public string playerId;
        public string playerName;
        public int score;
    }

    [Serializable]
    public class SubmitScoreRes
    {
        public bool ok;
        public int rank;
    }

    [Serializable]
    public class TopEntry
    {
        public int rank;
        public string playerName;
        public int score;
    }

    [Serializable]
    public class TopRes
    {
        public List<TopEntry> entries;
    }
}
