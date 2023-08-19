using TMPro;
using UnityEngine;

namespace Employee.IncomeGenerator
{
    [AddComponentMenu("Scripts/Employee.IncomeGenerator.IncomeLabelView")]
    public class IncomeLabelView : MonoBehaviour
    {
        [SerializeField] private TMP_Text label;
        [SerializeField] private Animator animator;
        private Camera cam;

        private void Start()
        {
            cam = Camera.main;
        }

        private void Update()
        {
            transform.LookAt(cam.transform.position);
        }

        public void SetValue(int value)
        {
            label.text = "+" + value.ToString();
        }

        public void AnimationEnd()
        {
            gameObject.SetActive(false);
        }
    }
}