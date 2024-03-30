using DA_Assets.FCU.Extensions;
using DA_Assets.FCU.Model;
using DA_Assets.Shared;
using DA_Assets.Shared.Extensions;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace DA_Assets.FCU.Drawers.CanvasDrawers
{
    [Serializable]
    public class GridLayoutDrawer : MonoBehaviourBinder<FigmaConverterUnity>
    {
        public void Draw(FObject fobject)
        {
            if (fobject.Data.GameObject.TryGetComponent(out LayoutGroup oldLayoutGroup))
            {
                oldLayoutGroup.Destroy();
            }

            fobject.Data.GameObject.TryAddComponent(out FlexibleGridLayoutGroup layoutGroup);

            layoutGroup.LayoutAxis = LayoutAxis.Horizontal;

            layoutGroup.childAlignment = fobject.GetHorLayoutAnchor();
            layoutGroup.padding = fobject.GetPadding();
            layoutGroup.ConstraintCount = fobject.GetColumnCount();

            layoutGroup.ChildControlWidth = false;
            layoutGroup.ChildControlHeight = false;

            float spacingX;
            float spacingY;

            if (fobject.PrimaryAxisAlignItems == PrimaryAxisAlignItem.SPACE_BETWEEN)
            {
                layoutGroup.ChildForceExpandWidth = true;
                spacingX = 0;
            }
            else
            {
                layoutGroup.ChildForceExpandWidth = false;
                spacingX = fobject.ItemSpacing.ToFloat();
            }

            if (fobject.CounterAxisAlignContent == "SPACE_BETWEEN")
            {
                layoutGroup.ChildForceExpandHeight = true;
                spacingY = 0;
            }
            else
            {
                layoutGroup.ChildForceExpandHeight = false;
                spacingY = fobject.CounterAxisSpacing.ToFloat();
            }

            layoutGroup.Spacing = new Vector2(spacingX, spacingY);
        }
    }
}