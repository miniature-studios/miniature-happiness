using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(Location))]
public class LocationBuilder : MonoBehaviour
{
    [SerializeField] GameObject scrollView;
    [SerializeField] RectTransform scrollViewContentRectTransform;
    [SerializeField] GameObject roomUiPrefab;
    [SerializeField] Location location;
    [SerializeField] WallPlacementRules wallPlacementRules;

    [SerializeField] GameObject test_room;

    Room MovingRoom = null;

    List<RoomSlot> roomsSlots = new();

    private void Awake()
    {
        List<RoomCreationInfo> list = new();
        for (int i = -5; i <= 5; i++)
        {
            for (int j = -5; j <= 5; j++)
            {
                RoomCreationInfo roomCreationInfo = new(ToGlobal(new Vector2Int(i,j)), RoomType.None, Vector2Int.up);
                list.Add(roomCreationInfo);
            }
        }
        SetupLevel(list);
        MoveToBuilderMode(new List<RoomType>() { RoomType.Empty, RoomType.Empty });
    }
    public void SetupLevel(List<RoomCreationInfo> rooms)
    {
        foreach (var room_cr_info in rooms.Where(x => !roomsSlots.Select(y => y.position).Contains(x.Position)))
        {
            // Load Right prefab
            Room room_link = Instantiate(test_room, room_cr_info.Position, GetQ(room_cr_info.Diraction), transform).GetComponent<Room>();
            roomsSlots.Add(new RoomSlot(null, room_link, ToGlobal(room_link.position), room_link.roomType == RoomType.None ? false : true));
            var onr_l = wallPlacementRules.offeredNeighbours.Where(x => room_link.roomType == x.room_from).ToList();
            foreach (var item in onr_l)
            {
                Room room_link1 = Instantiate(test_room, ToGlobal(room_link.position + GetRotateValue(room_link.currentDiraction, item.position)), GetQ(item.diraction), transform).GetComponent<Room>();
                roomsSlots.Add(new RoomSlot(null, room_link1, ToGlobal(room_link1.position), room_link1.roomType == RoomType.None ? false : true));
            }
        }
    }
    Vector2Int GetRotateValue(Vector2Int diraction, Vector2Int target)
    {
        if (diraction == Vector2Int.right)
            return new Vector2Int(target.y, -target.x);
        else if (diraction == Vector2Int.down)
            return new Vector2Int(-target.x, -target.y);
        else if (diraction == Vector2Int.left)
            return new Vector2Int(-target.y, target.x);
        return target;
    }
    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && MovingRoom == null)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            var hits = Physics.RaycastAll(ray, 150f);
            if (hits.ToList().FindAll(x => x.collider.GetComponent<Room>()) != null)
            {
                MovingRoom = hits.ToList().Find(x => x.collider.GetComponent<Room>()).collider.GetComponent<Room>();
                RoomSlot roomSlot = roomsSlots.Find(x => x.targetRoom.position == MovingRoom.position);
                roomSlot.bufferRoom = MovingRoom;
                roomSlot.targetRoom = Instantiate(test_room, roomSlot.position, GetQ(Vector2Int.up), transform).GetComponent<Room>();
            }
        }
        if (MovingRoom != null)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            var hits = Physics.RaycastAll(ray, 150f);
            Vector3 point = hits.ToList().Find(x => x.collider.GetComponent<LocationBuilder>() != null).point;
            Vector3 nearest_point = roomsSlots.Where(x => !x.IsPermanentlyBuilded).OrderBy(x => Vector3.Distance(point, x.position)).Last().position;
            if(nearest_point != roomsSlots.Find(x => x.targetRoom.position == MovingRoom.position).position)
            {
                RoomSlot roomSlot_from = roomsSlots.Find(x => x.bufferRoom.position == MovingRoom.position);
                RoomSlot roomSlot_to = roomsSlots.Find(x => x.position == nearest_point);

                roomSlot_to.bufferRoom = MovingRoom;
                roomSlot_from.bufferRoom = null;

                MovingRoom.position = roomSlot_from.targetRoom.position;
                MovingRoom.transform.position = nearest_point;

                // TODO MOVE BIG ROOM
            }
            if (Input.GetAxis("Mouse ScrollWheel") > 0f)
            {
                RotateSelectedRoom(Vector2Int.right);
            }
            else if (Input.GetAxis("Mouse ScrollWheel") < 0f)
            {
                RotateSelectedRoom(Vector2Int.left);
            }
            if (Input.GetMouseButtonUp(0))
            {
                if (!CheckSelectedRoomForRightPlace())
                    DeleteRoom(MovingRoom);
            }
        }
    }
    bool CheckSelectedRoomForRightPlace()
    {
        RoomSlot roomSlot = roomsSlots.Find(x => x.targetRoom.position == MovingRoom.position);
        if (roomSlot.targetRoom.roomType == RoomType.Empty)
        {
            return true;
        }
        return false;
        // TODO CHECK BIG ROOM
    }
    void RotateSelectedRoom(Vector2Int diraction)
    {
        //TODO ROTATE BIG ROOM
    }
    void UpdateRoomWalls(Vector2Int position)
    {
        
    }
    
    public void DeleteRoom(Room room)
    {
        Destroy(room.gameObject);
        // TODO DELETE BIG OBJECTS
    }
    public void DeleteMovingRoom()
    {
        DeleteRoom(MovingRoom);
    }
    public void AddRoomToScene(RoomType roomType)
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        var hits = Physics.RaycastAll(ray, 150f);
        Vector3 point = hits.ToList().Find(x => x.collider.GetComponent<LocationBuilder>() != null).point;
        Vector3 nearest_point = roomsSlots.Where(x => !x.IsPermanentlyBuilded).OrderBy(x => Vector3.Distance(point, x.position)).Last().position;

        RoomSlot roomSlot = roomsSlots.Find(x => x.position == nearest_point);

        // choose right room Type
        roomSlot.bufferRoom = Instantiate(test_room, nearest_point, GetQ(Vector2Int.up), transform).GetComponent<Room>();
        MovingRoom = roomSlot.bufferRoom;

        // TODO ADD BIG ROOM
    }
    public void BuildChoosedRoom()
    {
        if (CheckSelectedRoomForRightPlace())
        {
            RoomSlot roomSlot = roomsSlots.Find(x => x.targetRoom.position == MovingRoom.position);
            DeleteRoom(roomSlot.targetRoom);
            roomSlot.targetRoom = roomSlot.bufferRoom;
            // TODO PLACE BIG ROOM
        }
        else
        {
            // Load right object
            Instantiate(roomUiPrefab, scrollViewContentRectTransform);
        }
    }
    public void MoveToBuilderMode(List<RoomType> roomTypes)
    {
        scrollView.SetActive(true);
        foreach (var roomType in roomTypes)
        {
            Instantiate(roomUiPrefab, scrollViewContentRectTransform);
        }
    }
    public bool ValidateLocation()
    {
        scrollView.SetActive(false);
        var dict = new Dictionary<Vector2Int, Room>();
        foreach (var slot in roomsSlots)
        {
            slot.targetRoom.position = new Vector2Int((int)slot.position.x, (int)slot.position.z);
            dict.Add(slot.targetRoom.position, slot.targetRoom);
        }
        location.rooms = dict;

        return false;
    }

    Vector3 ToGlobal(Vector2Int input)
    {
        return new Vector3(input.x, 0, input.y) * 5;
    }
    Vector2Int ToLocal(Vector3 input)
    {
        return new Vector2Int((int)input.x, (int)input.z) / 5;
    }
    Quaternion GetQ(Vector2Int diraction)
    {
        return Quaternion.Euler(new Vector3(diraction.x, 0, diraction.y));
    }
    Quaternion RotateQua(Quaternion q, Vector2Int diraction)
    {
        Quaternion q1 = GetQ(diraction);
        return Quaternion.RotateTowards(q, q1, 180);
    }
}
public class RoomSlot
{
    public Room bufferRoom;
    public Room targetRoom;
    public Vector3 position;
    public bool IsPermanentlyBuilded = false;
    public RoomSlot(Room bufferRoom, Room targetRoom, Vector3 position, bool isPermanentlyBuilded)
    {
        this.bufferRoom = bufferRoom;
        this.targetRoom = targetRoom;
        this.position = position;
        IsPermanentlyBuilded = isPermanentlyBuilded;
    }
}
public class RoomCreationInfo
{
    public Vector3 Position;
    public RoomType RoomType;
    public Vector2Int Diraction;
    public RoomCreationInfo(Vector3 position, RoomType roomType, Vector2Int diraction)
    {
        Position = position;
        RoomType = roomType;
        Diraction = diraction;
    }
}
