using System.Collections.Generic;
using Common;
using Level.Room;
using Level.Shop.View;
using Pickle;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Level.Shop.Room
{
    [AddComponentMenu("Scripts/Level/Shop/Room/Level.Shop.Room.Plank")]
    public class Plank : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IUidHandle
    {
        [SerializeField]
        [RequiredIn(PrefabKind.Variant | PrefabKind.InstanceInScene)]
        [Pickle(LookupType = ObjectProviderType.Assets)]
        private CoreModel coreModelPrefab;
        public InternalUid Uid => coreModelPrefab.Uid;
        private int Cost => coreModelPrefab.ShopModel.Cost.Value;
        private int Rent => coreModelPrefab.RentCost.Value;

        [Required]
        [SerializeField]
        private TMP_Text rentLabel;

        [Required]
        [SerializeField]
        private TMP_Text costLabel;

        [Required]
        [SerializeField]
        private TMP_Text countLabel;

        [ReadOnly]
        [SerializeField]
        private List<CoreModel> coreModels = new();
        public bool IsEmpty => coreModels.Count == 0;
        public int RoomQuantity => coreModels.Count;

        private Controller shopController;
        private ViewImpl mainView;
        private Card createdCard;

        private void Awake()
        {
            shopController = GetComponentInParent<Controller>();
            mainView = GetComponentInParent<ViewImpl>();
        }

        public void AddCard(Card cardPrefab)
        {
            createdCard = Instantiate(cardPrefab, mainView.CardParent);
            createdCard.AssignPlank(this);
            createdCard.transform.position = mainView.CardPosition;
            createdCard.enabled = false;
        }

        public void AddCoreModel(CoreModel coreModel)
        {
            coreModels.Add(coreModel);
            coreModel.transform.SetParent(transform);
        }

        public void RemoveCoreModel(CoreModel coreModel)
        {
            _ = coreModels.Remove(coreModel);
        }

        private void Update()
        {
            costLabel.text = Cost.ToString();
            countLabel.text = RoomQuantity.ToString();
            rentLabel.text = $"{Rent}/day";
        }

        // Called by pressing button.
        public void TryBuyRoom()
        {
            _ = shopController.TryBuyRoom(Uid, Cost);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            createdCard.enabled = true;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            createdCard.enabled = false;
        }

        private void OnDestroy()
        {
            Destroy(createdCard.gameObject);
        }
    }
}
