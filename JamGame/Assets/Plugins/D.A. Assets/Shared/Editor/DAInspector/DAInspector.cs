using DA_Assets.Shared.Extensions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditor.AnimatedValues;
using UnityEngine;
using Debug = UnityEngine.Debug;

#pragma warning disable CS0649

namespace DA_Assets.Shared
{
    [CreateAssetMenu(menuName = DAConstants.Publisher + "/" + DAConstants.DAInspector)]
    public class DAInspector : SingletoneScriptableObject<DAInspector>
    {
        [SerializeField] GUIStyle[] guiStyles;
        [SerializeField] DAIResources resources;
        public DAIResources Resources => resources;

        private Dictionary<string, GroupData> groupDatas = new Dictionary<string, GroupData>();
        private Dictionary<string, HamburgerItem> hamItems = new Dictionary<string, HamburgerItem>();

        private const int hamburgerMenuItemsLimit = 200;

        public void DrawSplitGroup(Group group, Action body1, Action body2)
        {
            StackFrame sf = new StackFrame(1, true);
            string methodPath = GetMethodPath(sf);
            string unicumId = $"{methodPath}-{group.InstanceId}";

            if (groupDatas.TryGetValue(unicumId, out GroupData gd) == false)
            {
                gd = new GroupData();
                gd.SplitterPosition = group.SplitterStartPos;
                groupDatas.Add(unicumId, gd);
            }

            group.Body = () =>
            {
                DrawGroup(new Group
                {
                    Options = new GUILayoutOption[]
                    {
                        GUILayout.Width(gd.SplitterPosition),
                        GUILayout.MaxWidth(gd.SplitterPosition),
                        GUILayout.MinWidth(gd.SplitterPosition)
                    },
                    Body = () =>
                    {
                        body1?.Invoke();
                    }
                });

                GUILayout.Box("",
                    GUILayout.Width(group.SplitterWidth),
                    GUILayout.MaxWidth(group.SplitterWidth),
                    GUILayout.MinWidth(group.SplitterWidth),
                    GUILayout.ExpandHeight(true));
                gd.SplitterRect = GUILayoutUtility.GetLastRect();

                if (group.GroupType == GroupType.Horizontal)
                {
                    EditorGUIUtility.AddCursorRect(gd.SplitterRect, MouseCursor.ResizeHorizontal);
                }
                else if (group.GroupType == GroupType.Vertical)
                {
                    EditorGUIUtility.AddCursorRect(gd.SplitterRect, MouseCursor.ResizeVertical);
                }

                DrawGroup(new Group
                {
                    Options = new GUILayoutOption[]
                    {
                        GUILayout.ExpandWidth(true)
                    },
                    Body = () =>
                    {
                        body2?.Invoke();
                    }
                });
            };

            DrawGroup(group);

            if (Event.current != null)
            {
                switch (Event.current.rawType)
                {
                    case EventType.MouseDown:
                        if (gd.SplitterRect.Contains(Event.current.mousePosition))
                        {
                            gd.IsDragging = true;
                        }
                        break;
                    case EventType.MouseDrag:
                        if (gd.IsDragging)
                        {
                            gd.SplitterPosition += Event.current.delta.x;
                        }
                        break;
                    case EventType.MouseUp:
                        if (gd.IsDragging)
                        {
                            gd.IsDragging = false;
                        }
                        break;
                }
            }
        }

