using DA_Assets.FCU.Extensions;
using DA_Assets.FCU.Model;
using DA_Assets.Shared;
using DA_Assets.Shared.Extensions;
using System;
using UnityEngine.UI;

namespace DA_Assets.FCU.Drawers.CanvasDrawers
{
    [Serializable]
    public class VertLayoutDrawer : MonoBehaviourBinder<FigmaConverterUnity>
    {
        public void Draw(FObject fobject)
        {
            fobject.Data.GameObject.TryAddComponent(out VerticalLayoutGroup layoutGroup);

            layoutGroup.childAlignment = fobject.GetVertLayoutAnchor();
            layoutGroup.padding = fobject.GetPadding();
#if UNITY_2020_1_OR_NEWER
            layoutGroup.reverseArrangement = false;
#endif
            layoutGroup.childScaleWidth = false;
            layoutGroup.childScaleHeight = false;

            layoutGroup.childControlWidth = fobject.IsNeedStretchByX();
            layoutGroup.childControlHeight = fobject.IsNeedStretchByY();

            layoutGroup.spacing = fobject.GetVertSpacing();

            if (layoutGroup.childControlWidth)
            {
                layoutGroup.childForceExpandWidth = true;
            }
            else if (fobject.PrimaryAxisAlignItems == PrimaryAxisAlignItem.SPACE_BETWEEN)
            {
                layoutGroup.childForceExpandWidth = true;
            }
            else
            {
                layoutGroup.childForceExpandWidth = false;
            }

            if (layoutGroup.childControlHeight)
            {
                layoutGroup.childForceExpandHeight = true;
            }
            else if (fobject.CounterAxisAlignItems == CounterAxisAlignItem.SPACE_BETWEEN)
            {
                layoutGroup.childForceExpandHeight = true;
            }
            else
            {
                layoutGroup.childForceExpandHeight = false;
            }
        }
    }
}
