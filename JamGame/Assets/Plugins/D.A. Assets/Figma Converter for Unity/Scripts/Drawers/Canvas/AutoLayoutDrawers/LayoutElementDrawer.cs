using DA_Assets.FCU.Model;
using DA_Assets.Shared;
using DA_Assets.Shared.Extensions;
using System;
using UnityEngine.UI;

namespace DA_Assets.FCU.Drawers.CanvasDrawers
{
    [Serializable]
    public class LayoutElementDrawer : MonoBehaviourBinder<FigmaConverterUnity>
    {
        public void Draw(FObject fobject)
        {
            fobject.Data.GameObject.TryAddComponent(out LayoutElement layoutElement);

            layoutElement.minWidth = fobject.Size.x;
            layoutElement.minHeight = fobject.Size.y;

            if (fobject.LayoutPositioning == "ABSOLUTE")
            {
                layoutElement.ignoreLayout = true;
            }
            else
            {
                layoutElement.ignoreLayout = false;
            }
        }
    }
}
