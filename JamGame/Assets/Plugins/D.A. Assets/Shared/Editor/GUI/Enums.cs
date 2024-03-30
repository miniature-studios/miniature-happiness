namespace DA_Assets.Shared
{
    public enum GuiStyle
    {
        None = 0,
        Background = 1,
        Logo = 2,
        SectionHeader = 3,

        OutlineButton = 100,
        SquareButton30x30 = 101,
        TabButton = 102,
        LinkButton = 103,

        ProgressBar = 200,
        ProgressBarBg = 201,

        TextField = 300,
        BigTextField = 301,
        CheckBoxField = 302,

        HamburgerTabsBg = 400,

        HamburgerButton = 500,
        HambugerButtonBg = 501,
        HabmurgerImageSubButton = 502,
        HabmurgerTextSubButton = 503,
        HamburgerExpandButton = 504,

        Label12px = 600,
        RedLabel10px = 601,
        BlueLabel10px = 602,
        Label10px = 603,
        CheckBoxLabel = 604,

        TabBg1 = 700,
        TabBg2 = 701,
        ImgStar = 702,

        Group6Buttons = 800,
        Group5Buttons = 801,
        WindowRootBg = 802,
        LinkLabel10px = 803,
        Group2Buttons = 804,
        BigFieldLabel12px = 805,
        DAInspectorBackground = 806,
        DiffCheckerRightPanel = 807,
        DiffCheckerBackground = 808,
        DiffCheckerToImportPanel = 809,
        DiffCheckerToRemovePanel = 810,
        LabelCentered12px = 811,
        BoxPanel = 812,
    }

    public enum WidthType
    {
        Default = 0,
        Option = 1,
        Expand = 2
    }

    public enum GroupType
    {
        Horizontal = 0,
        Vertical = 1,
        Fade = 2
    }
}