        public void DrawGroup(Group group)
        {
            if (group.LabelWidth != null)
            {
                EditorGUIUtility.labelWidth = (float)group.LabelWidth;
            }

            StackFrame sf = new StackFrame(1, true);

            if (EditorGUIUtility.isProSkin)
            {
                if (group.DarkBg.ToBoolNullFalse())
                {
                    GUI.backgroundColor = Color.gray;
                }
            }
            else
            {
                GUI.backgroundColor = Color.white;
                EditorStyles.label.normal.textColor = Color.white;
            }

            if (group.GroupType == GroupType.Horizontal)
            {
                if (group.Style != GuiStyle.None)
                {
                    GUILayout.BeginHorizontal(GetStyle(group.Style), group.Options);
                }
                else
                {
                    GUILayout.BeginHorizontal(group.Options);
                }

                if (group.Flexible)
                    FlexibleSpace();

                group.Body.Invoke();

                if (group.Flexible)
                    FlexibleSpace();

                GUILayout.EndHorizontal();
            }
            else if (group.GroupType == GroupType.Vertical)
            {
                if (group.Style != GuiStyle.None)
                {
                    GUILayout.BeginVertical(GetStyle(group.Style), group.Options);
                }
                else
                {
                    GUILayout.BeginVertical(group.Options);
                }

                if (group.Scroll)
                {
                    BeginScroll(group, sf);
                }

                if (group.Flexible)
                    FlexibleSpace();

                group.Body.Invoke();

                if (group.Flexible)
                    FlexibleSpace();

                if (group.Scroll)
                {
                    EndScroll();
                }

                GUILayout.EndVertical();
            }
            else if (group.GroupType == GroupType.Fade)
            {
                if (EditorGUILayout.BeginFadeGroup(group.Fade.faded))
                {
                    if (group.Flexible)
                        FlexibleSpace();

                    group.Body.Invoke();

                    if (group.Flexible)
                        FlexibleSpace();
                }

                EditorGUILayout.EndFadeGroup();
            }
            else
            {
                DALogger.Log($"Unknown group type.");
            }

            if (EditorGUIUtility.isProSkin)
            {
                if (group.DarkBg.ToBoolNullFalse())
                {
                    GUI.backgroundColor = Color.white;
                }
            }
            else
            {
                GUI.backgroundColor = Color.white;
                EditorStyles.label.normal.textColor = Color.black;
            }
        }

        public bool HamburgerButton(Rect position, GUIContent content)
        {
            bool _value = GUI.Button(position, content, GetStyle(GuiStyle.HamburgerButton));
            return _value;
        }

        public bool HamburgerToggle(Rect position, bool value)
        {
            bool _value = EditorGUI.Toggle(position, value, EditorStyles.toggle);
            return _value;
        }

        public bool CheckBox(GUIContent label, bool value, bool rightSide = true, Action onClick = null)
        {
            bool _value = false;

            DrawGroup(new Group
            {
                Style = GuiStyle.CheckBoxField,
                GroupType = GroupType.Horizontal,
                DarkBg = true,
                Body = () =>
                {
                    if (rightSide)
                    {
                        Btn();
                    }

                    Rect rect = GUILayoutUtility.GetRect(width: 25, height: 25);
                    _value = EditorGUI.Toggle(
                        rect,
                        value,
                        EditorStyles.toggle);

                    if (!rightSide)
                    {
                        Btn();
                    }
                }
            });

            void Btn()
            {
                if (GUILayout.Button(label, GetStyle(GuiStyle.CheckBoxLabel)))
                {
                    if (onClick == null)
                    {
                        value = !value;
                    }
                    else
                    {
                        onClick.Invoke();
                    }
                }

                GUILayout.FlexibleSpace();
            }

            return _value;
        }

        public void DrawTable(Action[,] actions)
        {
            int rows = actions.GetLength(0);
            int columns = actions.GetLength(1);

            DrawGroup(new Group
            {
                GroupType = GroupType.Horizontal,
                Body = () =>
                {
                    for (int i = 0; i < columns; i++)
                    {
                        DrawGroup(new Group
                        {
                            GroupType = GroupType.Vertical,
                            Body = () =>
                            {
                                for (int j = 0; j < rows; j++)
                                {
                                    DrawGroup(new Group
                                    {
                                        GroupType = GroupType.Horizontal,
                                        Body = actions[j, i]
                                    });
                                }
                            }
                        });
                    }
                }
            });
        }

        public int ShaderDropdown(GUIContent label, int option, string[] options, Action<int> onChange)
        {
            DrawGroup(new Group
            {
                GroupType = GroupType.Horizontal,
                Body = () =>
                {
                    Label(label);

                    EditorGUI.BeginChangeCheck();
                    option = EditorGUILayout.Popup(option, options, GetStyle(GuiStyle.TextField));
                    if (EditorGUI.EndChangeCheck())
                    {
                        onChange?.Invoke(option);
                    }
                }
            });

            Rect rect = GUILayoutUtility.GetRect(width: 125, height: 25);
            rect.y -= 22;
            rect.x += 156;

            Space6();
            return option;
        }

