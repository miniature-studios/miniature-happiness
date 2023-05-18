using Common;
using System.Collections.Generic;
using UnityEngine;

public class ShopController : MonoBehaviour
{
    [SerializeField] private TileBuilderController tileBuilderController;
    [SerializeField] private FinancesController financesController;
    [SerializeField] private Transform roomsUIContainer;
    [SerializeField] private Transform employeeUIContainer;
    public Transform ShopPanel;

    public void CloseShop()
    {
        SetState(UIElementState.Hided);
    }

    public void SetState(UIElementState ui_element_state)
    {
        switch (ui_element_state)
        {
            case UIElementState.Unhided:
                ShopPanel.gameObject.SetActive(true);
                break;
            default:
            case UIElementState.Hided:
                ShopPanel.gameObject.SetActive(false);
                break;
        }
    }

    public void SetShopRooms(List<RoomConfig> room_configs)
    {
        foreach (GameObject child in roomsUIContainer.transform)
        {
            Destroy(child);
        }
        foreach (RoomConfig config in room_configs)
        {
            RoomShopUI shop_ui = Instantiate(config.RoomShopUI, roomsUIContainer);
            shop_ui.Init(TryBuyRoom);
        }
    }

    public bool TryBuyRoom(int cost, TileUI tile_ui)
    {
        if (financesController.MoneyCount - cost < 0)
        {
            financesController.TakeMoney(cost);
            _ = tileBuilderController.tilesPanelController.CreateUIElement(tile_ui);
            return true;
        }
        else
        {
            return false;
        }
    }

    public void SetShopEmployees(List<EmployeeConfig> employee_configs)
    {

    }
}

