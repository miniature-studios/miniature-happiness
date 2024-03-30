using DA_Assets.FCU;
using UnityEngine;

namespace DA_Assets.Shared
{
    public class Footer
    {
        public static DAInspector gui => DAInspector.Instance;
        public static void DrawFooter()
        {
            gui.Space30();

            gui.DrawGroup(new Group
            {
                GroupType = GroupType.Horizontal,
                Flexible = true,    
                Body = () =>
                {
                    if (gui.LinkButton($"made by\n{DAConstants.Publisher}"))
                    {
                        Application.OpenURL(DAConstants.SiteLink);
                    }
                }
            });
        }
    }
}
