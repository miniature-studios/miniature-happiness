using DA_Assets.FCU.Model;
using DA_Assets.Shared;
using DA_Assets.Shared.Extensions;
using System;
using UnityEngine;
using UnityEngine.UI;

#pragma warning disable CS0649

namespace DA_Assets.FCU.Drawers.CanvasDrawers
{
    [Serializable]
    public class AutoLayoutDrawer : MonoBehaviourBinder<FigmaConverterUnity>
    {
        public void Draw(FObject fobject)
        {
            foreach (int index in fobject.Data.ChildIndexes)
            {
                if (monoBeh.CurrentProject.TryGetByIndex(index, out FObject child))
                {
                    this.LayoutElementDrawer.Draw(child);
                }
            }

            if (fobject.Data.GameObject.TryGetComponent(out LayoutGroup oldLayoutGroup))
            {
                oldLayoutGroup.Destroy();
            }

            if (fobject.LayoutWrap == LayoutWrap.WRAP)
            {
                this.GridLayoutDrawer.Draw(fobject);
            }
            else if (fobject.LayoutMode == LayoutMode.HORIZONTAL)
            {
                this.HorLayoutDrawer.Draw(fobject);
            }
            else if (fobject.LayoutMode == LayoutMode.VERTICAL)
            {
                this.VertLayoutDrawer.Draw(fobject);
            }
        }



        [SerializeField] GridLayoutDrawer gridLayoutDrawer;
        [SerializeProperty(nameof(gridLayoutDrawer))]
        public GridLayoutDrawer GridLayoutDrawer => gridLayoutDrawer.SetMonoBehaviour(monoBeh);

        [SerializeField] VertLayoutDrawer vertLayoutDrawer;
        [SerializeProperty(nameof(vertLayoutDrawer))]
        public VertLayoutDrawer VertLayoutDrawer => vertLayoutDrawer.SetMonoBehaviour(monoBeh);

        [SerializeField] HorLayoutDrawer horLayoutDrawer;
        [SerializeProperty(nameof(horLayoutDrawer))]
        public HorLayoutDrawer HorLayoutDrawer => horLayoutDrawer.SetMonoBehaviour(monoBeh);

        [SerializeField] LayoutElementDrawer layoutElementDrawer;
        [SerializeProperty(nameof(layoutElementDrawer))]
        public LayoutElementDrawer LayoutElementDrawer => layoutElementDrawer.SetMonoBehaviour(monoBeh);
    }
}
