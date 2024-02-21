using UnityEngine;
using UnityEngine.SceneManagement;

namespace Utils
{
    [AddComponentMenu("Scripts/Utils/Utils.ReloadScene")]
    internal class ReloadScene : MonoBehaviour
    {
        public void Reload()
        {
            _ = Resources.UnloadUnusedAssets();
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex, LoadSceneMode.Single);
        }
    }
}
