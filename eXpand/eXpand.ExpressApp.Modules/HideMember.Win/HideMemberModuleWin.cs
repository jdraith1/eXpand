using System.ComponentModel;
using DevExpress.ExpressApp;
using DevExpress.Utils;

namespace eXpand.ExpressApp.HideMember.Win
{
    [Description("Allows user to protect a member for a specific record"), ToolboxTabName("eXpressApp"),
     EditorBrowsable(EditorBrowsableState.Always), Browsable(true), ToolboxItem(true)]
    public sealed partial class HideMemberModuleWin : ModuleBase
    {
        public HideMemberModuleWin()
        {
            InitializeComponent();
        }
    }
}