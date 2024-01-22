using Level.Room;
using Pickle;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Level.Inventory.Room
{
    [AddComponentMenu("Scripts/Level/Inventory/Room/Level.Inventory.Room.View")]
    public class View : MonoBehaviour, IPointerExitHandler, IPointerEnterHandler
    {
        [Pickle(LookupType = ObjectProviderType.Assets)]
        public CoreModel CoreModelPrefab;

        public string Uid => CoreModelPrefab.Uid;

        [SerializeField]
        private ExtendedView extendedView;

        [ReadOnly]
        [SerializeField]
        private CoreModel coreModel;
        public CoreModel CoreModel => coreModel;

        private bool isHovered = false;
        private RectTransform canvas;
        private InputActions inputActions;

        private void Awake()
        {
            inputActions = new();
            canvas = FindObjectOfType<Canvas>().GetComponent<RectTransform>();
        }

        public void SetCoreModel(CoreModel coreModel)
        {
            this.coreModel = coreModel;
        }

        public void Update()
        {
            if (inputActions.UI.ExtendInventoryTileInfo.IsPressed() && isHovered)
            {
                if (!extendedView.gameObject.activeSelf)
                {
                    extendedView.gameObject.SetActive(true);
                    extendedView.transform.SetParent(canvas);
                }
                extendedView.SetLabelText($"Rent: {CoreModel.RentCost.Value}");
                extendedView.transform.position = Input.mousePosition + new Vector3(20, 20, 0);
            }
            else if (extendedView.gameObject.activeSelf)
            {
                extendedView.gameObject.SetActive(false);
                extendedView.transform.SetParent(transform);
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            isHovered = true;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            isHovered = false;
        }
    }
}
