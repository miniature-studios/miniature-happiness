using Common;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using System;

[RequireComponent(typeof(Location))]
public class LocationBuilder : MonoBehaviour
{
    [SerializeField] GameObject scrollView;
    [SerializeField] RectTransform scrollViewContentRectTransform;
    [SerializeField] GameObject roomUiPrefab;
    [SerializeField] Location location;
    [SerializeField] WallPlacementRules wallPlacementRules;
    Transform locationTransform;

    [SerializeField] GameObject test_room;

    Room MovingRoom = null;

    List<RoomSlot> roomsSlots = new();
    
    private void Awake()
    {
        locationTransform = location.GetComponent<Transform>();
        List<RoomCreationInfo> list = new();
        for (int i = -5; i <= 5; i++)
        {
            for (int j = -5; j <= 5; j++)
            {
                RoomCreationInfo roomCreationInfo = new()
                {
                    Position = new Vector3(i, 0, j) * 5,
                    Diraction = Vector2Int.up,
                    RoomType = RoomType.None
                };
                list.Add(roomCreationInfo);
            }
        }
        SetupLevel(list);
        MoveToBuilderMode(new List<RoomType>() { RoomType.Empty, RoomType.Empty });
    }
    public void SetupLevel(List<RoomCreationInfo> rooms)
    {
        foreach (var room in rooms)
        {
            roomsSlots.AddRange(InstantiateRoom(room));
        }
    }
    public Vector3 GetNearestPlace()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        var hits = Physics.RaycastAll(ray, 150f);

        Vector3 point = hits.ToList().Find(x => x.collider.GetComponent<LocationBuilder>() != null).point;
        //return locationCells.Where(x => !x.IsPermanentBusy).Select(x => x.GetComponent<Transform>().position).OrderBy(x => Vector3.Distance(x, point)).First();
        throw new NotImplementedException();
    }
    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && MovingRoom == null)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            var hits = Physics.RaycastAll(ray, 150f);
            MovingRoom = hits.ToList().Find(x => x.collider.GetComponent<Room>()).collider.GetComponent<Room>();
        }
        if(MovingRoom != null)
        {
            MovingRoom.transform.position = GetNearestPlace();
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
                    DeleteSelectedRoom();
            }
        }
    }
    bool CheckSelectedRoomForRightPlace()
    {
        return false;
    }
    void RotateSelectedRoom(Vector2Int diraction)
    {

    }
    void UpdateRoomWalls(Vector2Int position)
    {

    }
    public List<RoomSlot> InstantiateRoom(RoomCreationInfo roomInfo)
    {
        // TODO: Load Correct Prefab
        MovingRoom = Instantiate(test_room, GetNearestPlace(), new Quaternion(), locationTransform).GetComponent<Room>();
        // TODO: Call Location's PlaceRoom
        return null;
    }
    public void DeleteSelectedRoom()
    {
        Destroy(MovingRoom.gameObject);
        // TODO: Call Location's TakeRoom.
    }
    public void BuildChoosedRoom()
    {
        //LocationCell currentCell = locationCells.Find(x => x.GetComponent<Transform>().position == ShowedRoom.GetComponent<Transform>().position);
        //if (!currentCell.IsBusy && !currentCell.IsPermanentBusy)
        //{
        //    currentCell.Bind(ShowedRoom);
        //}
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
            dict.Add(slot.targetRoom.position, slot.targetRoom);
        }
        location.rooms = dict;
        
        return false;
    }
}
public class RoomSlot
{
    public Room targetRoom;
    public bool IsBuilded = false;
    public bool IsPermanentlyBuilded = false;
}
public class RoomCreationInfo
{
    public Vector3 Position;
    public RoomType RoomType;
    public Vector2Int Diraction;
}
