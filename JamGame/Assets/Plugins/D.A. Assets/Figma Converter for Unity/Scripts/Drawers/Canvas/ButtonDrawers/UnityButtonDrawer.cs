using DA_Assets.FCU.Extensions;
using DA_Assets.FCU.Model;
using DA_Assets.Shared;
using DA_Assets.Shared.Extensions;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;

namespace DA_Assets.FCU.Drawers.CanvasDrawers
{
    public class UnityButtonDrawer : MonoBehaviourBinder<FigmaConverterUnity>
    {
        public void SetupUnityButton(SyncData btnSyncData)
        {
            SyncHelper[] syncHelpers = btnSyncData.GameObject.GetComponentsInChildren<SyncHelper>(true).Skip(1).ToArray();

            Button btn = btnSyncData.GameObject.GetComponent<Button>();

            bool hasCustomButtonBackgrounds = false;

            List<FObject> backgrounds = new List<FObject>();

            foreach (int cindex in btnSyncData.ChildIndexes)
            {
                if (monoBeh.CurrentProject.TryGetByIndex(cindex, out FObject child))
                {
                    if (child.ContainsTag(FcuTag.Image))
                    {
                        backgrounds.Add(child);
                    }

                    if (child.ContainsAnyTag(
                       FcuTag.BtnDefault,
                       FcuTag.BtnDisabled,
                       FcuTag.BtnHover,
                       FcuTag.BtnPressed,
                       FcuTag.BtnSelected))
                    {
                        hasCustomButtonBackgrounds = true;
                        break;
                    }
                }
            }

            if (hasCustomButtonBackgrounds)
            {
                SetCustomTargetGraphics(syncHelpers, btn);
            }
            else
            {
                SetDefaultTargetGraphic(syncHelpers, btn);
            }
        }

        private void SetCustomTargetGraphics(SyncHelper[] syncHelpers, Button btn)
        {
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
            }
        }

        public void SetSprite(Selectable selectable, SyncHelper syncHelper)
        {
            selectable.transition = Selectable.Transition.SpriteSwap;
            SpriteState spriteState = selectable.spriteState;

            if (syncHelper.ContainsTag(FcuTag.BtnDefault))
            {
                if (syncHelper.TryGetComponent(out Graphic graphic))
                {
                    selectable.targetGraphic = graphic;
                }
            }
            else
            {
                if (syncHelper.TryGetComponent(out Image img))
                {
                    if (syncHelper.ContainsTag(FcuTag.BtnPressed))
                    {
                        spriteState.pressedSprite = img.sprite;
                        syncHelper.gameObject.Destroy();
                    }
                    else if (syncHelper.ContainsTag(FcuTag.BtnHover))
                    {
                        spriteState.highlightedSprite = img.sprite;
                        syncHelper.gameObject.Destroy();
                    }
                    else if (syncHelper.ContainsTag(FcuTag.BtnSelected))
                    {
                        spriteState.selectedSprite = img.sprite;
                        syncHelper.gameObject.Destroy();
                    }
                    else if (syncHelper.ContainsTag(FcuTag.BtnDisabled))
                    {
                        spriteState.disabledSprite = img.sprite;
                        syncHelper.gameObject.Destroy();
                    }
                }
            }

            selectable.spriteState = spriteState;
        }

        public void SetImageColor(Selectable selectable, SyncHelper syncHelper)
        {
            selectable.transition = Selectable.Transition.ColorTint;
            ColorBlock colorBlock = selectable.colors;

            if (syncHelper.TryGetComponent(out Graphic graphic))
            {
                if (syncHelper.ContainsTag(FcuTag.BtnDefault))
                {
                    selectable.targetGraphic = graphic;
                }
                else
                {
                    if (syncHelper.ContainsTag(FcuTag.BtnPressed))
                    {
                        colorBlock.pressedColor = graphic.color;
                        syncHelper.gameObject.Destroy();
                    }
                    else if (syncHelper.ContainsTag(FcuTag.BtnHover))
                    {
                        colorBlock.highlightedColor = graphic.color;
                        syncHelper.gameObject.Destroy();
                    }
                    else if (syncHelper.ContainsTag(FcuTag.BtnSelected))
                    {
                        colorBlock.selectedColor = graphic.color;
                        syncHelper.gameObject.Destroy();
                    }
                    else if (syncHelper.ContainsTag(FcuTag.BtnDisabled))
                    {
                        colorBlock.disabledColor = graphic.color;
                        syncHelper.gameObject.Destroy();
                    }
                }
            }

            selectable.colors = colorBlock;
        }

        public void SetDefaultTargetGraphic(SyncHelper[] syncHelpers, Button btn)
        {
            Graphic gr1 = null;
            bool exists = !syncHelpers.IsEmpty() && syncHelpers.First().TryGetComponent(out gr1);

            //If the first element of the hierarchy can be used as a target graphic.
            if (exists)
            {
                btn.targetGraphic = gr1;
            }
            else
            {
                //If there is at least some image, assign it to the targetGraphic.
                foreach (SyncHelper meta in syncHelpers)
                {
                    if (meta.TryGetComponent(out Image gr2))
                    {
                        btn.targetGraphic = gr2;
                        return;
                    }
                }

                //If there is at least some graphic, assign it to the targetGraphic.
                foreach (SyncHelper meta in syncHelpers)
                {
                    if (meta.TryGetComponent(out Graphic gr3))
                    {
                        btn.targetGraphic = gr3;
                        return;
                    }
                }

                //If there is a graphic on the button itself, assign it to the targetGraphic.
                if (btn.TryGetComponent(out Graphic gr4))
                {
                    btn.targetGraphic = gr4;
                }
            }
        }
    }
}
