using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PlatformState
{
    public Vector3 position;
    public bool isMoving;
    public float moveDistance;
    public float moveSpeed;
    public bool movingRight;
    public float startPosX; // titik tengah osilasi horizontal
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
    private const string Key = "SaveData_V1";
    public static bool ResumeRequested = false;

    public static void Save(SaveData data)
    {
        string json = JsonUtility.ToJson(data);
        PlayerPrefs.SetString(Key, json);
        PlayerPrefs.Save();
    }

    public static bool HasSave() => PlayerPrefs.HasKey(Key);

    public static SaveData Load()
    {
        if (!HasSave()) return null;
        string json = PlayerPrefs.GetString(Key);
        return JsonUtility.FromJson<SaveData>(json);
    }

    public static void Clear()
    {
        PlayerPrefs.DeleteKey(Key);
    }
}
