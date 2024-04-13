using DA_Assets.FCU.Extensions;
using DA_Assets.FCU.Model;
using DA_Assets.Shared;
using DA_Assets.Shared.Extensions;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using System;

namespace DA_Assets.FCU
{
    internal class PreImportWindow : DAInspectorWindow<PreImportWindow, FcuEditor, FigmaConverterUnity>
    {
        private Dictionary<string, InfinityScrollRectWindow<SelectableObject<DiffInfo>>> _importScrolls = new Dictionary<string, InfinityScrollRectWindow<SelectableObject<DiffInfo>>>();
        private Dictionary<string, InfinityScrollRectWindow<SelectableObject<SyncData>>> _removeScrolls = new Dictionary<string, InfinityScrollRectWindow<SelectableObject<SyncData>>>();

        private List<HamburgerItem> _selectableHambItems = new List<HamburgerItem>();

        private UpdateBool _importNew = new UpdateBool(false, false);
        private UpdateBool _importFigmaChanged = new UpdateBool(false, false);
        private UpdateBool _importUnityChanged = new UpdateBool(false, false);
        private UpdateBool _importOther = new UpdateBool(false, false);

        private Vector2Int _importNewCount = Vector2Int.zero;
        private Vector2Int _figmaSideChangedCount = Vector2Int.zero;
        private Vector2Int _unitySideChangedCount = Vector2Int.zero;
        private Vector2Int _importOtherCount = Vector2Int.zero;

        private PreImportInput _diffStruct = new PreImportInput();
        private Action<PreImportOutput> _callback;

        private bool inited = false;

        internal void SetData(PreImportInput diffStruct, Action<PreImportOutput> callback)
        {
            _diffStruct = diffStruct;
            _callback = callback;

            foreach (SelectableObject<DiffInfo> item in diffStruct.ToImport.Childs)
            {
                var isrw = new InfinityScrollRectWindow<SelectableObject<DiffInfo>>(4, 100);
                isrw.SetData(item.Childs, DrawToImportItem);
                _importScrolls.Add($"toimport_{item.Object.Id}", isrw);
            }

            foreach (SelectableObject<SyncData> item in diffStruct.ToRemove.Childs)
            {
                var isrw = new InfinityScrollRectWindow<SelectableObject<SyncData>>(11, 40);
                isrw.SetData(item.Childs, DrawToRemoveItem);
                _removeScrolls.Add(item.Object.Id, isrw);
            }

            UpdateFlags();
            inited = true;
        }

        private void CheckForClose()
        {
            if (_diffStruct.IsDefault())
            {
                Debug.Log($"{this.name} was closed because the scripts were recompiled.");
                this.Close();
            }
        }

        public override void DrawGUI()
        {
            if (!inited)
            {
                return;
            }
            else
            {
                CheckForClose();
            }

            gui.DrawGroup(new Group
            {
                GroupType = GroupType.Vertical,
                Scroll = true,
                Style = GuiStyle.DiffCheckerBackground,
                DarkBg = true,
                Body = () =>
                {
                    gui.DrawSplitGroup(new Group
                    {
                        GroupType = GroupType.Horizontal,
                        SplitterWidth = 5,
                        SplitterStartPos = 500
                    },
                    () =>
                    {
                        LeftPanel();
                    },
                    () =>
                    {
                        RightPanel();
                    });
                }
            });
        }

        private void Footer()
        {
            if (gui.OutlineButton(FcuLocKey.label_apply_and_continue.Localize(), FcuLocKey.tooltip_apply_and_continue.Localize()))
            {
                Apply_OnClick();
            }

            void Apply_OnClick()
            {
                IEnumerable<string> toImportSelected = _diffStruct.ToImport.Childs
                    .SelectRecursive(x => x.Childs)
                    .Where(x => x.Selected)
                    .Select(x => x.Object.Id);

                IEnumerable<SyncData> toRemoveSelected = _diffStruct.ToRemove.Childs
                    .SelectRecursive(x => x.Childs)
                    .Where(x => x.Selected)
                    .Select(x => x.Object);

                PreImportOutput result = new PreImportOutput
                {
                    ToImport = toImportSelected,
                    ToRemove = toRemoveSelected
                };

                _callback.Invoke(result);
                this.Close();
            }
        }

