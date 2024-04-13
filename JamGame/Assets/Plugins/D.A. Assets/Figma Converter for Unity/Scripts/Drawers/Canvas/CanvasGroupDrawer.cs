using DA_Assets.FCU.Model;
using DA_Assets.Shared;
using DA_Assets.Shared.Extensions;
using System;
using UnityEngine;

namespace DA_Assets.FCU.Drawers.CanvasDrawers
{
    [Serializable]
    public class CanvasGroupDrawer : MonoBehaviourBinder<FigmaConverterUnity>
    {
        public void Draw(FObject fobject)
        {
            if (fobject.Data.FcuImageType == FcuImageType.Downloadable)
                return;

            fobject.Data.GameObject.TryAddComponent(out CanvasGroup canvasGroup);
            canvasGroup.alpha = (float)fobject.Opacity;
        }
    }
}