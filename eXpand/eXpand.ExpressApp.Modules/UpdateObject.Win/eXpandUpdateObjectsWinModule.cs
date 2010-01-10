using System.ComponentModel;
using DevExpress.ExpressApp;
using DevExpress.Utils;
using EditorBrowsableState=System.ComponentModel.EditorBrowsableState;

namespace eXpand.ExpressApp.UpdateObject.Win
{
    [Description(
        "Includes Property Editors and Controllers to enable to extend UpdateObjects Module for Windows platform"),
     ToolboxTabName("eXpressApp"), EditorBrowsable(EditorBrowsableState.Always), Browsable(true), ToolboxItem(true)]
    public sealed partial class eXpandUpdateObjectsWinModule : ModuleBase
    {
        public eXpandUpdateObjectsWinModule()
        {
            InitializeComponent();
        }
    }
}