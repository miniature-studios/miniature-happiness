using System;
using UnityEngine;

[Serializable]
public class SerializedRoomConfig
{
    [SerializeField]
    private string selectedType;

    [SerializeField]
    private FixedRoomConfig fixedRoomConfig;

    [SerializeField]
    private RandomRoomConfig randomRoomConfig;

    public IRoomConfig ToRoomConfig()
    {
        return selectedType switch
        {
            "FixedRoomConfig" => fixedRoomConfig,
            "RandomRoomConfig" => randomRoomConfig,
            _ => throw new NotImplementedException(),
        };
    }
}
