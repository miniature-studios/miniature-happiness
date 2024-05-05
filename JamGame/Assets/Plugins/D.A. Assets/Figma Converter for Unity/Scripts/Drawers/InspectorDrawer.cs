using DA_Assets.FCU.Model;
using DA_Assets.Shared;
using DA_Assets.Shared.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DA_Assets.FCU.Drawers
{
    [Serializable]

    public class InspectorDrawer : MonoBehaviourBinder<FigmaConverterUnity>
    {
        private NodeType[] _allowedInspectorNodes = new NodeType[]
        {
            NodeType.CANVAS,
            NodeType.FRAME
        };

        public void FillSelectableFramesArray()
        {
            _selectableHambItems.Clear();

            SelectableFObject doc = new SelectableFObject();
            FObject docFObject = monoBeh.CurrentProject.FigmaProject.Document;

            FillNewSelectableItemRecursively(doc, docFObject);

            bool same = CompareIdsRecursively(_doc, doc);

            if (!same)
            {
                _doc = doc;
                _doc.SetAllSelected(true);
            }
        }

        private void FillNewSelectableItemRecursively(SelectableFObject parentItem, FObject parent)
        {
            parentItem.Name = parent.Name;
            parentItem.FObject = new SerializableFObject
            {
                Id = parent.Id,
                Type = parent.Type,
                Name = parent.Name
            };


            if (parent.Children != null)
            {
                foreach (FObject child in parent.Children)
                {
                    bool isAllowed = _allowedInspectorNodes.Contains(child.Type);
                    if (!isAllowed)
                        continue;

                    SelectableFObject childItem = new SelectableFObject();
                    FillNewSelectableItemRecursively(childItem, child);
                    parentItem.Childs.Add(childItem);
                }
            }
        }

        private bool CompareIdsRecursively(SelectableFObject item1, SelectableFObject item2)
        {
            if (item1.FObject.Id != item2.FObject.Id)
                return false;

            if (item1.Childs.Count != item2.Childs.Count)
                return false;

            for (int i = 0; i < item1.Childs.Count; i++)
            {
                if (!CompareIdsRecursively(item1.Childs[i], item2.Childs[i]))
                    return false;
            }

            return true;
        }

        [SerializeField] List<HamburgerItem> _selectableHambItems = new List<HamburgerItem>();
        public List<HamburgerItem> SelectableHamburgerItems { get => _selectableHambItems; set => SetValue(ref _selectableHambItems, value); }

        [SerializeField] SelectableFObject _doc = new SelectableFObject();
        public SelectableFObject SelectableDocument { get => _doc; set => SetValue(ref _doc, value); }
    }

    [Serializable]
    public class SelectableFObject
    {
        [SerializeField] string name;
        public string Name { get => name; set => name = value; }

        [SerializeField] bool selected;
        public bool Selected { get => selected; set => selected = value; }

        [SerializeField] List<SelectableFObject> childs = new List<SelectableFObject>();
        public List<SelectableFObject> Childs { get => childs; set => childs = value; }

        public SerializableFObject FObject { get; set; }

        public void SetAllSelected(bool value)
        {
            selected = value;

            foreach (SelectableFObject child in childs)
            {
                child.SetAllSelected(value);
            }
        }
    }

    [Serializable]
    public struct SerializableFObject
    {
        [SerializeField] string id;
        public string Id { get => id; set => id = value; }

        [SerializeField] NodeType type;
        public NodeType Type { get => type; set => type = value; }

        [SerializeField] string name;
        public string Name { get => name; set => name = value; }
    }
}