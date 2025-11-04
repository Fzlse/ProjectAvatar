using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ProjectAvatar.API;

[Serializable]
public class PlatformState
{
    public Vector3 position;
    public bool isMoving;
    public float moveDistance;
    public float moveSpeed;
    public bool movingRight;
    public float startPosX;
}

[Serializable]
public class SaveData
{
    public Vector3 playerPosition;
    public int score;
    public float spawnX;
    public Vector3 cameraPosition;
    public Quaternion cameraRotation;
    public float loopBGOffsetX;
    public Vector3 loopBGWorldPos;
    public List<PlatformState> platforms = new List<PlatformState>();
}

public static class SaveSystem
{
    // dipakai GameManager
    public static bool ResumeRequested = false;

    // cache terakhir dari server
    public static SaveData LastLoaded { get; private set; }

    // ====== COMPAT untuk MainMenu ======
    // MainMenu.cs kamu panggil SaveSystem.HasSave(), jadi kita sediakan lagi
    public static bool HasSave()
    {
        // sekarang basisnya di memori, bukan PlayerPrefs
        return LastLoaded != null;
    }

    // kalau mau minta resume dari luar (misal di MainMenu tombol Continue)
    public static void RequestResume()
    {
        ResumeRequested = true;
    }

    // DOWNLOAD dari server
    public static IEnumerator DownloadFromServer(string playerId, Action<SaveData> onOk, Action<string> onErr)
    {
        string path = $"/api/saves/{playerId}";
        yield return ApiClient.GetJson<SaveStateDto>(
            path,
            onOk: dto =>
            {
                if (dto == null)
                {
                    LastLoaded = null;
                    onOk?.Invoke(null);
                    return;
                }

                var data = ConvertFromDto(dto);
                LastLoaded = data;
                onOk?.Invoke(data);
            },
            onErr: err =>
            {
                Debug.LogWarning("[SaveSystem] download gagal: " + err);
                LastLoaded = null;
                onErr?.Invoke(err);
            }
        );
    }

    // UPLOAD ke server
    public static IEnumerator UploadToServer(string playerId, SaveData data, Action onOk, Action<string> onErr)
    {
        var dto = ConvertToDto(data);
        yield return ApiClient.PutJson(
            $"/api/saves/{playerId}",
            dto,
            onOk: () =>
            {
                LastLoaded = data;
                onOk?.Invoke();
            },
            onErr: err =>
            {
                Debug.LogWarning("[SaveSystem] upload gagal: " + err);
                onErr?.Invoke(err);
            }
        );
    }

    // ====== converter DTO <-> SaveData ======
    static SaveData ConvertFromDto(SaveStateDto dto)
    {
        var sd = new SaveData();
        sd.score = dto.score;
        sd.playerPosition = new Vector3(dto.playerPosition.x, dto.playerPosition.y, dto.playerPosition.z);

        if (dto.camera != null)
        {
            sd.cameraPosition = new Vector3(dto.camera.position.x, dto.camera.position.y, dto.camera.position.z);
            sd.cameraRotation = new Quaternion(dto.camera.rotation.x, dto.camera.rotation.y, dto.camera.rotation.z, dto.camera.rotation.w);
        }

        if (dto.level != null)
        {
            sd.spawnX = dto.level.spawnX;
            sd.platforms = new List<PlatformState>();
            if (dto.level.platforms != null)
            {
                foreach (var p in dto.level.platforms)
                {
                    sd.platforms.Add(new PlatformState
                    {
                        position = new Vector3(p.position.x, p.position.y, p.position.z),
                        isMoving = p.isMoving,
                        moveDistance = p.moveDistance,
                        moveSpeed = p.moveSpeed,
                        movingRight = p.movingRight,
                        startPosX = p.startPosX
                    });
                }
            }
        }

        if (dto.loopBG != null)
        {
            sd.loopBGOffsetX = dto.loopBG.offsetX;
            sd.loopBGWorldPos = new Vector3(dto.loopBG.worldPos.x, dto.loopBG.worldPos.y, dto.loopBG.worldPos.z);
        }

        return sd;
    }

    static SaveUpsertReq ConvertToDto(SaveData data)
    {
        var dto = new SaveUpsertReq();
        dto.version = 1;
        dto.score = data.score;
        dto.playerPosition = new Vec3 { x = data.playerPosition.x, y = data.playerPosition.y, z = data.playerPosition.z };
        dto.camera = new SaveCamera
        {
            position = new Vec3 { x = data.cameraPosition.x, y = data.cameraPosition.y, z = data.cameraPosition.z },
            rotation = new Quat { x = data.cameraRotation.x, y = data.cameraRotation.y, z = data.cameraRotation.z, w = data.cameraRotation.w }
        };
        dto.level = new SaveLevel
        {
            spawnX = data.spawnX,
            platforms = new List<ApiPlatformState>()
        };
        if (data.platforms != null)
        {
            foreach (var p in data.platforms)
            {
                dto.level.platforms.Add(new ApiPlatformState
                {
                    position = new Vec3 { x = p.position.x, y = p.position.y, z = p.position.z },
                    isMoving = p.isMoving,
                    moveDistance = p.moveDistance,
                    moveSpeed = p.moveSpeed,
                    movingRight = p.movingRight,
                    startPosX = p.startPosX
                });
            }
        }
        dto.loopBG = new SaveLoop
        {
            offsetX = data.loopBGOffsetX,
            worldPos = new Vec3 { x = data.loopBGWorldPos.x, y = data.loopBGWorldPos.y, z = data.loopBGWorldPos.z }
        };
        return dto;
    }
}