        public int Dropdown(GUIContent label, int option, string[] options, Action<int> onChange)
        {
            DrawGroup(new Group
            {
                GroupType = GroupType.Horizontal,
                Body = () =>
                {
                    Label(label);

                    EditorGUI.BeginChangeCheck();
                    option = EditorGUILayout.Popup(option, options, GetStyle(GuiStyle.TextField));
                    if (EditorGUI.EndChangeCheck())
                    {
                        onChange?.Invoke(option);
                    }
                }
            });

            Space6();
            return option;
        }

        public bool Toggle(GUIContent label, bool value, GUIContent btnLabel = null, Action buttonClick = null)
        {
            int option = value ? 1 : 0;

            DrawGroup(new Group
            {
                GroupType = GroupType.Horizontal,
                Body = () =>
                {
                    Label(label);

                    option = EditorGUILayout.Popup(option, new string[]
                    {
                        "DISABLED",
                        "ENABLED"
                    }, GetStyle(GuiStyle.TextField));

                    if (buttonClick != null)
                    {
                        Space10();

                        if (GUILayout.Button(btnLabel, GetStyle(GuiStyle.HabmurgerTextSubButton)))
                        {
                            buttonClick.Invoke();
                        }
                    }
                }
            });

            Space6();

            bool _value = option == 1;

            return _value;
        }

        public string BigTextField(string value, string label = null, string tooltip = null, bool password = false)
        {
            string _value = "";

            DrawGroup(new Group
            {
                GroupType = GroupType.Horizontal,
                Body = () =>
                {
                    if (label != null)
                    {
                        GUILayout.Label(new GUIContent(label, tooltip), GetStyle(GuiStyle.BigFieldLabel12px), GUILayout.Width(EditorGUIUtility.labelWidth));
                    }

                    if (password)
                    {
                        _value = EditorGUILayout.PasswordField(value, GetStyle(GuiStyle.BigTextField), GUILayout.ExpandWidth(true));
                    }
                    else
                    {
                        _value = EditorGUILayout.TextField(value, GetStyle(GuiStyle.BigTextField), GUILayout.ExpandWidth(true));
                    }
                }
            });

            return _value;
        }

        public string TextField(GUIContent label, string currentValue, GUIContent btnLabel = null, Action buttonClick = null, bool password = false)
        {
            DrawGroup(new Group
            {
                GroupType = GroupType.Horizontal,
                Body = () =>
                {
                    Label(label);

                    if (password)
                    {
                        currentValue = EditorGUILayout.PasswordField(currentValue, GetStyle(GuiStyle.TextField));
                    }
                    else
                    {
                        currentValue = EditorGUILayout.TextField(currentValue, GetStyle(GuiStyle.TextField));
                    }

                    if (buttonClick != null)
                    {
                        Space6();

                        GUIStyle style;

                        if (btnLabel.image == null)
                        {
                            style = GetStyle(GuiStyle.HabmurgerTextSubButton);
                        }
                        else
                        {
                            style = GetStyle(GuiStyle.HabmurgerImageSubButton);
                        }

                        if (GUILayout.Button(btnLabel, style))
                        {
                            buttonClick.Invoke();
                        }
                    }
                }
            });

            Space6();

            return currentValue;
        }

        public float SliderField(GUIContent label, float value, float minValue, float maxValue)
        {
            float _value = 0;

            DrawGroup(new Group
            {
                GroupType = GroupType.Horizontal,
                DarkBg = true,
                Body = () =>
                {
                    Label(label);

                    _value = EditorGUILayout.Slider(value, minValue, maxValue);
                }
            });

            Space6();

            return _value;
        }

        public float FloatField(GUIContent label, float value)
        {
            float _value = 0;

            DrawGroup(new Group
            {
                GroupType = GroupType.Horizontal,
                DarkBg = true,
                Body = () =>
                {
                    Label(label);
                    _value = EditorGUILayout.FloatField(value, GetStyle(GuiStyle.TextField));
                }
            });

            Space6();

            return _value;
        }

        public int IntField(GUIContent label, int value)
        {
            int _value = 0;

            DrawGroup(new Group
            {
                GroupType = GroupType.Horizontal,
                DarkBg = true,
                Body = () =>
                {
                    Label(label);
                    _value = EditorGUILayout.IntField(value, GetStyle(GuiStyle.TextField));
                }
            });

            Space6();

            return _value;
        }

