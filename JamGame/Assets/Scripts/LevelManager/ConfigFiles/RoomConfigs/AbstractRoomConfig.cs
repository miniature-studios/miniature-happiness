using UnityEngine;

public abstract class AbstractRoomConfig : ScriptableObject
{
    public abstract RoomConfig GetRoomConfig();
}