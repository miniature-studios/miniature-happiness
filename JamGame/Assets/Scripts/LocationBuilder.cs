using Common;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(Location))]
public class LocationBuilder : MonoBehaviour
{
    [SerializeField]
    GameObject scrollView;
    [SerializeField]
    RectTransform scrollViewContentRectTransform;
    [SerializeField]
    GameObject roomUiPrefab;
    [SerializeField]
    GameObject backgroundCell;
    [SerializeField]
    GameObject test_room;
    [SerializeField]
    Location location;
    Transform locationTransform;

    List<LocationCell> locationCells = new();
    
    private void Awake()
    {
        locationTransform = location.GetComponent<Transform>();
        List<Vector2Int> buffer = new();
        for (int i = -5; i <= 5; i++)
        {
            for (int j = -5; j <= 5; j++)
            {
                buffer.Add(new Vector2Int(i, j));
            }
        }
        SetupLevel(buffer, new Dictionary<Vector2Int, RoomType>());
        MoveToBuilderMode(new List<RoomType>() { RoomType.Outside, RoomType.Outside });
    }
    public void SetupLevel(List<Vector2Int> FreePlaces, Dictionary<Vector2Int, RoomType> NecessarilyPlaces)
    {
        foreach (var freePlace in FreePlaces)
        {
            locationCells.Add(Instantiate(backgroundCell, new Vector3(freePlace.x, 0, freePlace.y) * 5, new Quaternion(), locationTransform).GetComponent<LocationCell>());
            if (NecessarilyPlaces.ContainsKey(freePlace))
            {
                //locationCells.Last().BuildPermanentRoom(NecessarilyPlaces[freePlace]);
            }
        }
    }
    public Vector3 GetNearestPlace()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        var hits = Physics.RaycastAll(ray, 150f);

        Vector3 point = hits.ToList().Find(x => x.collider.GetComponent<LocationBuilder>() != null).point;
        return locationCells.Select(x => x.GetComponent<Transform>().position).OrderBy(x => Vector3.Distance(x, point)).First();
    }
    Room ShowedRoom = null;
    public void RenderSelectedRoom(RoomType roomType)
    {
        // TODO: Load Correct Prefab
        ShowedRoom = Instantiate(test_room, GetNearestPlace(), new Quaternion(), locationTransform).GetComponent<Room>();
        // TODO: Call Location's PlaceRoom
    }
    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && ShowedRoom == null)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            var hits = Physics.RaycastAll(ray, 150f);

            if (hits.ToList().FindAll(x => x.collider.GetComponent<Room>()) != null)
            {
                // TODO: Load Correct Prefab
                ShowedRoom = Instantiate(test_room, GetNearestPlace(), new Quaternion(), locationTransform).GetComponent<Room>();
            }
        }
        if(ShowedRoom != null)
        {
            ShowedRoom.transform.position = GetNearestPlace();
            if (Input.GetAxis("Mouse ScrollWheel") > 0f) // forward
            {

            }
            else if (Input.GetAxis("Mouse ScrollWheel") < 0f) // backwards
            {

            }
            if (Input.GetMouseButtonUp(0))
            {
                ShowedRoom = null;
            }
        }
    }
    public void DeleteSelectedRoom()
    {
        Destroy(ShowedRoom.gameObject);
        // TODO: Call Location's TakeRoom.
    }
    public void BuildChoosedRoom()
    {
        LocationCell currentCell = locationCells.Find(x => x.GetComponent<Transform>().position == ShowedRoom.GetComponent<Transform>().position);
        if (!currentCell.IsBusy && !currentCell.IsPermanentBusy)
        {
            currentCell.Bind(ShowedRoom);
        }
    }
    public void MoveToBuilderMode(List<RoomType> roomPrefabsNames)
    {
        scrollView.SetActive(true);
        foreach (var roomPrefabsName in roomPrefabsNames)
        {
            var buffer = Instantiate(roomUiPrefab, scrollViewContentRectTransform);
            RoomUI roomui = buffer.GetComponent<RoomUI>();
            roomui.roomType = roomPrefabsName;
            roomui.LoadImage();
        }
    }
    public void MoveToGameMode()
    {
        scrollView.SetActive(false);
    }
}
