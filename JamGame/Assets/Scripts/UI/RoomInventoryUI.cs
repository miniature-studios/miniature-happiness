using System;
using TMPro;
using UnityEngine;

public class RoomInventoryUI : MonoBehaviour
{
    public TileUnion TileUnion;

    [SerializeField]
    private TMP_Text counterText;
    private int counter = 1;
    public int Counter
    {
        get => counter;
        set
        {
            counterText.text = Convert.ToString(value);
            counter = value;
            if (counter <= 0)
            {
                Destroy(gameObject);
            }
        }
    }
}