        public Vector2Int Vector2IntField(GUIContent label, Vector2Int value)
        {
            Vector2Int _value = default;

            DrawGroup(new Group
            {
                GroupType = GroupType.Horizontal,
                DarkBg = true,
                Body = () =>
                {
                    Label(label);
                    _value = EditorGUILayout.Vector2IntField("", value);
                }
            });

            Space6();

            return _value;
        }

        public Vector4 Vector4Field(GUIContent label, Vector4 value)
        {
            Vector4 _value = default;

            DrawGroup(new Group
            {
                GroupType = GroupType.Horizontal,
                DarkBg = true,
                Body = () =>
                {
                    Label(label);
                    _value = EditorGUILayout.Vector4Field("", value);
                }
            });

            Space6();

            return _value;
        }

        public void Label(GUIContent label, WidthType labelWidthType = WidthType.Default, GuiStyle customStyle = GuiStyle.Label12px)
        {
            GUIStyle style = GetStyle(customStyle);

            switch (labelWidthType)
            {
                case WidthType.Default:
                    GUILayout.Label(label, style, GUILayout.Width(EditorGUIUtility.labelWidth));
                    break;
                case WidthType.Option:
                    GUILayout.Label(label, style);
                    break;
                case WidthType.Expand:
                    GUILayout.Label(label, style, GUILayout.ExpandWidth(true));
                    break;
            }
        }

        public GUIStyle GetStyle(GuiStyle customStyle)
        {
            foreach (GUIStyle style in guiStyles)
            {
                if (style.name == $"{customStyle}")
                {
                    return style;
                }
            }

            Debug.LogError($"'{customStyle}' style not found.");
            return guiStyles.FirstOrDefault(x => x.name == GuiStyle.None.ToString());
        }



        public void Space10() => GUILayout.Space(10);
        public void Space6() => GUILayout.Space(6);

        public int LayerField(GUIContent label, int layer)
        {
            int result = 0;

            DrawGroup(new Group
            {
                GroupType = GroupType.Horizontal,
                Body = () =>
                {
                    if (label != null)
                    {
                        Label(label);
                    }

                    Rect r = EditorGUILayout.GetControlRect(false, 20);
                    result = EditorGUI.LayerField(r, layer, GetStyle(GuiStyle.TextField));
                }
            });

            Space6();

            return result;
        }

        public void Line(bool horizontal = true, int lineHeight = 1)
        {
            if (horizontal)
            {
                Rect rect = EditorGUILayout.GetControlRect(false, lineHeight);
                rect.height = lineHeight;
                rect.x -= 3;

                Color lineColor = Color.gray;
                lineColor.a = 0.25f;

                EditorGUI.DrawRect(rect, lineColor);
            }
            else
            {
                Rect rect = EditorGUILayout.GetControlRect(false, lineHeight);
                rect.width = 1;
                rect.height = lineHeight;
                rect.x += rect.width / 2;

                Color lineColor = Color.gray;
                lineColor.a = 0.25f;

                EditorGUI.DrawRect(rect, lineColor);
            }

        }

        public void TopProgressBar(float value)
        {
            Rect position = GUILayoutUtility.GetRect(0, 7, GUILayout.ExpandWidth(true));

            GUIStyle pbarBG = GetStyle(GuiStyle.ProgressBarBg);
            GUIStyle pbarBody = GetStyle(GuiStyle.ProgressBar);

            int controlId = GUIUtility.GetControlID(nameof(TopProgressBar).GetHashCode(), FocusType.Keyboard);

            if (Event.current.GetTypeForControl(controlId) == EventType.Repaint)
            {
                if (value > 0.0f)
                {
                    Rect barRect = new Rect(position);
                    barRect.width *= value;
                    pbarBody.Draw(barRect, false, false, false, false);
                }
            }
        }

