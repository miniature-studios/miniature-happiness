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

    Room SelectedRoom = null;
    public bool selected = false;

    List<Room> allRooms = new List<Room>();
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
        SetupLevel(list);
        MoveToBuilderMode(rooms_ui);
    }
    public void SetupLevel(List<RoomCreationInfo> rooms)
    {
        foreach (var room_cr_info in rooms)
        {
            Room room_link = Instantiate(roomObjectsHandler.room_list.Find(x => x.roomtype == room_cr_info.RoomType).obj, ToGlobal(room_cr_info.Position), GetQ(room_cr_info.Diraction), transform).GetComponent<Room>();
            room_link.position = room_cr_info.Position;
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
                //SelectedRoom.transform.position += new Vector3(0, 1, 0);
            }
        }
        else
        {
            if (selected)
            {
                selected = false;
                //SelectedRoom.transform.position -= new Vector3(0,1,0);
            }
            SelectedRoom = null;
        }
    }
    public void DeleteRoom_ByClick()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        var hits = Physics.RaycastAll(ray, 150f);
        if (hits.ToList().FindAll(x => x.collider.GetComponent<Room>()) != null)
        {
            var select = hits.ToList().Find(x => x.collider.GetComponent<Room>()).collider.GetComponent<Room>();
            if(select.roomType != RoomType.Corridor) Instantiate(roomObjectsHandler.room_ui.Find(x => x.roomtype == select.roomType).obj, scrollViewContentRectTransform);
            Destroy(select.gameObject);
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
        }
    }
    public void RotateSelectedRoom()
    {
        // TODO
    }
    public void AddRoomToScene(RoomType roomType)
    {
        var position = allRooms.Find(x => x.roomType == RoomType.None).GetComponent<Transform>().position;
        var link = allRooms.Find(x => x.transform.position == position);
        Destroy(link.gameObject);
        allRooms.Remove(link);
        allRooms.Add(Instantiate(roomObjectsHandler.room_list.Find(x => x.roomtype == roomType).obj, position, new Quaternion(), transform).GetComponent<Room>());
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
        return new Vector3(input.x, 0, input.y) * 4.1f;
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