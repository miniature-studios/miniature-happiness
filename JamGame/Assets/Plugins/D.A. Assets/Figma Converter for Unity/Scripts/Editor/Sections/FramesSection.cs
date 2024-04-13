using DA_Assets.FCU.Drawers;
using DA_Assets.Shared;
using DA_Assets.Shared.Extensions;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DA_Assets.FCU
{
    internal class FramesSection : ScriptableObjectBinder<FcuEditor, FigmaConverterUnity>
    {
        private Dictionary<string, InfinityScrollRectWindow<SelectableFObject>> _scrolls = new Dictionary<string, InfinityScrollRectWindow<SelectableFObject>>();

        public override void Init()
        {
            base.Init();

            foreach (SelectableFObject item in monoBeh.InspectorDrawer.SelectableDocument.Childs)
            {
                var isrw = new InfinityScrollRectWindow<SelectableFObject>(10, 35);
                _scrolls.Add(item.FObject.Id, isrw);
            }
        }

        private void DrawFrame(SelectableFObject item)
        {
            item.Selected = gui.CheckBox(new GUIContent(item.FObject.Name), item.Selected, rightSide: false);
        }

        public void Draw()
        {
            SelectableFObject doc = monoBeh.InspectorDrawer.SelectableDocument;

            DrawMenuWithChildren(doc);

            void DrawMenuWithChildren(SelectableFObject item)
            {
                int selectedCount = item.Childs.SelectRecursive(x => x.Childs).Count(x => x.Childs.IsEmpty() && x.Selected);
                int allCount = item.Childs.SelectRecursive(x => x.Childs).Count(x => x.Childs.IsEmpty());
                bool isAllSelected = selectedCount == allCount;
                SetCheckboxValue(item.FObject.Id, isAllSelected);

                if (item.FObject.Type == Model.NodeType.CANVAS)
                {
                    gui.DrawMenu(monoBeh.InspectorDrawer.SelectableHamburgerItems, new HamburgerItem
                    {
                        Id = item.FObject.Id,
                        GUIContent = new GUIContent($"{item.FObject.Name} ({selectedCount}/{allCount})", ""),
                        Body = () =>
                        {
                            foreach (SelectableFObject item1 in monoBeh.InspectorDrawer.SelectableDocument.Childs)
                            {
                                if (_scrolls.TryGetValue(item1.FObject.Id, out var scroll1))
                                {
                                    scroll1.SetData(item1.Childs, DrawFrame);
                                }
                            }

                            if (_scrolls.TryGetValue(item.FObject.Id, out var scroll2))
                            {
                                scroll2.OnGUI();
                            }
                        },
                        CheckBoxValueChanged = (id, value) => SetAllChildrenSelected(item, value)
                    });
                }
                else
                {
                    GUIContent gc;

                    if (item.FObject.Type == Model.NodeType.DOCUMENT)
                    {
                        gc = new GUIContent(FcuLocKey.label_frames_to_import.Localize(selectedCount, allCount), "");
                    }
                    else
                    {
                        gc = new GUIContent($"{item.FObject.Name} ({selectedCount}/{allCount})", "");
                    }

                    gui.DrawMenu(monoBeh.InspectorDrawer.SelectableHamburgerItems, new HamburgerItem
                    {
                        Id = item.FObject.Id,
                        GUIContent = gc,
                        Body = () =>
                        {
                            foreach (var child in item.Childs)
                            {
                                DrawMenuWithChildren(child);
                            }
                        },
                        CheckBoxValueChanged = (id, value) => SetAllChildrenSelected(item, value)
                    });
                }
            }

            void SetCheckboxValue(string id, bool value)
            {
                var checkBoxValue = monoBeh.InspectorDrawer.SelectableHamburgerItems.FirstOrDefault(item => item.Id == id)?.CheckBoxValue;
                if (checkBoxValue != null)
                {
                    checkBoxValue.Value = value;
                    checkBoxValue.Temp = value;
                }
            }

            void SetAllChildrenSelected(SelectableFObject item, bool selected)
            {
                item.Selected = selected;
                foreach (var child in item.Childs)
                {
                    SetAllChildrenSelected(child, selected);
                }
            }
        }
    }
}