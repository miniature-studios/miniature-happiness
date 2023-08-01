using Common;
using System;
using TMPro;
using UnityEngine;

namespace Level.Inventory.Room
{
    [AddComponentMenu("Level.Inventory.Room.View")]
    public class View : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text counterText;

        [SerializeField]
        private Model model;

        [SerializeField]
        private GameObject extendetUIInfoPrefab;

        private RectTransform targetInfo = null;
        private Canvas canvas;

        public void Awake()
        {
            canvas = FindObjectOfType<Canvas>();
        }

        public void CountUpdated(int count)
        {
            counterText.text = Convert.ToString(count);
        }

        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.LeftControl))
            {
                switch
                    (
                        RaycastUtilities.PointerIsOverTargetGO(Input.mousePosition, gameObject),
                        targetInfo == null
                    )

                {
                    case (true, true):
                        targetInfo = Instantiate(
                                extendetUIInfoPrefab,
                                Input.mousePosition + new Vector3(20, 20, 0),
                                new Quaternion(),
                                canvas.transform
                            )
                            .GetComponent<RectTransform>();
                        targetInfo.GetComponentInChildren<TMP_Text>().text =
                            $"Electr. Con.: {model.TileUnion.TarrifProperties.ElectricityConsumption}\n"
                            + $"Water Con.: {model.TileUnion.TarrifProperties.WaterConsumption}\n"
                            + $"Cost: {model.TileUnion.Cost.Value}";
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
    }
}
