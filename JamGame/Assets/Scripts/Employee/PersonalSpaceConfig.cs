using UnityEngine;

[CreateAssetMenu(fileName = "PersonalSpaceConfig", menuName = "ScriptableObjects/PersonalSpaceConfig", order = 2)]
public class PersonalSpaceConfig : ScriptableObject
{
    public float ActorRadius;
    public float MovelessVelocityThreshold;
}
