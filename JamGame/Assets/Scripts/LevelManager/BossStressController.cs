using UnityEngine;

public class BossStressController : MonoBehaviour
{
    [SerializeField] private BossBar bossBar;
    [HideInInspector] public float StressSpeed = 0;
    public bool Active { get; set; } = false;
    private void FixedUpdate()
    {
        if (Active)
        {
            bossBar.SetBarValue(bossBar.BarValue + (StressSpeed * Time.fixedDeltaTime));
        }
    }
}

