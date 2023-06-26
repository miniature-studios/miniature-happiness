using Common;
using Level;
using System;
using TMPro;
using UnityEngine;

public class RoomShopUI : MonoBehaviour
{
    [SerializeField]
    private RoomInventoryUI tileUIPrefab;

    [SerializeField]
    private TMP_Text moneyText;

    [SerializeField]
    private TMP_Text waterText;

    [SerializeField]
    private TMP_Text electricityText;

    [SerializeField]
    private TMP_Text roomNameText;
    public RoomInventoryUI RoomInventoryUI => tileUIPrefab;

    private RoomProperties roomProperties;
    private Func<RoomProperties, RoomInventoryUI, Result> roomBuying;

    private void OnValidate()
    {
        UpdateView();
    }

    private void Awake()
    {
        roomBuying = GetComponentInParent<ShopController>().TryBuyRoom;
        UpdateView();
    }

    private void UpdateView()
    {
        if (
            tileUIPrefab.TileUnion.TryGetComponent(out roomProperties)
            && tileUIPrefab.TileUnion.TryGetComponent(out RoomViewProperties roomViewProperties)
        )
        {
            roomNameText.text = roomViewProperties.RoomName;
            moneyText.text = "Money cost: " + Convert.ToString(roomProperties.Cost);
            waterText.text = "Water: " + Convert.ToString(roomProperties.WaterConsumption);
            electricityText.text =
                "Electro: " + Convert.ToString(roomProperties.ElectricityComsumption);
        }
        else
        {
            Debug.LogError($"No room properties in {tileUIPrefab.name}");
        }
    }

    public void TryBuyRoom()
    {
        if (roomBuying(roomProperties, tileUIPrefab).Success)
        {
            Destroy(gameObject);
        }
    }
}
