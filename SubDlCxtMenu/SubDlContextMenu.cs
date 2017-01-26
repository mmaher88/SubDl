using SharpShell.Attributes;
using SharpShell.SharpContextMenu;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SubDlCxtMenu
{
    [ComVisible(true)]
    [COMServerAssociation(AssociationType.ClassOfExtension, ".mp4", ".mkv", ".avi", ".mpg", ".flv", ".wmv", ".mov")]
    public class SubDlContextMenu : SharpContextMenu
    {
        protected override bool CanShowMenu()
        {
            return true;
        }

        protected override ContextMenuStrip CreateMenu()
        {
            var menu = new ContextMenuStrip();

            //  Create a 'Get Subtitles' item.
            var getSubMenuItem = new ToolStripMenuItem
            {
                Text = "Get Subtitles",
                Image = Properties.Resources.SubDlIconSmall
            };

            //  When we click, we'll count the lines.
            getSubMenuItem.Click += (sender, args) => GetSubtitles();

            //  Add the item to the context menu.
            menu.Items.Add(getSubMenuItem);

            return menu;
        }

        private void GetSubtitles()
        {
            foreach (var filePath in SelectedItemPaths)
            {
                //  Start SubDl process
                ProcessStartInfo subDlProcInfo = new ProcessStartInfo(@"C:\SubDl\SubDl.exe", "\"" + filePath + "\"");
                Process.Start(subDlProcInfo);
            }
        } 
    }
}
