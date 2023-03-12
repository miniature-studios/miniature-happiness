using Common;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RoomObjectsHandler", menuName = "ScriptableObjects/RoomObjectsHandler", order = 3)]
public class RoomObjectsHandler : ScriptableObject
{
    public List<MatchRoomTypes> room_list;
    public List<MatchRoomUi> room_ui;
    public List<RoomType> nonmuvable_rooms;
}
[Serializable]
public class MatchRoomTypes
{
    public RoomType roomtype;
    public GameObject obj;
}
[Serializable]
public class MatchRoomUi
{
    public RoomType roomtype;
    public GameObject obj;
}
