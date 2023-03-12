using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class LocationBuilder : MonoBehaviour
{
    [SerializeField] GameObject scrollView;
    [SerializeField] RectTransform scrollViewContentRectTransform;
    [SerializeField] Location location;
    [SerializeField] WallPlacementRules wallPlacementRules;

    Room SelectedRoom = null;
    List<Room> allRooms = new List<Room>();
    private void Awake()
    {
        List<RoomCreationInfo> list = new();
        for (int i = -5; i <= 5; i++)
        {
            for (int j = -5; j <= 5; j++)
            {
                RoomCreationInfo roomCreationInfo = new(new Vector2Int(i, j), RoomType.None, Vector2Int.up);
                list.Add(roomCreationInfo);
            }
        }
        SetupLevel(list);
        MoveToBuilderMode(new List<RoomType>() { RoomType.Corridor, RoomType.Corridor });
    }
    public void SetupLevel(List<RoomCreationInfo> rooms)
    {
        foreach (var room_cr_info in rooms)
        {
            GameObject need_room = Resources.Load("room_cr_info.RoomType") as GameObject; // Add loading
            Room room_link = Instantiate(need_room, ToGlobal(room_cr_info.Position), GetQ(room_cr_info.Diraction), transform).GetComponent<Room>();
            room_link.position = room_cr_info.Position;
            allRooms.Add(room_link);
        }
        /*
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
        }*/
    }
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            var hits = Physics.RaycastAll(ray, 150f);
            if (hits.ToList().FindAll(x => x.collider.GetComponent<Room>()) != null)
            {
                SelectedRoom = hits.ToList().Find(x => x.collider.GetComponent<Room>()).collider.GetComponent<Room>();
            }
            else
            {
                SelectedRoom = null;
            }
        }
        if (Input.GetMouseButtonDown(1))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            var hits = Physics.RaycastAll(ray, 150f);
            if (hits.ToList().FindAll(x => x.collider.GetComponent<Room>()) != null)
            {
                var select = hits.ToList().Find(x => x.collider.GetComponent<Room>()).collider.GetComponent<Room>();
                Instantiate(Resources.Load("select.roomType/UI") as GameObject, scrollViewContentRectTransform);
                Destroy(select.gameObject);
            }
        }
        if (SelectedRoom != null)
        {
            if (Input.GetKeyDown(KeyCode.W))
            {
                MoveSelectedRoom(diraction: Vector2Int.up);
            }
            else if (Input.GetKeyDown(KeyCode.A))
            {
                MoveSelectedRoom(diraction: Vector2Int.left);
            }
            else if (Input.GetKeyDown(KeyCode.S))
            {
                MoveSelectedRoom(diraction: Vector2Int.down);
            }
            else if (Input.GetKeyDown(KeyCode.D))
            {
                MoveSelectedRoom(diraction: Vector2Int.right);
            }
            else if (Input.GetKeyDown(KeyCode.R))
            {
                RotateSelectedRoom();
            }
        }
    }
    public void MoveSelectedRoom(Vector2Int diraction)
    {
        Vector3 from = SelectedRoom.transform.position;
        Vector3 to = SelectedRoom.transform.position + ToGlobal(diraction);

        Destroy(allRooms.Find(x => x.transform.position == to).gameObject);
        SelectedRoom.transform.position = to;
        Instantiate(Resources.Load("VoidRoom") as GameObject, from, new Quaternion(), transform);
    }
    public void RotateSelectedRoom()
    {
        // TODO
    }
    public void AddRoomToScene(RoomType roomType)
    {
        var position = allRooms.Find(x => x.roomType == RoomType.None).GetComponent<Transform>().position;
        Destroy(allRooms.Find(x => x.transform.position == position).gameObject);
        Instantiate(Resources.Load("roomType") as GameObject, position, new Quaternion(), transform);
    }
    public void MoveToBuilderMode(List<RoomType> roomTypes)
    {
        scrollView.SetActive(true);
        foreach (var roomType in roomTypes)
        {
            Instantiate(Resources.Load("roomType/UI") as GameObject, scrollViewContentRectTransform);
        }
    }
    public bool ValidateLocation()
    {
        scrollView.SetActive(false);
        var dict = new Dictionary<Vector2Int, Room>();
        foreach (var slot in allRooms)
        {
            // TODO
            //slot.transform.position = new Vector2Int((int)slot.position.x, (int)slot.position.z);
            //dict.Add(slot.targetRoom.position, slot.targetRoom);
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
public class RoomCreationInfo
{
    public Vector2Int Position;
    public RoomType RoomType;
    public Vector2Int Diraction;
    public RoomCreationInfo(Vector2Int position, RoomType roomType, Vector2Int diraction)
    {
        Position = position;
        RoomType = roomType;
        Diraction = diraction;
    }
}
