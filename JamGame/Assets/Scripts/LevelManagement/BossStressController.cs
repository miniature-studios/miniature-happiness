using UnityEngine;

public class BossStressController : MonoBehaviour
{
    [SerializeField] private BossBar bossBar;

    [HideInInspector] public float StressSpeed = 0;

    public UIHider UIHider => bossBar.UIHider;
    public BossStressState StressState { get; set; } = BossStressState.Freezed;

    private void FixedUpdate()
    {
        if (StressState == BossStressState.Accumulate)
        {
            bossBar.SetBarValue(bossBar.BarValue + (StressSpeed * Time.fixedDeltaTime));
        }
    }
}

public enum BossStressState
{
    Freezed,
    Accumulate
}