using Common;
using Level.Room;
using Pickle;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Level.Inventory.Room
{
    [AddComponentMenu("Scripts/Level.Inventory.Room.View")]
    public class View : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [Pickle(LookupType = ObjectProviderType.Assets)]
        public CoreModel CoreModelPrefab;

        [SerializeField]
        [InspectorReadOnly]
        private string hashCode;
        public string HashCode
        {
            get => hashCode;
            set => hashCode = value;
        }

        [SerializeField]
        private GameObject extendedUIInfoPrefab;

        private RectTransform targetInfo = null;
        private Canvas canvas;

        private Func<CoreModel> getCoreModelInstance;
        public Func<CoreModel> GetCoreModelInstance => getCoreModelInstance;

        private bool pointerIsOver;

        public void Awake()
        {
            canvas = FindObjectOfType<Canvas>();
        }

        public void Constructor(Func<CoreModel> getCoreModelInstance)
        {
            this.getCoreModelInstance = getCoreModelInstance;
        }

        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.LeftControl))
            {
                switch (pointerIsOver, targetInfo == null)
                {
                    case (true, true):
                        targetInfo = Instantiate(
                                extendedUIInfoPrefab,
                                Input.mousePosition + new Vector3(20, 20, 0),
                                new Quaternion(),
                                canvas.transform
                            )
                            .GetComponent<RectTransform>();
                        targetInfo.GetComponentInChildren<TMP_Text>().text =
                            $"Electricity. Con.: {getCoreModelInstance().TariffProperties.ElectricityConsumption}\n"
                            + $"Water Con.: {getCoreModelInstance().TariffProperties.WaterConsumption}\n"
                            + $"Cost: {getCoreModelInstance().ShopModel.Cost}";
                        break;
                    case (true, false):
                        targetInfo.position = Input.mousePosition + new Vector3(20, 20, 0);
                        break;
                    case (false, false):
                        Destroy(targetInfo.gameObject);
                        break;
                }
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            pointerIsOver = true;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            pointerIsOver = false;
        }
    }
}