        private void LeftPanel()
        {
            gui.DrawGroup(new Group
            {
                GroupType = GroupType.Vertical,
                Style = GuiStyle.DiffCheckerToImportPanel,
                Body = () =>
                {
                    gui.SectionHeader(FcuLocKey.label_components_to_import.Localize(), FcuLocKey.tooltip_components_to_import.Localize());

                    gui.DrawGroup(new Group
                    {
                        GroupType = GroupType.Horizontal,
                        Body = () =>
                        {
                            DrawToImport();

                            gui.Space5();

                            if (gui.OutlineButton(FcuLocKey.label_open_diff_checker.Localize()))
                            {
                                Application.OpenURL("https://www.diffchecker.com/");
                            }

                            gui.FlexibleSpace();
                        }
                    });

                    gui.Space15();

                    DrawMenuWithChildren(_diffStruct.ToImport);


                    void DrawMenuWithChildren(SelectableObject<DiffInfo> item)
                    {
                        bool isRoot = item.Object.Id == PreImportDataCreator.TO_IMPORT_MENU_ID;

                        int allCount = item.CountItems(x => x.Childs.IsEmpty());
                        int selectedCount = item.CountItems(x => x.Childs.IsEmpty() && x.Selected);

                        bool isAllSelected = selectedCount == allCount;

                        GUIContent gc;
                        string menuId = "toimport_";

                        if (isRoot)
                        {
                            menuId += PreImportDataCreator.TO_IMPORT_MENU_ID;
                            gc = new GUIContent(FcuLocKey.label_components_with_count.Localize(selectedCount, allCount), "");

                            gui.DrawMenu(_selectableHambItems, new HamburgerItem
                            {
                                Id = menuId,
                                GUIContent = gc,
                                Body = () =>
                                {
                                    foreach (var child in item.Childs)
                                    {
                                        DrawMenuWithChildren(child);
                                    }
                                },
                                CheckBoxValueChanged = (id, value) =>
                                {
                                    SetAllChildrenSelected(item, value, false);
                                }
                            });
                        }
                        else
                        {
                            menuId += item.Object.Id;
                            gc = new GUIContent($"{item.Object.Name} ({selectedCount}/{allCount})", "");

                            gui.DrawMenu(_selectableHambItems, new HamburgerItem
                            {
                                Id = menuId,
                                GUIContent = gc,
                                Body = () =>
                                {
                                    _importScrolls[menuId].OnGUI();
                                },
                                CheckBoxValueChanged = (id, value) =>
                                {
                                    SetAllChildrenSelected(item, value, false);
                                }
                            });
                        }

                        SetCheckboxValue(menuId, isAllSelected);
                    }


                }
            });
        }
        private void SetCheckboxValue(string id, bool value)
        {
            UpdateBool checkBoxValue = _selectableHambItems.FirstOrDefault(item => item.Id == id)?.CheckBoxValue;

            if (checkBoxValue != null)
            {
                checkBoxValue.Value = value;
                checkBoxValue.Temp = value;
            }
        }
        private void RightPanel()
        {
            gui.DrawGroup(new Group
            {
                GroupType = GroupType.Vertical,
                Style = GuiStyle.DiffCheckerToRemovePanel,
                Body = () =>
                {
                    gui.SectionHeader(FcuLocKey.label_remove_from_scene.Localize(), FcuLocKey.tooltip_remove_from_scene.Localize());

                    gui.Space15();

                    foreach (var item in _diffStruct.ToRemove.Childs)
                    {
                        int selectedCount = item.Childs.Count(x => x.Selected);
                        int allCount = item.Childs.Count;
                        bool isAllSelected = selectedCount == allCount;

                        string menuId = "toremove_";
                        menuId += item.Object.Id;

                        gui.DrawMenu(_selectableHambItems, new HamburgerItem
                        {
                            Id = menuId,
                            GUIContent = new GUIContent($"{item.Object.NewName} ({selectedCount}/{allCount})"),
                            Body = () => _removeScrolls[item.Object.Id].OnGUI(),
                            CheckBoxValueChanged = (id, value) =>
                            {
                                SetAllChildrenSelected(item, value, true);
                            }
                        });

                        SetCheckboxValue(menuId, isAllSelected);
                    }

                    gui.FlexibleSpace();
                    gui.Line();
                    gui.Space15();

                    Footer();
                }
            });
        }

