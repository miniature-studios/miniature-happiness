using System.Collections.Generic;
using UnityEngine;

namespace Employee.IncomeGenerator
{
    [AddComponentMenu("Scripts/Employee/IncomeGenerator/Employee.IncomeGenerator.View")]
    public class View : MonoBehaviour
    {
        [SerializeField]
        private Model model;

        [SerializeField]
        private IncomeLabelView labelViewPrototype;
        private List<IncomeLabelView> incomeLabelViewPool;

        private void Start()
        {
            model.NewIncome.AddListener(OnNewIncome);
            incomeLabelViewPool = new List<IncomeLabelView> { labelViewPrototype };
        }

        private void OnNewIncome(int income)
        {
            foreach (IncomeLabelView view in incomeLabelViewPool)
            {
                if (!view.gameObject.activeSelf)
                {
                    view.gameObject.SetActive(true);
                    view.SetValue(income);
                    return;
                }
            }

            IncomeLabelView label_view = Instantiate(labelViewPrototype, transform);
            label_view.SetValue(income);
            incomeLabelViewPool.Add(label_view);
        }
    }
}
