using Common;
using Level.Room;
using Pickle;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

namespace Level.Shop.Room
{
    [AddComponentMenu("Scripts/Level/Shop/Room/Level.Shop.Room.Card")]
    public class Card : MonoBehaviour, IUidHandle
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
        private TMP_Text costLabel;

        [Required]
        [SerializeField]
        private TMP_Text rentLabel;

        [Required]
        [SerializeField]
        private TMP_Text quantityLabel;

        private RectTransform rectTransform;
        public RectTransform RectTransform => rectTransform;

        private Plank plank;

        public void Initialize()
        {
            rectTransform = GetComponent<RectTransform>();
        }

        public void AssignPlank(Plank plank)
        {
            this.plank = plank;
        }

        private void Update()
        {
            costLabel.text = Cost.ToString();
            rentLabel.text = $"{Rent}/day";
            quantityLabel.text = plank.RoomQuantity.ToString();
        }
    }
}