        public bool LinkLabel(GUIContent label, WidthType widthType, GuiStyle customStyle)
        {
            bool clicked = false;

            DrawGroup(new Group
            {
                GroupType = GroupType.Horizontal,
                Body = () =>
                {
                    GUIStyle style = GetStyle(customStyle);

                    Rect btnRect = default(Rect);

                    switch (widthType)
                    {
                        case WidthType.Default:
                            btnRect = GUILayoutUtility.GetRect(label, style, GUILayout.Width(EditorGUIUtility.labelWidth));
                            break;
                        case WidthType.Option:
                            btnRect = GUILayoutUtility.GetRect(label, style);
                            break;
                        case WidthType.Expand:
                            btnRect = GUILayoutUtility.GetRect(label, style, GUILayout.ExpandWidth(true));
                            break;
                    }

                    clicked = GUI.Button(btnRect, label, style);
                }
            });

            return clicked;
        }

        public bool SquareButton30x30(GUIContent label)
        {
            bool clicked = false;

            DrawGroup(new Group
            {
                GroupType = GroupType.Horizontal,
                Body = () =>
                {
                    GUIStyle style = GetStyle(GuiStyle.SquareButton30x30);
                    Rect btnRect = GUILayoutUtility.GetRect(label, style, GUILayout.ExpandWidth(true));
                    clicked = GUI.Button(btnRect, label, style);
                }
            });

            return clicked;
        }

        public bool TabButton(GUIContent label)
        {
            bool clicked = false;

            DrawGroup(new Group
            {
                GroupType = GroupType.Horizontal,
                Body = () =>
                {
                    GUIStyle style = GetStyle(GuiStyle.TabButton);
                    Rect btnRect = GUILayoutUtility.GetRect(label, style, GUILayout.ExpandWidth(true));
                    clicked = GUI.Button(btnRect, label, style);
                }
            });

            return clicked;
        }

        public bool Button(GUIContent label, WidthType widthType, GuiStyle customStyle)
        {
            bool clicked = false;

            DrawGroup(new Group
            {
                GroupType = GroupType.Horizontal,
                Body = () =>
                {
                    GUIStyle style = GetStyle(customStyle);

                    Rect btnRect;

                    if (widthType == WidthType.Expand)
                    {
                        btnRect = GUILayoutUtility.GetRect(label, style, GUILayout.ExpandWidth(true));
                    }
                    else
                    {
                        btnRect = GUILayoutUtility.GetRect(label, style);
                    }

                    if (style.fixedWidth > 0)
                    {

                    }
                    else if (btnRect.width > 300 && widthType != WidthType.Expand)
                    {
                        btnRect.width /= 2;
                        btnRect.x += btnRect.width / 2;
                    }

                    clicked = GUI.Button(btnRect, label, style);
                }
            });

            return clicked;
        }

        public void BeginScroll(Group group, StackFrame sf)
        {
            string methodPath = GetMethodPath(sf);
            string unicumId = $"{methodPath}-{group.InstanceId}";

            if (groupDatas.TryGetValue(unicumId, out GroupData gd) == false)
            {
                gd = new GroupData();
                groupDatas.Add(unicumId, gd);
            }

            gd.ScrollPosition = EditorGUILayout.BeginScrollView(gd.ScrollPosition, false, false);
        }

        public void EndScroll()
        {
            EditorGUILayout.EndScrollView();
        }

        public string GetMethodPath(StackFrame frame)
        {
            var method = frame.GetMethod();
            string className = method.DeclaringType.Name;
            int lineNumber = frame.GetFileLineNumber();
            return $"{className}-{lineNumber}";
        }

        private List<HamburgerItem> _internalHambBuffer;
        private int _bufferCount = 0;

        public void DrawMenu(HamburgerItem menu)
        {
            if (_internalHambBuffer == null)
            {
                _internalHambBuffer = new List<HamburgerItem>();
            }

            DrawMenu(_internalHambBuffer, menu);
        }

