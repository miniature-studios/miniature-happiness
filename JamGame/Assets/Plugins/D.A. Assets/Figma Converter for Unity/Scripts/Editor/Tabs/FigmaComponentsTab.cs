using DA_Assets.Shared;

#pragma warning disable IDE0003

namespace DA_Assets.FCU
{
    internal class FigmaComponentsTab : ScriptableObjectBinder<FcuSettingsWindow, FigmaConverterUnity>
    {
        public void Draw()
        {
            gui.SectionHeader(FcuLocKey.label_figma_comp.Localize(), FcuLocKey.tooltip_figma_comp.Localize());
            gui.Space15();

            gui.Label12px(FcuLocKey.label_figma_comp_desc.Localize(), widthType: WidthType.Expand);

            gui.Space15();

            gui.DrawGroup(new Group
            {
                GroupType = GroupType.Vertical,
                Body = () =>
                {
                    for (int i = 0; i < monoBeh.Settings.MainSettings.ComponentsUrls.Length; i++)
                    {
                        monoBeh.Settings.MainSettings.ComponentsUrls[i] = gui.BigTextField(monoBeh.Settings.MainSettings.ComponentsUrls[i]);

                        gui.Space(18);
                    }
                }
            });
        }
    }
}