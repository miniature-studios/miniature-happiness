using System.Collections.Generic;
using System.Linq;
using Employee;
using Employee.Needs;
using UnityEngine;

namespace Location
{
    [AddComponentMenu("Scripts/Location/Location.NeedProviderManager")]
    public class NeedProviderManager : MonoBehaviour
    {
        private List<NeedProvider> needProviders;

        public void InitGameMode()
        {
            needProviders = new List<NeedProvider>(
                transform.GetComponentsInChildren<NeedProvider>()
            );
        }

        public IEnumerable<NeedProvider> FindAllAvailableProviders(
            EmployeeImpl employee,
            NeedType need_type
        )
        {
            foreach (NeedProvider provider in needProviders)
            {
                if (provider.NeedType == need_type && provider.IsAvailable(employee))
                {
                    yield return provider;
                }
            }
        }

        public IEnumerable<NeedProvider> FindAllNeedProvidersOfType(NeedType needType)
        {
            return needProviders.Where((np) => np.NeedType == needType);
        }

        // Bound to event.
        public void OnEmployeeFired(EmployeeImpl employee)
        {
            foreach (NeedProvider need_provider in needProviders)
            {
                need_provider.OnEmployeeFired(employee);
            }
        }
    }
}
