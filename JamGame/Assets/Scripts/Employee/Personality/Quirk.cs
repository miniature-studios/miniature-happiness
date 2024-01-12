using System.Collections.Generic;
using System.Collections.Immutable;
using Employee.Needs;
using UnityEngine;

namespace Employee
{
    [CreateAssetMenu(fileName = "Quirk", menuName = "Employee/Quirk", order = 3)]
    public class Quirk : ScriptableObject
    {
        [SerializeField]
        private List<Need.NeedProperties> additionalNeeds;
        public ImmutableList<Need.NeedProperties> AdditionalNeeds =>
            additionalNeeds.ToImmutableList();

        // TODO: Move to QuirkView
        // TODO: Change to Image
        [SerializeField]
        private string name_;
        public string Name => name_;
    }
}
