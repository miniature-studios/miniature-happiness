using DA_Assets.Shared;

#pragma warning disable CS0162

namespace DA_Assets.FCU
{
    internal class ImportEventsTab : ScriptableObjectBinder<FcuSettingsWindow, FigmaConverterUnity>
    {
        internal void Draw()
        {
            gui.SectionHeader(FcuLocKey.label_import_events.Localize());
            gui.Space15();

#if UNITY_2020_1_OR_NEWER == false
            gui.Label12px(FcuLocKey.label_supported_from_unity_version.Localize("2020.1.0"), widthType: WidthType.Expand);
            return;
#endif

            gui.SerializedPropertyField<FigmaConverterUnity>(scriptableObject.SerializedObject, x => x.Events.OnImportStart);
            gui.SerializedPropertyField<FigmaConverterUnity>(scriptableObject.SerializedObject, x => x.Events.OnImportComplete);
            gui.SerializedPropertyField<FigmaConverterUnity>(scriptableObject.SerializedObject, x => x.Events.OnImportFail);
            gui.SerializedPropertyField<FigmaConverterUnity>(scriptableObject.SerializedObject, x => x.Events.OnObjectInstantiate);
            gui.SerializedPropertyField<FigmaConverterUnity>(scriptableObject.SerializedObject, x => x.Events.OnAddComponent);
        }
    }
}
