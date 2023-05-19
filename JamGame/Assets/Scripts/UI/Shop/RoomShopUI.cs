using System;
using TMPro;
using UnityEngine;

public class RoomShopUI : MonoBehaviour
{
    [SerializeField] private int cost;

    public TileUI TileUnionUIPrefab;
    public TMP_Text MoneyCounter;

    private Func<int, TileUI, bool> RoomBuying;

    public void Awake()
    {
        MoneyCounter.text = Convert.ToString(cost);
    }

    public void Init(Func<int, TileUI, bool> room_buying)
    {
        RoomBuying = room_buying;
    }

    public void TryBuyRoom()
    {
        if (RoomBuying(cost, TileUnionUIPrefab))
        {
            Destroy(gameObject);
        }
    }
}
