using System.Collections.Generic;
using System.Collections.Immutable;
using Common;
using Employee.Needs;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Employee.Personality
{
    [CreateAssetMenu(fileName = "Quirk", menuName = "Employee/Quirk")]
    public class Quirk : ScriptableObject, IPostprocessedUidHandle, IUidHandle
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

        [SerializeField]
        private string fullName;
        public string FullName => fullName;

        [TextArea]
        [SerializeField]
        private string description;
        public string Description => description;

        [Required]
        [PreviewField]
        [SerializeField]
        private Sprite icon;
        public Sprite Icon => icon;
    }
}
