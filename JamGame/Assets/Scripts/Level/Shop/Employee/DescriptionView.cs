using Common;
using Employee.Personality;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

namespace Level.Shop.Employee
{
    [AddComponentMenu("Scripts/Level/Shop/Employee/Level.Shop.Employee.DescriptionView")]
    internal class DescriptionView : MonoBehaviour
    {
        [Required]
        [SerializeField]
        private TMP_Text nameLabel;

        [Required]
        [SerializeField]
        private TMP_Text hireCostLabel;

        [Required]
        [SerializeField]
        private TMP_Text professionLabel;

        [Required]
        [AssetsOnly]
        [SerializeField]
        private DescriptionQuirkLine descriptionQuirkLineAsset;

        [Required]
        [SerializeField]
        private Transform descriptionQuirkLineParent;

        public void UpdateData(CardView card)
        {
            if (card == null)
            {
                gameObject.SetActive(false);
                return;
            }

            gameObject.SetActive(true);
            nameLabel.text = card.EmployeeConfig.Name;
            hireCostLabel.text = card.EmployeeConfig.HireCost.ToString();
            professionLabel.text = card.EmployeeConfig.Profession;

            descriptionQuirkLineParent.DestroyChildsImmediate();
            if (card.EmployeeConfig.Quirks.Count == 0)
            {
                descriptionQuirkLineParent.gameObject.SetActive(false);
            }
            else
            {
                foreach (Quirk quirk in card.EmployeeConfig.Quirks)
                {
                    DescriptionQuirkLine quirkIcon = Instantiate(
                        descriptionQuirkLineAsset,
                        descriptionQuirkLineParent
                    );
                    quirkIcon.FillData(card.QuirkConfigsByUid[quirk.Uid]);
                }
            }
        }
    }
}
