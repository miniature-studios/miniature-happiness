using System.Collections.Generic;
using System.Collections.Immutable;
using Common;
using Employee.Needs;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Employee
{
    [CreateAssetMenu(fileName = "Quirk", menuName = "Employee/Quirk", order = 3)]
    public class Quirk : ScriptableObject
    {
        [SerializeField]
        [InlineProperty]
        private InternalUid uid;
        public InternalUid Uid => uid;

        [SerializeField]
        private List<Need.NeedProperties> additionalNeeds;
        public ImmutableList<Need.NeedProperties> AdditionalNeeds =>
            additionalNeeds.ToImmutableList();

        [SerializeReference]
        private List<IEffect> effects = new();
        public IEnumerable<IEffect> Effects => effects;
    }
}
