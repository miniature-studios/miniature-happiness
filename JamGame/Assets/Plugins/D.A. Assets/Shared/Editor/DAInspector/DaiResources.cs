using System;
using UnityEngine;

namespace DA_Assets.Shared
{
    [Serializable]
    public class DAIResources
    {
        [SerializeField] Texture2D fcuLogo;
        public Texture2D FcuLogo => fcuLogo;

        [SerializeField] Texture2D daGradientLogo;
        public Texture2D DAGradientLogo => daGradientLogo;

        [SerializeField] Texture2D daButtonLogo;
        public Texture2D DAButtonLogo => daButtonLogo;

        [Space]////////////////////////////////////////

        [SerializeField] Texture2D imgViewRecent;
        public Texture2D ImgViewRecent => imgViewRecent;

        [SerializeField] Texture2D imgExpandClosed;
        public Texture2D ImgExpandClosed => imgExpandClosed;

        [SerializeField] Texture2D imgExpandOpened;
        public Texture2D ImgExpandOpened => imgExpandOpened;

        [SerializeField] Texture2D iconExpandWindow;
        public Texture2D IconExpandWindow => iconExpandWindow;

        [SerializeField] Texture2D iconMinimizeWindow;
        public Texture2D IconMinimizeWindow => iconMinimizeWindow;

        [SerializeField] Texture2D iconStop;
        public Texture2D IconStop => iconStop;

        [SerializeField] Texture2D iconDownload;
        public Texture2D IconDownload => iconDownload;

        [SerializeField] Texture2D iconImport;
        public Texture2D IconImport => iconImport;

        [SerializeField] Texture2D[] cornerIcons;
        /// <summary>
        /// 0 - top left, 1 - top right, 3 - bottom left, 2 - bottom right
        /// </summary>
        public Texture2D[] CornerIcons => cornerIcons;

        [SerializeField] Texture2D iconSettings;
        public Texture2D IconSettings => iconSettings;

        [SerializeField] Texture2D iconOpen;
        public Texture2D IconOpen => iconOpen;

        [SerializeField] Texture2D iconAuth;
        public Texture2D IconAuth => iconAuth;

        [SerializeField] Texture2D imgComponent;
        public Texture2D ImgComponent => imgComponent;

        [Space]////////////////////////////////////////

        [SerializeField] Texture2D imgStar;
        public Texture2D ImgStar => imgStar;

        [SerializeField] Texture2D fcuIcon;
        public Texture2D FcuIcon => fcuIcon;

        [Space]////////////////////////////////////////

        [SerializeField] Texture2D clickIcon;
        public Texture2D ClickIcon => clickIcon;

        [SerializeField] Texture2D hoverIcon;
        public Texture2D HoverIcon => hoverIcon;

        [SerializeField] Texture2D loopIcon;
        public Texture2D LoopIcon => loopIcon;

        [SerializeField] Texture2D searchIcon;
        public Texture2D SearchIcon => searchIcon;
    }
}