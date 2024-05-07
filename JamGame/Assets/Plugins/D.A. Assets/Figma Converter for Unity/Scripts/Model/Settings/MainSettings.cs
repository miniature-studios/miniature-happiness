using System;
using DA_Assets.Shared;
using DA_Assets.Shared.Extensions;
using UnityEngine;

namespace DA_Assets.FCU.Model
{
    [Serializable]
    public class MainSettings : MonoBehaviourBinder<FigmaConverterUnity>
    {
        [SerializeField]
        private UIFramework uiFramework = UIFramework.UGUI;
        public UIFramework UIFramework
        {
            get => uiFramework;
            set
            {
                switch (value)
                {
                    case UIFramework.UITK:
#if UITKPLUGIN_EXISTS == false
                        DALogger.LogError(
                            FcuLocKey.log_asset_not_imported.Localize(nameof(UIFramework.UITK))
                        );
                        uiFramework = UIFramework.UGUI;
                        return;
#endif
                }

                SetValue(ref uiFramework, value);
            }
        }

        [SerializeField]
        private PositioningMode positioningMode = PositioningMode.Absolute;
        public PositioningMode PositioningMode
        {
            get => positioningMode;
            set => SetValue(ref positioningMode, value);
        }

        [SerializeField]
        private PivotType pivotType = PivotType.MiddleCenter;
        public PivotType PivotType
        {
            get => pivotType;
            set => SetValue(ref pivotType, value);
        }

        [SerializeField]
        private ImageFormat imageFormat = ImageFormat.PNG;
        public ImageFormat ImageFormat
        {
            get => imageFormat;
            set => SetValue(ref imageFormat, value);
        }

        [SerializeField]
        private float imageScale = 4.0f;
        public float ImageScale
        {
            get => imageScale;
            set => SetValue(ref imageScale, value);
        }

        [SerializeField]
        private float pixelsPerUnit = 100;
        public float PixelsPerUnit
        {
            get => pixelsPerUnit;
            set => SetValue(ref pixelsPerUnit, value);
        }

        [SerializeField]
        private int goLayer = 5;
        public int GameObjectLayer
        {
            get => goLayer;
            set => SetValue(ref goLayer, value);
        }

        [SerializeField]
        private string spritesPath = "Assets\\Sprites";
        public string SpritesPath
        {
            get => spritesPath;
            set => SetValue(ref spritesPath, value);
        }

        [SerializeField]
        private string uguiOutputPath = "Assets\\UGUI Output";
        public string UGUIOutputPath
        {
            get => uguiOutputPath;
            set => SetValue(ref uguiOutputPath, value);
        }

        [SerializeField]
        private bool redownloadSprites = false;
        public bool RedownloadSprites
        {
            get => redownloadSprites;
            set => SetValue(ref redownloadSprites, value);
        }

        [SerializeField]
        private bool rawImport = false;
        public bool RawImport
        {
            get => rawImport;
            set
            {
                if (value && value != rawImport)
                {
                    DALogger.LogError(
                        FcuLocKey.log_dev_function_enabled.Localize(
                            FcuLocKey.label_raw_import.Localize()
                        )
                    );
                }

                SetValue(ref rawImport, value);
            }
        }

        [SerializeField]
        private bool windowMode = false;
        public bool WindowMode
        {
            get => windowMode;
            set => SetValue(ref windowMode, value);
        }

        [SerializeField]
        private string projectUrl;
        public string ProjectUrl
        {
            get => projectUrl;
            set
            {
                string _value = value;

                try
                {
                    string fileTag = "/file/";
                    char del = '/';

                    if (_value.IsEmpty()) { }
                    else if (_value.Contains(fileTag))
                    {
                        _value = _value.GetBetween(fileTag, del.ToString());
                    }
                    else if (_value.Contains(del.ToString()))
                    {
                        string[] splited = value.Split(del);
                        _value = splited[4];
                    }
                }
                catch
                {
                    Debug.LogError(FcuLocKey.log_incorrent_project_url.Localize());
                }

                SetValue(ref projectUrl, _value);
            }
        }

        [SerializeField]
        private string[] componentsUrls = new string[5];
        public string[] ComponentsUrls
        {
            get
            {
                if (componentsUrls.IsEmpty())
                {
                    componentsUrls = new string[5];
                }

                return componentsUrls;
            }
            set => SetValue(ref componentsUrls, value);
        }
    }
}
