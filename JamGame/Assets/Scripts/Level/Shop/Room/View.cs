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
    [AddComponentMenu("Scripts/Level/Shop/Room/Level.Shop.Room.View")]
    public class View : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
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

        private Controller shopController;
        private ViewImpl mainView;

        public void AddCoreModel(CoreModel coreModel)
        {
            coreModels.Add(coreModel);
            coreModel.transform.SetParent(transform);
        }

        public void RemoveCoreModel(CoreModel coreModel)
        {
            _ = coreModels.Remove(coreModel);
        }

        private void Awake()
        {
            shopController = GetComponentInParent<Controller>();
            mainView = GetComponentInParent<ViewImpl>();
        }

        private void Update()
        {
            costLabel.text = $"Money cost: {Cost}";
            countLabel.text = coreModels.Count.ToString();
            rentLabel.text = $"{Rent}/day";
        }

        // Called by pressing button.
        public void TryBuyRoom()
        {
            _ = shopController.TryBuyRoom(Uid, Cost);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            //mainView.CardParent
            //
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            //
        }
    }
}
