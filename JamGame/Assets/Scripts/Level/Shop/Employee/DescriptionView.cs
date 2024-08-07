using Common;
using Employee.Personality;
using Pickle;
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
        [SerializeField]
        [Pickle(typeof(DescriptionQuirkLine), LookupType = ObjectProviderType.Assets)]
        private DescriptionQuirkLine descriptionQuirkLinePrefab;

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

            descriptionQuirkLineParent.DestroyChildren();
            if (card.EmployeeConfig.Quirks.Count == 0)
            {
                descriptionQuirkLineParent.gameObject.SetActive(false);
            }
            else
            {
                descriptionQuirkLineParent.gameObject.SetActive(true);
                foreach (Quirk quirk in card.EmployeeConfig.Quirks)
                {
                    DescriptionQuirkLine quirkIcon = Instantiate(
                        descriptionQuirkLinePrefab,
                        descriptionQuirkLineParent
                    );
                    quirkIcon.FillData(card.QuirksByUid[quirk.Uid]);
                }
            }
        }
    }
}
