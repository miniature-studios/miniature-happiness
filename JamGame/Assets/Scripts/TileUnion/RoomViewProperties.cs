using UnityEngine;

// TODO: Are we need this?
public class RoomViewProperties : MonoBehaviour
{
    [SerializeField]
    private string roomName;
    public string RoomName => roomName;
}
