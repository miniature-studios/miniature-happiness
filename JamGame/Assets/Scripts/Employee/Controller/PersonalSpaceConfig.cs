using UnityEngine;

[CreateAssetMenu(
    fileName = "PersonalSpaceConfig",
    menuName = "Employee/PersonalSpaceConfig",
    order = 2
)]
public class PersonalSpaceConfig : ScriptableObject
{
    public float ActorRadius;
    public float MovelessVelocityThreshold;
}
