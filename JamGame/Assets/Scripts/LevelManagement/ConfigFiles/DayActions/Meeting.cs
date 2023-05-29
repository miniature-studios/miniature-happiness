using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class Meeting : IDayAction
{
    [SerializeField] private List<SerializedEmployeeConfig> shopEmployees;
    [SerializeField] private List<SerializedRoomConfig> shopRooms;

    public IEnumerable<EmployeeConfig> ShopEmployees
        => shopEmployees.Select(x => x.ToEmployeeConfig().GetEmployeeConfig());
    public IEnumerable<RoomConfig> ShopRooms
        => shopRooms.Select(x => x.ToRoomConfig().GetRoomConfig());
}