        private void SetAllChildrenSelected<T>(SelectableObject<T> item, bool selected, bool remove) where T : IHaveId
        {
            SetAllChildrenSelectedRecursive(item, selected);

            UpdateFlags();

            void SetAllChildrenSelectedRecursive(SelectableObject<T> currentItem, bool value)
            {
                currentItem.Selected = value;

                foreach (var child in currentItem.Childs)
                {
                    SetAllChildrenSelectedRecursive(child, value);
                }
            }
        }

        private void DrawToRemoveItem(SelectableObject<SyncData> item)
        {
            bool oldState = item.Selected;
            item.Selected = gui.CheckBox(new GUIContent(GetHierarchyWithoutRootFrame(item.Object.Hierarchy)), item.Selected, false);
            if (item.Selected != oldState)
            {
                UpdateFlags();
            }
        }

        private string GetHierarchyWithoutRootFrame(string hierarchy)
        {
            int index = hierarchy.IndexOf(FcuConfig.HierarchyDelimiter);
            if (index != -1)
            {
                return ".." + hierarchy.Substring(index);
            }
            return hierarchy;
        }

        private void DrawToImportItem(SelectableObject<DiffInfo> item)
        {
            gui.DrawGroup(new Group
            {
                GroupType = GroupType.Horizontal,
                DarkBg = true,
                Body = () =>
                {
                    gui.DrawGroup(new Group
                    {
                        GroupType = GroupType.Vertical,
                        Body = () =>
                        {
                            gui.Space5();

                            string name = item.Object.OldData != null ? item.Object.OldData.Hierarchy : item.Object.NewData.Data.Hierarchy;
                            name = GetHierarchyWithoutRootFrame(name);
                            gui.Label12px(name, widthType: WidthType.Expand);
                            gui.Space5();

                            gui.DrawGroup(new Group
                            {
                                GroupType = GroupType.Horizontal,
                                Body = () =>
                                {
                                    bool oldState = item.Selected;
                                    item.Selected = GUILayout.Toggle(item.Selected, GUIContent.none);
                                    if (item.Selected != oldState)
                                    {
                                        UpdateFlags();
                                    }

                                    gui.DrawGroup(new Group
                                    {
                                        GroupType = GroupType.Vertical,
                                        Body = () =>
                                        {
                                            if (GUILayout.Button(new GUIContent(FcuLocKey.label_copy_old_data.Localize()), GUILayout.Width(120)))
                                            {
                                                GUIUtility.systemCopyBuffer = item.Object.OldData.HashData;
                                            }

                                            gui.Space5();

                                            if (GUILayout.Button(new GUIContent(FcuLocKey.label_copy_new_data.Localize()), GUILayout.Width(120)))
                                            {
                                                GUIUtility.systemCopyBuffer = item.Object.NewData.Data.HashData;
                                            }
                                        }
                                    });

                                    gui.Space15();

                                    gui.DrawGroup(new Group
                                    {
                                        GroupType = GroupType.Vertical,
                                        Body = () =>
                                        {
                                            if (item.Object.HasFigmaDiff)
                                            {
                                                gui.Label12px(FcuLocKey.label_different_component_data.Localize(), widthType: WidthType.Expand);
                                            }
                                            else
                                            {
                                                gui.Label12px("_");
                                            }

                                            gui.Space5();

                                            if (item.Object.Color.Enabled)
                                            {
                                                gui.Label12px(FcuLocKey.label_has_differences.Localize(
                                                    "Color",
                                                    $"{item.Object.Color.Value1}",
                                                    $"{item.Object.Color.Value2}"), widthType: WidthType.Expand);
                                            }
                                            else
                                            {
                                                gui.Label12px("_");
                                            }

                                            gui.Space5();

                                            if (item.Object.Size.Enabled)
                                            {
                                                gui.Label12px(FcuLocKey.label_has_differences.Localize(
                                                    "Size",
                                                    $"{item.Object.Size.Value1}",
                                                    $"{item.Object.Size.Value2}"), widthType: WidthType.Expand);
                                            }
                                            else
                                            {
                                                gui.Label12px("_");
                                            }
                                        }
                                    });

                                    gui.FlexibleSpace();
                                }
                            });

                            gui.Space5();

                            gui.Line();
                        }
                    });
                }
            });
        }

