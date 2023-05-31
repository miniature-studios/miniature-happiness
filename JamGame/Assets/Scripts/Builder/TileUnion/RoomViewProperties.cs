using UnityEngine;

public class RoomViewProperties : MonoBehaviour
{
    [SerializeField] private string roomName;
    public string RoomName => roomName;
}
