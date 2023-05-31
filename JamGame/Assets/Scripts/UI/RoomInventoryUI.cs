using System;
using TMPro;
using UnityEngine;

public class RoomInventoryUI : MonoBehaviour
{
    public TileUnion TileUnion;
    [SerializeField] private TMP_Text counter;
    private int _counter = 1;
    public int Counter
    {
        get => _counter;
        set
        {
            counter.text = Convert.ToString(value);
            _counter = value;
            if (_counter <= 0)
            {
                Destroy(gameObject);
            }
        }
    }

}
