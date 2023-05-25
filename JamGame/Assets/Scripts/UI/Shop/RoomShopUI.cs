using System;
using TMPro;
using UnityEngine;

public class RoomShopUI : MonoBehaviour
{
    [SerializeField] private RoomInventoryUI tileUIPrefab;
    [SerializeField] private TMP_Text moneyText;
    [SerializeField] private TMP_Text waterText;
    [SerializeField] private TMP_Text electricityText;
    [SerializeField] private TMP_Text roomNameText;
    private RoomProperties roomProperties;

    private Func<RoomProperties, RoomInventoryUI, bool> RoomBuying;

    private void OnValidate()
    {
        if (tileUIPrefab)
        {
            Awake();
        }
    }

    private void Awake()
    {
        if (tileUIPrefab.TileUnionPrefab.TryGetComponent(out roomProperties))
        {
            roomNameText.text = roomProperties.RoomName;
            moneyText.text = "Money cost: " + Convert.ToString(roomProperties.Cost);
            waterText.text = "Water: " + Convert.ToString(roomProperties.WaterConsumption);
            electricityText.text = "Electro: " + Convert.ToString(roomProperties.ElectricityComsumption);
        }
        else
        {
            Debug.LogError($"No room properties in {tileUIPrefab.name}");
        }
    }

    public void Init(Func<RoomProperties, RoomInventoryUI, bool> room_buying)
    {
        RoomBuying = room_buying;
    }

    public void TryBuyRoom()
    {
        if (RoomBuying(roomProperties, tileUIPrefab))
        {
            Destroy(gameObject);
        }
    }
}