        private void UpdateData(Func<SelectableObject<DiffInfo>, bool> condition, bool value)
        {
            foreach (var item in _diffStruct.ToImport.Childs)
            {
                UpdateDataRecursive(item, condition, value);
            }
        }

        private void UpdateDataRecursive(SelectableObject<DiffInfo> item, Func<SelectableObject<DiffInfo>, bool> condition, bool value)
        {
            if (condition(item))
                item.Selected = value;

            foreach (var child in item.Childs)
            {
                UpdateDataRecursive(child, condition, value);
            }
        }

        private void UpdateFlags()
        {
            Debug.Log($"UpdateFlags");

            int importNewCount = _diffStruct.ToImport.CountItems(x => x.Childs.IsEmpty() && x.Object.IsNew);
            int importNewCountSelected = _diffStruct.ToImport.CountItems(x => x.Childs.IsEmpty() && x.Object.IsNew && x.Selected);
            _importNewCount = new Vector2Int(importNewCountSelected, importNewCount);
            _importNew.Value = importNewCount == importNewCountSelected && importNewCount != 0;
            _importNew.Temp = importNewCount == importNewCountSelected && importNewCount != 0;

            int figmaChangedCount = _diffStruct.ToImport.CountItems(x => x.Childs.IsEmpty() && x.Object.HasFigmaDiff);
            int figmaChangedCountSelected = _diffStruct.ToImport.CountItems(x => x.Childs.IsEmpty() && x.Object.HasFigmaDiff && x.Selected);
            _figmaSideChangedCount = new Vector2Int(figmaChangedCountSelected, figmaChangedCount);
            _importFigmaChanged.Value = figmaChangedCount == figmaChangedCountSelected && figmaChangedCount != 0;
            _importFigmaChanged.Temp = figmaChangedCount == figmaChangedCountSelected && figmaChangedCount != 0;

            int unityChangedCount = _diffStruct.ToImport.CountItems(x => x.Childs.IsEmpty() && x.Object.IsUnitySideChanged());
            int unityChangedCountSelected = _diffStruct.ToImport.CountItems(x => x.Childs.IsEmpty() && x.Object.IsUnitySideChanged() && x.Selected);
            _unitySideChangedCount = new Vector2Int(unityChangedCountSelected, unityChangedCount);
            _importUnityChanged.Value = unityChangedCount == unityChangedCountSelected && unityChangedCount != 0;
            _importUnityChanged.Temp = unityChangedCount == unityChangedCountSelected && unityChangedCount != 0;

            int importSameCount = _diffStruct.ToImport.CountItems(x =>
                x.Object.IsNotChanged() &&
                !x.Object.IsUnitySideChanged());

            int importSameCountSelected = _diffStruct.ToImport.CountItems(x =>
                x.Object.IsNotChanged() &&
                x.Selected);

            _importOtherCount = new Vector2Int(importSameCountSelected, importSameCount);
            _importOther.Value = importSameCount == importSameCountSelected && importSameCount != 0;
            _importOther.Temp = importSameCount == importSameCountSelected && importSameCount != 0;
        }

        private void ImportFigmaSide_OnChanged()
        {
            UpdateData(x => x.Object.HasFigmaDiff, _importFigmaChanged.Value);
            UpdateFlags();
        }

        private void UnitySide_OnChanged()
        {
            UpdateData(x => x.Object.Color.Enabled || x.Object.Size.Enabled, _importUnityChanged.Value);
            UpdateFlags();
        }

