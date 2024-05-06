using System;
using Overlay;
using Sirenix.OdinInspector;
using UnityEngine;

[AddComponentMenu("Scripts/PostprocessingSelector")]
internal class PostprocessingSelector : MonoBehaviour, IOverlayRenderer<Stress>
{
    public enum Mode
    {
        Normal,
        StressOverlay
    }

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

    public void ApplyOverlay(Stress overlay)
    {
        SetMode(Mode.StressOverlay);
    }

    public void RevertOverlays()
    {
        SetMode(Mode.Normal);
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
