using Common;
using UnityEngine;
using UnityEngine.Events;

public class LevelTemperaryData : MonoBehaviour
{
    [InspectorReadOnly] private Check check;
    public UnityEvent<IReadonlyData<Check>> CheckChanged;
    public void CreateCheck(Check new_check)
    {
        check = new_check;
        CheckChanged?.Invoke(check);
    }
}

