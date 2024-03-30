using DA_Assets.Shared;

namespace DA_Assets.FCU
{
    internal class AssetTab : ScriptableObjectBinder<FcuSettingsWindow, FigmaConverterUnity>
    {
        public void Draw()
        {
            if (scriptableObject.Inspector.Header.MonoBeh == null)
            {
                scriptableObject.Close();
            }
            else
            {
                scriptableObject.Inspector.DrawGUI(GuiStyle.None);
            }
        }
    }
}

