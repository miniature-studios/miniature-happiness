using UnityEngine;

public class BossBar : MonoBehaviour
{
    [SerializeField] private RectTransform rectTransform;
    [SerializeField] private float minValueYPosition;
    [SerializeField] private float maxValueYPosition;
    [SerializeField] private float maxValue = 100;
    [SerializeField] private float changeSpeed = 10;
    public UIHider UIHider;
    private float currentYPosition;
    private float currentYScale;
    private void Awake()
    {
        currentYPosition = maxValueYPosition;
        currentYScale = rectTransform.localScale.y;
        SetBarValue(0);
    }
    public float BarValue { get; private set; }
    public void SetBarValue(float value)
    {
        currentYScale = value / maxValue;
        currentYPosition = (currentYScale * (minValueYPosition - maxValueYPosition)) + minValueYPosition;
        BarValue = value;
    }
    private void Update()
    {
        rectTransform.anchoredPosition3D = Vector3.Lerp(
            rectTransform.anchoredPosition3D,
            new Vector3(
                rectTransform.anchoredPosition3D.x,
                currentYPosition,
                rectTransform.anchoredPosition3D.z
                ),
            Time.deltaTime * changeSpeed
            );
        rectTransform.localScale = Vector3.Lerp(
            rectTransform.localScale,
            new Vector3(
                rectTransform.localScale.x,
                currentYScale,
                rectTransform.localScale.z
                ),
            Time.deltaTime * changeSpeed
            );
    }
}

