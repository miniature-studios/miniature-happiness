using System.Collections.Generic;
using System.Collections.Immutable;
using Employee.Needs;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Employee
{
    [CreateAssetMenu(fileName = "Quirk", menuName = "Employee/Quirk", order = 3)]
    public class Quirk : ScriptableObject
    {
        [ReadOnly]
        [SerializeField]
        // TODO: Wrap it in newtype.
        private string uid;
        public string Uid => uid;

#if UNITY_EDITOR
        public void SetHashCode(string uid)
        {
            this.uid = uid;
        }
#endif

        [SerializeField]
        private List<Need.NeedProperties> additionalNeeds;
        public ImmutableList<Need.NeedProperties> AdditionalNeeds =>
            additionalNeeds.ToImmutableList();
    }
}
