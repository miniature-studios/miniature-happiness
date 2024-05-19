using System.Collections.Generic;
using System.Collections.Immutable;
using Common;
using Employee.Needs;
using UnityEngine;

namespace Employee.Personality
{
    [CreateAssetMenu(fileName = "Quirk", menuName = "Employee/Quirk")]
    public class Quirk : ScriptableObject, IUidPostprocessingHandle
    {
        [SerializeField]
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
