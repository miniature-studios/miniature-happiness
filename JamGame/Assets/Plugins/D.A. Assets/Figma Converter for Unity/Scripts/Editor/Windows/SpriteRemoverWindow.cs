using DA_Assets.Shared;
using DA_Assets.Shared.Extensions;
using System.Collections;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace DA_Assets.FCU
{
    internal class SpriteRemoverWindow : EditorWindow
    {
        [SerializeField] string spritesPath = "Assets\\Sprites";
        private static Vector2 windowSize = new Vector2(500, 150);
        private DAInspector gui => DAInspector.Instance;

        [MenuItem("Tools/" + FcuConfig.ProductName + "/Remove unused sprites")]
        public static void ShowWindow()
        {
            SpriteRemoverWindow win = GetWindow<SpriteRemoverWindow>("Remove unused sprites");
            win.maxSize = windowSize;
            win.minSize = windowSize;

            win.position = new Rect(
                (Screen.currentResolution.width - windowSize.x * 2) / 2,
                (Screen.currentResolution.height - windowSize.y * 2) / 2,
                windowSize.x,
                windowSize.y);
        }

        private void OnGUI()
        {
            gui.DrawGroup(new Group
            {
                GroupType = GroupType.Vertical,
                Style = GuiStyle.TabBg2,
                Body = () =>
                {
                    gui.Label12px(FcuLocKey.label_remove_unused_sprites.Localize());

                    gui.Space15();

                    spritesPath = gui.DrawSelectPathField(
                        spritesPath,
                        new GUIContent(FcuLocKey.label_sprites_path.Localize(), FcuLocKey.tooltip_sprites_path.Localize()),
                        new GUIContent(FcuLocKey.label_change.Localize()),
                        FcuLocKey.label_select_folder.Localize());

                    gui.Space15();

                    if (gui.OutlineButton(FcuLocKey.label_remove.Localize()))
                    {
                        RemoveCurrentSceneUnusedSprites().StartDARoutine(null);
                    }
                }
            });
        }

        public IEnumerator RemoveCurrentSceneUnusedSprites()
        {
#if UNITY_EDITOR
            Image[] images;

#if UNITY_2023_3_OR_NEWER
            images = MonoBehaviour.FindObjectsByType<Image>(FindObjectsSortMode.None);
#else
            images = MonoBehaviour.FindObjectsOfType<Image>();
#endif

            var sceneSpritePathes = images
                .Where(x => x.sprite != null)
                .Select(x => AssetDatabase.GetAssetPath(x.sprite));

            var assetSpritePathes = AssetDatabase.FindAssets($"t:{typeof(Sprite).Name}", new string[]
            {
                spritesPath
            }).Select(x => AssetDatabase.GUIDToAssetPath(x));

            var result = assetSpritePathes.Where(x1 => sceneSpritePathes.All(x2 => x2 != x1));

            foreach (var filePath in result)
            {
                File.Delete(filePath.GetFullAssetPath());
            }

            DALogger.Log(FcuLocKey.log_sprites_removed.Localize(result.Count()));

            AssetDatabase.Refresh();
#endif
            yield return null;
        }
    }
}