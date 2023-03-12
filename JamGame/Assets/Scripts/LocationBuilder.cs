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
    [SerializeField] RoomObjectsHandler roomObjectsHandler;
    [SerializeField] GameObject selector_pointer;

    Room SelectedRoom = null;
    public bool selected = false;

    List<Room> allRooms = new List<Room>();
    GameObject pointer;
    private void Awake()
    {
        List<RoomCreationInfo> list = new();
        for (int i = -2; i <= 2; i++)
        {
            for (int j = -2; j <= 2; j++)
            {
                RoomCreationInfo roomCreationInfo = new(new Vector2Int(i, j), RoomType.None, Vector2Int.up);
                list.Add(roomCreationInfo);
            }
        }
        List<RoomType> rooms_ui = new List<RoomType>() { RoomType.Corridor };
        rooms_ui.Add(RoomType.Empty);
        rooms_ui.Add(RoomType.Empty);
        rooms_ui.Add(RoomType.Empty);
        SetupLevel(list);
        MoveToBuilderMode(rooms_ui);
    }
    public void SetupLevel(List<RoomCreationInfo> rooms)
    {
        foreach (var room_cr_info in rooms)
        {
            Room room_link = Instantiate(roomObjectsHandler.room_list.Find(x => x.roomtype == room_cr_info.RoomType).obj, ToGlobal(room_cr_info.Position), GetQ(room_cr_info.Diraction), transform).GetComponent<Room>();
            allRooms.Add(room_link);
        }
    }
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            SelectRoom_ByClick();
        }
        if (Input.GetMouseButtonDown(1))
        {
            DeleteRoom_ByClick();
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
    public void SelectRoom_ByClick()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        var hits = Physics.RaycastAll(ray, 150f);
        if (hits.ToList().Where(x => x.collider.GetComponent<Room>() != null).Count() != 0)
        {
            SelectedRoom = hits.ToList().Find(x => x.collider.GetComponent<Room>()).collider.GetComponent<Room>();
            if (!selected)
            {
                selected = true;
                pointer = Instantiate(selector_pointer, SelectedRoom.transform.position, new Quaternion(), transform);
            }
        }
        else
        {
            if (selected)
            {
                selected = false;
                Destroy(pointer.gameObject);
            }
            SelectedRoom = null;
        }
    }
    public void DeleteRoom_ByClick()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        var hits = Physics.RaycastAll(ray, 150f);
        if (hits.ToList().FindAll(x => x.collider.GetComponent<Room>()).Count() != 0)
        {
            var select = hits.ToList().Find(x => x.collider.GetComponent<Room>()).collider.GetComponent<Room>();
            if (select.roomType != RoomType.Corridor)
            { 
                Instantiate(roomObjectsHandler.room_ui.Find(x => x.roomtype == select.roomType).obj, scrollViewContentRectTransform); 
            }
            allRooms.Add(Instantiate(roomObjectsHandler.room_list.Find(x => x.roomtype == RoomType.None).obj, select.transform.position, new Quaternion(), transform).GetComponent<Room>());
            Destroy(select.gameObject);
            UpdateWalls(allRooms.Last());
        }
    }
    public void MoveSelectedRoom(Vector2Int diraction)
    {
        Vector3 from = SelectedRoom.transform.position;
        Vector3 to = SelectedRoom.transform.position + ToGlobal(diraction);

        var found = allRooms.Find(x => x.transform.position == to);
        if (found != null && !roomObjectsHandler.nonmuvable_rooms.Contains(found.roomType))
        {
            found.transform.position = from;
            SelectedRoom.transform.position = to;
            pointer.transform.position += ToGlobal(diraction);
            UpdateWalls(found);
            UpdateWalls(SelectedRoom);
        }
    }
    public void RotateSelectedRoom()
    {
        SelectedRoom.transform.rotation.eulerAngles.Set(SelectedRoom.transform.rotation.eulerAngles.x, SelectedRoom.transform.rotation.eulerAngles.y + 90, SelectedRoom.transform.rotation.eulerAngles.z);
        SelectedRoom.currentDiraction = new Vector2Int(SelectedRoom.currentDiraction.y, -SelectedRoom.currentDiraction.x);
    }
    public void AddRoomToScene(RoomType roomType)
    {
        var position = allRooms.Find(x => x.roomType == RoomType.None).GetComponent<Transform>().position;
        var link = allRooms.Find(x => x.transform.position == position);
        Destroy(link.gameObject);
        allRooms.Remove(link);
        allRooms.Add(Instantiate(roomObjectsHandler.room_list.Find(x => x.roomtype == roomType).obj, position, new Quaternion(), transform).GetComponent<Room>());
        UpdateWalls(allRooms.Last());
    }
    public void MoveToBuilderMode(List<RoomType> roomTypes)
    {
        scrollView.SetActive(true);
        foreach (var roomType in roomTypes)
        {
            Instantiate(roomObjectsHandler.room_ui.Find(x => x.roomtype == roomType).obj, scrollViewContentRectTransform);
        }
    }
    public bool ValidateLocation()
    {
        scrollView.SetActive(false);
        var dict = new Dictionary<Vector2Int, Room>();
        foreach (var room in allRooms)
        {
            room.position = ToLocal(room.transform.position / 4.1f);
            dict.Add(room.position, room);
        }
        location.rooms = dict;
        return true;
    }
    public void UpdateWalls(Room target_room)
    {
        for (int i = 0; i < 4; i++)
        {
            Vector2Int room_diraction = target_room.currentDiraction;
            Vector2Int buffer_diraction;
            Room buffer_room;

            buffer_room = allRooms.Find(x => x.transform.position == target_room.transform.position + ToGlobal(room_diraction) * 4.1f);

            if (buffer_room != null)
            {
                buffer_diraction = buffer_room.currentDiraction;
                while (buffer_room.transform.position + ToGlobal(buffer_diraction) != target_room.transform.position)
                    buffer_diraction = new Vector2Int(buffer_diraction.y, -buffer_diraction.x);

                UpdateTwoWalls(target_room, room_diraction, buffer_room, buffer_diraction);
            }
            else
            {
                UpdateWall(target_room, room_diraction);
            }
            target_room.currentDiraction = new Vector2Int(target_room.currentDiraction.y, -target_room.currentDiraction.x);
        }
    }
    public void UpdateTwoWalls(Room room1, Vector2Int dir1, Room room2, Vector2Int dir2)
    {
        var ww1 = room1.GetWallVisualizer(dir1);
        var ww2 = room2.GetWallVisualizer(dir2);
        List<WallType> awaibleWallTypes = new();
        foreach (var item in ww1.GetAvailableWalls())
        {
            if(ww2.GetAvailableWalls().Contains(item))
                awaibleWallTypes.Add(item);
        }
        if (awaibleWallTypes.Count == 0)
            Debug.LogWarning("NO MATCH");
        if(awaibleWallTypes.Count > 1)
        {
            var v = wallPlacementRules.neighborsLimitation.Find(x => x.room_a == room1.roomType && x.room_b == room2.roomType || x.room_a == room2.roomType && x.room_b == room1.roomType);
            foreach (var item in awaibleWallTypes)
            {
                if(item != v.wall)
                    awaibleWallTypes.Remove(item);
            }
        }
        ww1.SetWall(awaibleWallTypes[0]);
        ww2.SetWall(awaibleWallTypes[0]);
    }
    public void UpdateWall(Room room1, Vector2Int dir1)
    {
        var ww1 = room1.GetWallVisualizer(dir1);
        ww1.SetWall(WallType.Window);
    }
    Vector3 ToGlobal(Vector2Int input)
    {
        return new Vector3(input.x, 0, input.y) * 4.1f;
    }
    Vector2Int ToLocal(Vector3 input)
    {
        return new Vector2Int((int)input.x, (int)input.z);
    }
    Quaternion GetQ(Vector2Int diraction)
    {
        return Quaternion.Euler(new Vector3(diraction.x, 0, diraction.y));
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