        private void ImportNew_OnChanged()
        {
            UpdateData(x => x.Object.IsNew, _importNew.Value);
            UpdateFlags();
        }

        private void ImportOther_OnChanged()
        {
            UpdateData(x => !x.Object.IsNew &&
                !x.Object.HasFigmaDiff &&
                !x.Object.IsUnitySideChanged(), _importOther.Value);

            UpdateFlags();
        }

        private void DrawToImport()
        {
            gui.DrawTable(new Action[4, 3]
            {
                {
                    () =>
                    {
                        gui.Label12px(FcuLocKey.label_new.Localize(), FcuLocKey.tooltip_new_components.Localize());
                    },
                    () =>
                    {
                        GUILayout.Label($"{_importNewCount.x}/{_importNewCount.y}", EditorStyles.boldLabel);
                    },
                    () =>
                    {
                        _importNew.Value = GUILayout.Toggle(_importNew.Value, GUIContent.none);
                        if (_importNew.Value != _importNew.Temp)
                        {
                            _importNew.Temp = _importNew.Value;
                            ImportNew_OnChanged();
                        }
                    }
                },
                {
                    () =>
                    {
                        gui.Label12px(FcuLocKey.label_changed_in_figma.Localize(), FcuLocKey.tooltip_changed_in_figma.Localize());
                    },
                    () =>
                    {
                        GUILayout.Label($"{_figmaSideChangedCount.x}/{_figmaSideChangedCount.y}", EditorStyles.boldLabel);
                    },
                    () =>
                    {
                        _importFigmaChanged.Value = GUILayout.Toggle(_importFigmaChanged.Value, GUIContent.none);
                        if (_importFigmaChanged.Value != _importFigmaChanged.Temp)
                        {
                            _importFigmaChanged.Temp = _importFigmaChanged.Value;
                            ImportFigmaSide_OnChanged();
                        }
                    }
                },
                {
                    () =>
                    {
                        gui.Label12px(FcuLocKey.label_changed_in_unity.Localize(), FcuLocKey.tooltip_changed_in_unity.Localize());
                    },
                    () =>
                    {
                        GUILayout.Label($"{_unitySideChangedCount.x}/{_unitySideChangedCount.y}", EditorStyles.boldLabel);
                    },
                    () =>
                    {
                        _importUnityChanged.Value = GUILayout.Toggle(_importUnityChanged.Value, GUIContent.none);
                        if (_importUnityChanged.Value != _importUnityChanged.Temp)
                        {
                            _importUnityChanged.Temp = _importUnityChanged.Value;
                            UnitySide_OnChanged();
                        }
                    }
                },
                {
                    () =>
                    {
                        gui.Label12px(FcuLocKey.label_without_changes.Localize(), FcuLocKey.tooltip_label_without_changes.Localize());
                    },
                    () =>
                    {
                        GUILayout.Label($"{_importOtherCount.x}/{_importOtherCount.y}", EditorStyles.boldLabel);
                    },
                    () =>
                    {
                        _importOther.Value = GUILayout.Toggle(_importOther.Value, GUIContent.none);
                        if (_importOther.Value != _importOther.Temp)
                        {
                            _importOther.Temp = _importOther.Value;
                            ImportOther_OnChanged();
                        }
                    }
                },
            });
        }
    }

    internal static class DiffCheckerExtensions
    {
        internal static bool IsNotChanged(this DiffInfo diffInfo)
        {
            return !diffInfo.IsNew &&
                !diffInfo.HasFigmaDiff &&
                !diffInfo.IsUnitySideChanged();
        }

        internal static bool IsUnitySideChanged(this DiffInfo diffInfo)
        {
            return diffInfo.Color.Enabled || diffInfo.Size.Enabled;
        }

        internal static int CountItems<T>(this SelectableObject<T> obj, Func<SelectableObject<T>, bool> condition)
        {
            int count = 0;

            if (condition(obj))
            {
                count++;
            }

            foreach (SelectableObject<T> child in obj.Childs)
            {
                count += child.CountItems(condition);
            }

            return count;
        }
    }
}