        public void DrawMenu(List<HamburgerItem> buffer, HamburgerItem menu)
        {
            if (buffer.Count > hamburgerMenuItemsLimit)
            {
                Debug.Log($"_bufferCount: {buffer.Count}, limit: {hamburgerMenuItemsLimit}");
                buffer = buffer.GetRange(hamburgerMenuItemsLimit / 2, hamburgerMenuItemsLimit - 1);
                Debug.Log($"_bufferCount after GetRange: {buffer.Count}");
            }

            int index = buffer.FindIndex(x => x.Id == menu.Id);

            if (index < 0)
            {
                buffer.Add(menu);
                index = 0;
            }

            GUILayout.BeginHorizontal(GetStyle(GuiStyle.HambugerButtonBg));

            if (buffer[index].Fade == null)
            {
                buffer[index].Fade = new AnimBool(false);
                buffer[index].Fade.speed = 4f;
            }

            if (menu.Body != null)
            {
                Texture2D t2d;

                if (buffer[index].Fade.value)
                {
                    t2d = resources.ImgExpandOpened;
                }
                else
                {
                    t2d = resources.ImgExpandClosed;
                }

                GUILayout.Button(t2d, GetStyle(GuiStyle.HamburgerExpandButton));
            }

            Rect btnRect = GUILayoutUtility.GetRect(menu.GUIContent, GetStyle(GuiStyle.HamburgerButton), GUILayout.ExpandWidth(true));
            btnRect.x += 15;
            btnRect.width -= 46;

            if (HamburgerButton(btnRect, menu.GUIContent))
            {
                buffer[index].Fade.target = !buffer[index].Fade.target;
            }

            Rect smallBtnRect = default;

            if (menu.OnButtonClick != null)
            {
                smallBtnRect = GUILayoutUtility.GetRect(new GUIContent(), GUI.skin.box);
                smallBtnRect.width = 20;
                smallBtnRect.height = 20;
                smallBtnRect.x -= 15;

                GUIStyle style;

                if (menu.ButtonGuiContent.image == null)
                    style = GetStyle(GuiStyle.HabmurgerTextSubButton);
                else
                    style = GetStyle(GuiStyle.HabmurgerImageSubButton);

                if (GUI.Button(smallBtnRect, menu.ButtonGuiContent, style))
                    menu.OnButtonClick.Invoke();
            }

            if (menu.CheckBoxValueChanged != null)
            {
                Rect cbRect = btnRect;
                cbRect.x += btnRect.width;
                cbRect.width = 20;

                if (smallBtnRect == default)
                    cbRect.x += 10;
                else
                    cbRect.x -= 1;

                GUI.backgroundColor = (Color)Color.gray;

                buffer[index].CheckBoxValue.Value = HamburgerToggle(
                    cbRect,
                    buffer[index].CheckBoxValue.Value);

                GUI.backgroundColor = (Color)Color.white;

                if (buffer[index].CheckBoxValue.Value != buffer[index].CheckBoxValue.Temp)
                {
                    buffer[index].CheckBoxValue.Temp = buffer[index].CheckBoxValue.Value;
                    menu.CheckBoxValueChanged.Invoke(menu.Id, buffer[index].CheckBoxValue.Value);
                }
            }

            GUILayout.EndHorizontal();

            if (menu.Body != null)
            {
                DrawGroup(new Group
                {
                    GroupType = GroupType.Horizontal,
                    Body = () =>
                    {
                        Space15();

                        DrawGroup(new Group
                        {
                            GroupType = GroupType.Vertical,
                            Body = () =>
                            {
                                DrawGroup(new Group
                                {
                                    GroupType = GroupType.Fade,
                                    Fade = buffer[index].Fade,
                                    Body = () =>
                                    {
                                        Space6();
                                        menu.Body.Invoke();
                                    }
                                });
                            }
                        });

                        Space15();
                    }
                });
            }
        }

        public T EnumField<T>(GUIContent label, T @enum, bool uppercase = true, string[] itemNames = null, Action onChange = null)
        {
            List<int> enumValues = Enum.GetValues(@enum.GetType()).Cast<int>().ToList();

            if (itemNames == null)
            {
                itemNames = Enum.GetNames(@enum.GetType());
            }

            if (uppercase)
            {
                for (int i = 0; i < itemNames.Length; i++)
                {
                    itemNames[i] = Regex.Replace(itemNames[i], "(\\B[A-Z])", "$1").ToUpper();
                }
            }
            else
            {
                for (int i = 0; i < itemNames.Length; i++)
                {
                    itemNames[i] = itemNames[i].Replace("_", " ");
                }
            }

            int result = 0;

            DrawGroup(new Group
            {
                GroupType = GroupType.Horizontal,
                Body = () =>
                {
                    if (label != null)
                    {
                        Label(label);
                    }

                    int _result2 = EditorGUILayout.Popup(enumValues.IndexOf(Convert.ToInt32(@enum)), itemNames, GetStyle(GuiStyle.TextField));
                    result = enumValues[_result2];
                }
            });

            Space6();

            T _result = (T)(object)result;

            if (_result.Equals(@enum) == false)
            {
                onChange?.Invoke();
            }

            return _result;
        }

