using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

namespace Employee
{
    [CreateAssetMenu(fileName = "Quirk", menuName = "Employee/Quirk", order = 3)]
    public class Quirk : ScriptableObject
    {
        [SerializeField]
        private List<Need.NeedProperties> additionalNeeds;
        public ReadOnlyCollection<Need.NeedProperties> AdditionalNeeds =>
            additionalNeeds.AsReadOnly();

        // TODO: Move to QuirkView
        // TODO: Change to Image
        [SerializeField]
        private string name_;
        public string Name => name_;
    }
}
