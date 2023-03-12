using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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
        for (int i = -3; i <= 3; i++)
        {
            list.Add(new(new Vector2Int(i, 3), RoomType.Outside, Vector2Int.up));
            list.Add(new(new Vector2Int(i, -3), RoomType.Outside, Vector2Int.up));
            list.Add(new(new Vector2Int(3, i), RoomType.Outside, Vector2Int.up));
            list.Add(new(new Vector2Int(-3, i), RoomType.Outside, Vector2Int.up));
        }
        list.Find(x => x.Position == new Vector2Int(0, 2)).RoomType = RoomType.BossRoom1;
        list.Remove(list.Find(x => x.Position == new Vector2Int(1, 2)));
        list.Remove(list.Find(x => x.Position == new Vector2Int(0, 1)));
        list.Remove(list.Find(x => x.Position == new Vector2Int(1, 1)));
        List<RoomType> rooms_ui = new List<RoomType>() { RoomType.Corridor };
        rooms_ui.Add(RoomType.Empty);
        rooms_ui.Add(RoomType.Empty);
        rooms_ui.Add(RoomType.Empty);
        rooms_ui.Add(RoomType.WorkPlace);
        rooms_ui.Add(RoomType.WorkPlace);
        SetupLevel(list);
        MoveToBuilderMode(rooms_ui);
    }
    public void SetupLevel(List<RoomCreationInfo> rooms)
    {
        foreach (var room_cr_info in rooms)
        {
            Room room_link = Instantiate(roomObjectsHandler.room_list.Find(x => x.roomtype == room_cr_info.RoomType).obj, ToGlobal(room_cr_info.Position), new Quaternion(), transform).GetComponent<Room>();
            RotateTransform(room_link.transform, room_cr_info.Diraction);
            allRooms.Add(room_link);
            if (wallPlacementRules.offeredNeighbours.Select(x => x.parent_room).Contains(room_link.roomType))
            {
                List<OfferedNeighbours> offeredNeighbours = wallPlacementRules.offeredNeighbours.Where(x => x.parent_room == room_link.roomType).ToList();
                var main_diraction = room_link.currentDiraction;
                foreach (var neighbor in offeredNeighbours)
                {
                    var spawn_position = room_link.transform.position + RotateByVector(ToGlobal(neighbor.position), main_diraction);
                    Room room_link_1 = Instantiate(roomObjectsHandler.room_list.Find(x => x.roomtype == neighbor.cild_room).obj, spawn_position, new Quaternion(), transform).GetComponent<Room>();
                    RotateTransform(room_link_1.transform, neighbor.diraction);
                    allRooms.Add(room_link_1);
                }
            }
        }
        UpdateAllWalls();
    }
    Vector3 RotateByVector(Vector3 vector, Vector2Int rotation)
    {
        var buffer_rotation = Vector2Int.up;
        while (buffer_rotation != rotation)
            vector = new Vector3(vector.y, 0, -vector.x);
        return vector;
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
        var roomHits = hits.ToList().Where(x => x.collider.GetComponent<Room>() != null);
        if (roomHits.Count() != 0 && !roomObjectsHandler.nonmuvable_rooms.Contains(hits.ToList().Find(x => x.collider.GetComponent<Room>()).collider.GetComponent<Room>().roomType))
        {
            SelectedRoom = hits.ToList().Find(x => x.collider.GetComponent<Room>()).collider.GetComponent<Room>();
            if (!selected)
            {
                selected = true;
                pointer = Instantiate(selector_pointer, SelectedRoom.transform.position, new Quaternion(), transform);
            }
            if (selected)
            {
                Destroy(pointer.gameObject);
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
            if(select == SelectedRoom)
            {
                selected = false;
                Destroy(pointer.gameObject);
            }
            Instantiate(roomObjectsHandler.room_ui.Find(x => x.roomtype == select.roomType).obj, scrollViewContentRectTransform); 
            var new_none = Instantiate(roomObjectsHandler.room_list.Find(x => x.roomtype == RoomType.None).obj, select.transform.position, new Quaternion(), transform).GetComponent<Room>();
            allRooms.Add(new_none);
            Destroy(select.gameObject);
            allRooms.Remove(select);
            UpdateWalls(new_none);
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
            UpdateAllWalls();
        }
    }
    public void UpdateAllWalls()
    {
        foreach (var item in allRooms)
        {
            UpdateWalls(item);
        }
    }
    public void RotateSelectedRoom()
    {
        SelectedRoom.transform.Rotate(0,90,0);
        SelectedRoom.currentDiraction = new Vector2Int(SelectedRoom.currentDiraction.y, -SelectedRoom.currentDiraction.x);
        UpdateWalls(SelectedRoom);
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
    public void ValidateLocation()
    {
        scrollView.SetActive(false);
        var dict = new Dictionary<Vector2Int, Room>();
        foreach (var room in allRooms)
        {
            room.position = ToLocal(room.transform.position / 4.1f);
            dict.Add(room.position, room);
        }
        location.rooms = dict;
        var availableDiractions = new Dictionary<Vector2Int, List<Direction>>();
        foreach (var target_room in allRooms)
        {
            List<Direction> directions = new List<Direction>();

            int delay = GetDelay(target_room.currentDiraction);
            Vector2Int see_diration = Vector2Int.up;
            for (int i = 0; i < 4; i++)
            {
                Vector2Int buffer_diraction;
                Room buffer_room;
                buffer_room = allRooms.Find(x => x.transform.position == target_room.transform.position + ToGlobal(see_diration));

                if (buffer_room == null)
                    continue;

                buffer_diraction = buffer_room.currentDiraction;
                while (buffer_room.transform.position + ToGlobal(buffer_diraction) != target_room.transform.position)
                    buffer_diraction = new Vector2Int(buffer_diraction.y, -buffer_diraction.x);

                int delay_2 = GetDelay(buffer_room.currentDiraction);
                for (int j = 0; j < delay_2; j++)
                {
                    buffer_diraction = new Vector2Int(-buffer_diraction.y, buffer_diraction.x);
                }

                var room_inner_diration = see_diration;
                for (int j = 0; j < delay; j++)
                {
                    room_inner_diration = new Vector2Int(-room_inner_diration.y, room_inner_diration.x);
                }

                bool passable = IsPassable(target_room, room_inner_diration, buffer_room, buffer_diraction);

                if (passable)
                    directions.Add(room_inner_diration.ToDirection());

                see_diration = new Vector2Int(see_diration.y, -see_diration.x);
            }
            availableDiractions.Add(target_room.position, directions);
        }
        location.PathfindingProvider = new PathfindingProvider(availableDiractions);
    }
    public bool IsPassable(Room room1, Vector2Int dir1, Room room2, Vector2Int dir2)
    {
        var ww1 = room1.GetWallVisualizer(dir1);
        var ww2 = room2.GetWallVisualizer(dir2);

        if (ww1.GetActiveWall().IsPassable() && ww2.GetActiveWall().IsPassable())
            return true;
        return false;
    }
    public void UpdateWalls(Room target_room)
    {
        int delay = GetDelay(target_room.currentDiraction);
        Vector2Int see_diration = Vector2Int.up;
        for (int i = 0; i < 4; i++)
        {
            Vector2Int buffer_diraction;
            Room buffer_room;
            buffer_room = allRooms.Find(x => x.transform.position == target_room.transform.position + ToGlobal(see_diration));

            if (buffer_room == null)
                continue;

            buffer_diraction = buffer_room.currentDiraction;
            while (buffer_room.transform.position + ToGlobal(buffer_diraction) != target_room.transform.position)
                buffer_diraction = new Vector2Int(buffer_diraction.y, -buffer_diraction.x);

            int delay_2 = GetDelay(buffer_room.currentDiraction);
            for (int j = 0; j < delay_2; j++)
            {
                buffer_diraction = new Vector2Int(-buffer_diraction.y, buffer_diraction.x);
            }

            var room_inner_diration = see_diration;
            for (int j = 0; j < delay; j++)
            {
                room_inner_diration = new Vector2Int(-room_inner_diration.y, room_inner_diration.x);
            }

            UpdateTwoWalls(target_room, room_inner_diration, buffer_room, buffer_diraction);

            see_diration = new Vector2Int(see_diration.y, -see_diration.x);
        }
    }
    public int GetDelay(Vector2Int vector)
    {
        Vector2Int v1 = Vector2Int.up, v2 = vector;
        int delay = 0;
        while (v2 != v1)
        {
            v2 = new Vector2Int(-v2.y, v2.x);
            delay++;
        }
        return delay;
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
        WallType choose = WallType.None;
        if (awaibleWallTypes.Count == 0)
        {
            Debug.LogError($"No Pair of ({room1.roomType}) and ({room2.roomType})");
        }
        if(awaibleWallTypes.Count > 1)
        {
            var wallPlacmentRule = wallPlacementRules.neighborsLimitation.Find(x => (x.room_a == room1.roomType && x.room_b == room2.roomType) || (x.room_a == room2.roomType && x.room_b == room1.roomType));
            if (wallPlacmentRule != null)
            {
                if (!awaibleWallTypes.Contains(wallPlacmentRule.wall))
                {
                    Debug.LogError($"Invalid rule for pair ({room1.roomType}) and ({room2.roomType})");
                }
                else
                {
                    choose = wallPlacmentRule.wall;
                }
            }
            else
            {
                string str = "";
                foreach (var item in awaibleWallTypes)
                    str += $"\n{item}";

                Debug.LogWarning($"Pair of ({room1.roomType}) and ({room2.roomType}) has {awaibleWallTypes.Count} Variants:" + str);
                choose = awaibleWallTypes.FirstOrDefault();
            }
        }
        if (awaibleWallTypes.Count == 1)
        {
            choose = awaibleWallTypes.First();
        }
        ww1.SetWall(choose);
        ww2.SetWall(choose);
    }
    Vector3 ToGlobal(Vector2Int input)
    {
        return new Vector3(input.x, 0, input.y) * 4.1f;
    }
    Vector2Int ToLocal(Vector3 input)
    {
        return new Vector2Int((int)input.x, (int)input.z);
    }
    void RotateTransform(Transform tr, Vector2Int diraction)
    {
        tr.Rotate(0, 90 * GetDelay(diraction), 0);
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