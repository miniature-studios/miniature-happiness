using System.Collections.Generic;
using UnityEngine;

public class ShopController : MonoBehaviour
{
    [SerializeField] private InventoryUIController tilesPanelController;
    [SerializeField] private Finances financesController;
    [SerializeField] private Transform roomsUIContainer;
    private Animator shopAnimator;

    private void Awake()
    {
        shopAnimator = GetComponent<Animator>();
    }

    public void OpenShop()
    {
        shopAnimator.SetBool("Showed", true);
    }

    public void CloseShop()
    {
        shopAnimator.SetBool("Showed", false);
    }

    public void SetShopRooms(IEnumerable<RoomConfig> room_configs)
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

    public bool TryBuyRoom(RoomProperties roomProporties, RoomInventoryUI tile_ui)
    {
        if (financesController.MoneyCount - roomProporties.Cost > 0)
        {
            financesController.TakeMoney(roomProporties.Cost);
            _ = tilesPanelController.CreateUIElement(tile_ui);
            return true;
        }
        else
        {
            Debug.Log("Not enough money");
            return false;
        }
    }

    public void SetShopEmployees(IEnumerable<EmployeeConfig> employee_configs)
    {
        // TODO
    }

    public bool TryBuyEmployee(int cost, RoomInventoryUI tile_ui)
    {
        // TODO
        throw new System.NotImplementedException();
    }
}

