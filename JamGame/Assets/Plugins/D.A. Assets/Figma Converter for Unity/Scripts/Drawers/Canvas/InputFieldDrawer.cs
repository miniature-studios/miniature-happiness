using DA_Assets.FCU.Model;
using DA_Assets.Shared;
using DA_Assets.Shared.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;

#if TextMeshPro
using TMPro;
#endif

namespace DA_Assets.FCU.Drawers.CanvasDrawers
{
    [Serializable]
    public class InputFieldDrawer : MonoBehaviourBinder<FigmaConverterUnity>
    {
        private List<SyncData> inputFields;

        public void Init()
        {
            inputFields = new List<SyncData>();
        }

        public void Draw(FObject fobject)
        {
            switch (monoBeh.Settings.ComponentSettings.TextComponent)
            {
                case TextComponent.UnityText:
                    fobject.Data.GameObject.TryAddComponent(out InputField inputField);
                    break;
#if TextMeshPro
                case TextComponent.TextMeshPro:
                    fobject.Data.GameObject.TryAddComponent(out TMP_InputField tmpInputField);
                    break;
#endif
            }

            inputFields.Add(fobject.Data);
        }

        public IEnumerator SetTargetGraphics()
        {
            switch (monoBeh.Settings.ComponentSettings.TextComponent)
            {
                case TextComponent.UnityText:
                    yield return SetTargetGraphicsInputFields();
                    break;
                case TextComponent.TextMeshPro:
                    yield return SetTargetGraphicsTmpInputFields();
                    break;
            }

            inputFields.Clear();
        }

        private IEnumerator SetTargetGraphicsInputFields()
        {
            foreach (SyncData fieldMeta in inputFields)
            {
                InputField inputField = fieldMeta.GameObject.GetComponent<InputField>();

                InputFieldModel ifm = GetGraphics(fieldMeta);

                inputField.targetGraphic = ifm.Background;
                inputField.placeholder = ifm.Placeholder;

                inputField.textComponent = (Text)ifm.TextComponent;
                inputField.textComponent.supportRichText = false;

                inputField.enabled = false;
                yield return WaitFor.Delay001();
                inputField.enabled = true;
            }
        }
        private IEnumerator SetTargetGraphicsTmpInputFields()
        {
#if TextMeshPro
            foreach (SyncData fieldMeta in inputFields)
            {
                TMP_InputField inputField = fieldMeta.GameObject.GetComponent<TMP_InputField>();

                InputFieldModel ifm = GetGraphics(fieldMeta);

                inputField.targetGraphic = ifm.Background;
                inputField.placeholder = ifm.Placeholder;
                inputField.textComponent = (TMP_Text)ifm.TextComponent;

                inputField.enabled = false;
                yield return WaitFor.Delay001();
                inputField.enabled = true;
            }
#endif
            yield return null;
        }
        private InputFieldModel GetGraphics(SyncData fieldMeta)
        {
            SyncHelper[] childMetas = fieldMeta.GameObject.GetComponentsInChildren<SyncHelper>(true).Skip(1).ToArray();

            InputFieldModel ifm = new InputFieldModel
            {
                TextComponent = null,
                Background = null,
                Placeholder = null
            };

            foreach (SyncHelper meta in childMetas)
            {
                bool exists = meta.TryGetComponent(out Graphic graphic);

                if (exists == false)
                {
                    continue;
                }

                if (ifm.Placeholder == null)
                {
                    if (meta.Data.Tags.Contains(FcuTag.Placeholder))
                    {
                        ifm.Placeholder = graphic;
                        break;
                    }
                }
            }

            foreach (SyncHelper meta in childMetas)
            {
                bool exists = meta.TryGetComponent(out Graphic graphic);

                if (exists == false)
                {
                    continue;
                }

                if (ifm.TextComponent == null)
                {
                    if (meta.Data.Tags.Contains(FcuTag.Text) && graphic != ifm.Placeholder)
                    {
                        ifm.TextComponent = graphic;
                        break;
                    }
                }
            }

            foreach (SyncHelper meta in childMetas)
            {
                bool exists = meta.TryGetComponent(out Graphic graphic);

                if (exists == false)
                {
                    continue;
                }

                if (ifm.Background == null && graphic != ifm.Placeholder && graphic != ifm.TextComponent)
                {
                    if (meta.Data.Tags.Contains(FcuTag.Image))
                    {
                        ifm.Background = graphic;
                        break;
                    }
                }
            }

            if (ifm.Background == null)
            {
                bool exists = fieldMeta.GameObject.TryGetComponent(out Graphic graphic);

                if (exists)
                {
                    ifm.Background = graphic;
                }
            }

            return ifm;
        }
    }
    public struct InputFieldModel
    {
        public Graphic Background;
        public Graphic TextComponent;
        public Graphic Placeholder;
    }
}
