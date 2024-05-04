using System;
using Sirenix.OdinInspector;
using UnityEngine;

[AddComponentMenu("Scripts/PostprocessingSelector")]
internal class PostprocessingSelector : MonoBehaviour
{
    public enum Mode
    {
        Normal,
        StressOverlay
    }

    [SerializeField]
    [ReadOnly]
    private Mode mode = Mode.Normal;

    [Serializable]
    private struct NormalSettings
    {
        public GameObject Effects;
    }

    [Serializable]
    private struct StressOverlaySettings
    {
        public GameObject Effects;
        public PostEffectMask PostEffectMask;
    }

    [SerializeField]
    private NormalSettings normalModeSettings;

    [SerializeField]
    private StressOverlaySettings stressOverlayModeSettings;

    private void Awake()
    {
        stressOverlayModeSettings.Effects.SetActive(false);

        stressOverlayModeSettings.PostEffectMask.enabled = false;
        normalModeSettings.Effects.SetActive(true);
    }

    [Button]
    public void SetMode(Mode mode)
    {
        switch (mode)
        {
            case Mode.Normal:
                stressOverlayModeSettings.Effects.SetActive(false);
                stressOverlayModeSettings.PostEffectMask.enabled = false;

                normalModeSettings.Effects.SetActive(true);
                break;
            case Mode.StressOverlay:
                normalModeSettings.Effects.SetActive(false);

                stressOverlayModeSettings.Effects.SetActive(true);
                stressOverlayModeSettings.PostEffectMask.enabled = true;
                break;
            default:
                throw new ArgumentException();
        }
    }
}