        public void DragZoneInt(Rect dragZone, ref int dragValue)
        {
            Event currentEvent = Event.current;
            int controlID = GUIUtility.GetControlID(FocusType.Passive);
            switch (currentEvent.type)
            {
                case EventType.Repaint:
                    // Change the cursor to the horizontal resize cursor when it's over the drag zone
                    if (dragZone.Contains(currentEvent.mousePosition))
                    {
                        EditorGUIUtility.AddCursorRect(dragZone, MouseCursor.ResizeHorizontal);
                    }
                    break;

                case EventType.MouseDown:
                    // Check if the mouse is within the drag zone
                    if (dragZone.Contains(currentEvent.mousePosition))
                    {
                        // Start the drag operation
                        GUIUtility.hotControl = controlID;
                        currentEvent.Use();
                    }
                    break;

                case EventType.MouseDrag:
                    // Check if we are dragging the hot control
                    if (GUIUtility.hotControl == controlID)
                    {
                        // Perform drag operation here, for example, adjust the dragValue
                        dragValue += (int)currentEvent.delta.x;
                        currentEvent.Use();
                    }
                    break;

                case EventType.MouseUp:
                    // End the drag operation
                    if (GUIUtility.hotControl == controlID)
                    {
                        GUIUtility.hotControl = 0;
                        currentEvent.Use();
                    }
                    break;
            }
        }

        public string DrawSelectPathField(string selectedPath, GUIContent label, GUIContent btnLabel, string folderPanelText)
        {
            TextField(
               label,
               selectedPath,
               btnLabel,
               () =>
               {
                   string _selectedPath = EditorUtility.OpenFolderPanel(folderPanelText, "", "");

                   if (string.IsNullOrWhiteSpace(_selectedPath) == false)
                   {
                       if (IsPathInsideAssetsPath(_selectedPath))
                       {
                           selectedPath = _selectedPath;
                       }
                       else
                       {
                           //Console.LogError(FcuLocKey.label_inside_assets_folder.Localize());
                       }
                   }
               });

            return ToRelativePath(selectedPath);
        }

        private bool IsPathInsideAssetsPath(string path)
        {
            if (path.IndexOf(Application.dataPath, System.StringComparison.InvariantCultureIgnoreCase) == -1)
            {
                return false;
            }

            return true;
        }

        private string ToRelativePath(string absolutePath)
        {
            if (absolutePath.StartsWith(Application.dataPath))
            {
                return "Assets" + absolutePath.Substring(Application.dataPath.Length);
            }

            return absolutePath;
        }


        public void Label10px(string label, string tooltip = null, WidthType widthType = WidthType.Default) =>
            Label(new GUIContent(label, tooltip), widthType, GuiStyle.Label10px);

        public void Label12px(string label, string tooltip = null, WidthType widthType = WidthType.Default) =>
            Label(new GUIContent(label, tooltip), widthType, GuiStyle.Label12px);

        public void RedLinkLabel10px(string label, string tooltip = null, WidthType widthType = WidthType.Default) =>
            LinkLabel(new GUIContent(label, tooltip), widthType, GuiStyle.RedLabel10px);

        public void BlueLinkLabel10px(string label, string tooltip = null, WidthType widthType = WidthType.Default) =>
            LinkLabel(new GUIContent(label, tooltip), widthType, GuiStyle.BlueLabel10px);

        public bool SectionHeader(string label, string tooltip = null) =>
            Button(new GUIContent(label, tooltip), WidthType.Expand, GuiStyle.SectionHeader);

        public bool OutlineButton(string label, string tooltip = null, WidthType expand = WidthType.Default) =>
            Button(new GUIContent(label, tooltip), expand, GuiStyle.OutlineButton);

        public bool LinkButton(string label, string tooltip = null, WidthType expand = WidthType.Default) =>
            Button(new GUIContent(label, tooltip), expand, GuiStyle.LinkButton);



        public int SPACE_10 => 10;
        public int SPACE_5 => 5;
        public int SPACE_6 => 6;
        public int SPACE_15 => 15;
        public int SPACE_30 => 30;
        public int SPACE_60 => 60;

