using Common;
using Pickle;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Employee.Personality
{
    [CreateAssetMenu(fileName = "QuirkConfig", menuName = "Employee/QuirkConfig")]
    public class QuirkConfig : ScriptableObject, IUidHandle
    {
        [Pickle(LookupType = ObjectProviderType.Assets)]
        public Quirk QuirkModelAsset;

        [SerializeField]
        private string fullName;
        public string FullName => fullName;

        [Required]
        [PreviewField]
        [SerializeField]
        private Sprite icon;
        public Sprite Icon => icon;

        public InternalUid Uid => QuirkModelAsset.Uid;
    }
}
