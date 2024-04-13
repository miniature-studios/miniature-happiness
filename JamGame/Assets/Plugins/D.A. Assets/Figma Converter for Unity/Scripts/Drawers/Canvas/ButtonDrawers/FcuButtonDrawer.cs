using DA_Assets.FCU.Extensions;
using DA_Assets.FCU.Model;
using DA_Assets.FCU.UI;
using DA_Assets.Shared;
using DA_Assets.Shared.Extensions;
using System.Linq;
using TMPro;
using UnityEngine.UI;

namespace DA_Assets.FCU.Drawers.CanvasDrawers
{
    public class FcuButtonDrawer : MonoBehaviourBinder<FigmaConverterUnity>
    {
        public void SetupFcuButton(SyncData btnSyncData)
        {
            FcuButton btn = btnSyncData.GameObject.GetComponent<FcuButton>();

            SyncHelper[] syncHelpers = btnSyncData.GameObject.GetComponentsInChildren<SyncHelper>(true).Skip(1).ToArray();

            foreach (SyncHelper syncHelper in syncHelpers)
            {
                if (syncHelper.ContainsTag(FcuTag.Image))
                {
                    if (syncHelper.Data.FcuImageType == FcuImageType.Downloadable || syncHelper.Data.FcuImageType == FcuImageType.Generative)
                    {
                        monoBeh.CanvasDrawer.ButtonDrawer.UnityButtonDrawer.SetSprite(btn, syncHelper);
                    }
                    else
                    {
                        monoBeh.CanvasDrawer.ButtonDrawer.UnityButtonDrawer.SetImageColor(btn, syncHelper);
                    }
                }
                else if (syncHelper.ContainsTag(FcuTag.Text))
                {
                    SetText(btn, syncHelper);
                }
            }
        }

        private void SetText(FcuButton btn, SyncHelper syncHelper)
        {
            if (syncHelper.TryGetComponent(out Graphic graphic))
            {
                if (syncHelper.ContainsAnyTag(
                   FcuTag.BtnDefault,
                   FcuTag.BtnDisabled,
                   FcuTag.BtnHover,
                   FcuTag.BtnPressed,
                   FcuTag.BtnSelected))
                {
                    btn.ChangeTextColor = true;
                }

                if (syncHelper.ContainsTag(FcuTag.BtnDefault))
                {
                    btn.TextDefaultColor = graphic.color;

                    switch (monoBeh.Settings.ComponentSettings.TextComponent)
                    {
                        case TextComponent.UnityText:
                            btn.ButtonText = syncHelper.GetComponent<Text>();
                            break;
                        case TextComponent.TextMeshPro:
#if TextMeshPro
                            btn.ButtonText = syncHelper.GetComponent<TMP_Text>();
#endif
                            break;
                    }
                }
                else if (syncHelper.ContainsTag(FcuTag.BtnPressed))
                {
                    btn.TextPressedColor = graphic.color;
                    syncHelper.gameObject.Destroy();
                }
                else if (syncHelper.ContainsTag(FcuTag.BtnHover))
                {
                    btn.TextHoverColor = graphic.color;
                    syncHelper.gameObject.Destroy();
                }
                else if (syncHelper.ContainsTag(FcuTag.BtnSelected))
                {
                    btn.TextSelectedColor = graphic.color;
                    syncHelper.gameObject.Destroy();
                }
                else if (syncHelper.ContainsTag(FcuTag.BtnDisabled))
                {
                    btn.TextDisabledColor = graphic.color;
                    syncHelper.gameObject.Destroy();
                }
            }
        }
    }
}