        public void Space60() => GUILayout.Space(SPACE_60);
        public void Space30() => GUILayout.Space(SPACE_30);
        public void Space15() => GUILayout.Space(SPACE_15);

        public void Space5() => GUILayout.Space(SPACE_5);
        public void FlexibleSpace() => GUILayout.FlexibleSpace();
        public void Space(float pixels) => GUILayout.Space(pixels);



        private SerializedProperty GetPropertyRecursive(string[] names, int index, SerializedProperty property)
        {
            if (index >= names.Length)
            {
                return property;
            }
            else
            {
                string fieldName = names[index];
                //Debug.Log($"{fieldName} | {property}");
                SerializedProperty rprop = property.FindPropertyRelative(fieldName);
                return GetPropertyRecursive(names, index + 1, rprop);
            }
        }
        public IEnumerable<SerializedProperty> GetChildren(SerializedProperty property)
        {
            property = property.Copy();
            var nextElement = property.Copy();
            bool hasNextElement = nextElement.NextVisible(false);
            if (!hasNextElement)
            {
                nextElement = null;
            }

            property.NextVisible(true);
            while (true)
            {
                if ((SerializedProperty.EqualContents(property, nextElement)))
                {
                    yield break;
                }

                yield return property;

                bool hasNext = property.NextVisible(false);
                if (!hasNext)
                {
                    break;
                }
            }
        }
        public void DrawChildProperty<T>(SerializedObject so, SerializedProperty parentElement, Expression<Func<T, object>> pathExpression)
        {
            SerializedProperty targetGraphic = GetChildProperty(parentElement, pathExpression);

            if (targetGraphic == null)
            {
                return;
            }

            DrawProperty(so, targetGraphic);
        }
        public SerializedProperty GetPropertyFromArray<T>(SerializedProperty arrayProperty, int elementIndex)
        {
            if (arrayProperty?.arraySize > 0 && arrayProperty.arraySize >= elementIndex + 1)
            {
                return arrayProperty.GetArrayElementAtIndex(elementIndex);
            }

            return null;
        }
        public SerializedProperty GetChildProperty<T>(SerializedProperty arrayProperty, Expression<Func<T, object>> pathExpression)
        {
            try
            {
                string[] fields = pathExpression.GetFieldsArray();
                return arrayProperty.FindPropertyRelative(fields[0]);
            }
            catch
            {
                return null;
            }
        }
        public void DrawProperty(SerializedObject so, SerializedProperty property, bool darkTheme = true)
        {
            DrawGroup(new Group
            {
                GroupType = GroupType.Horizontal,
                DarkBg = darkTheme,
                Body = () =>
                {
                    Space15();

                    DrawGroup(new Group
                    {
                        GroupType = GroupType.Vertical,
                        DarkBg = darkTheme,
                        Body = () =>
                        {
                            so.Update();

                            try
                            {
                                EditorGUILayout.PropertyField(property, true);
                            }
                            catch (Exception)
                            {

                            }

                            so.ApplyModifiedProperties();
                        }
                    });
                }
            });
        }

        public void SerializedPropertyField<T>(SerializedObject so, Expression<Func<T, object>> pathExpression, bool darkTheme = true, bool? isExpanded = null)
        {
            string[] fields = pathExpression.GetFieldsArray();

            DrawGroup(new Group
            {
                GroupType = GroupType.Horizontal,
                Body = () =>
                {
                    Space(SPACE_15 - 1);

                    DrawGroup(new Group
                    {
                        GroupType = GroupType.Vertical,
                        DarkBg = darkTheme,
                        Body = () =>
                        {
                            SerializedProperty rootProperty = so.FindProperty(fields[0]);
                            SerializedProperty lastProperty = GetPropertyRecursive(fields, 1, rootProperty);

                            if (isExpanded != null)
                            {
                                lastProperty.isExpanded = (bool)isExpanded;
                            }

                            so.Update();
                            EditorGUILayout.PropertyField(lastProperty, true);
                            so.ApplyModifiedProperties();
                        }
                    });
                }
            });
        }

        internal void DrawMenu(object selectableHamburgerItems, HamburgerItem hamburgerItem)
        {
            throw new NotImplementedException();
        }
    }
}