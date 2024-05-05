using System;
using UnityEditor.AnimatedValues;
using UnityEngine;

namespace DA_Assets.Shared
{
    public struct Group
    {
        public int InstanceId { get; set; }
        public GroupType GroupType { get; set; }
        public Action Body { get; set; }
        public GuiStyle Style { get; set; }
        public bool Flexible { get; set; }
        public AnimBool Fade { get; set; }
        public GUILayoutOption[] Options { get; set; }
        public bool Scroll { get; set; }
        public int? LabelWidth { get; set; }
        public int SplitterWidth { get; set; }
        public int SplitterStartPos { get; set; }
        public bool? DarkBg { get; set; }
    }

    public class GroupData
    {
        public Vector2 ScrollPosition { get; set; } = Vector2.zero;
        public float SplitterPosition { get; set; }
        public Rect SplitterRect { get; set; }
        public bool IsDragging { get; set; }
    }
}