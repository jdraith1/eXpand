using System.ComponentModel;
using DevExpress.ExpressApp;
using DevExpress.Utils;

namespace eXpand.ExpressApp.UpdateObject
{
    [Description("Includes Property Editors and Controllers to enable massive update of Object Members"), ToolboxTabName("eXpressApp"), EditorBrowsable(EditorBrowsableState.Always), Browsable(true), ToolboxItem(true)]
    public sealed partial class eXpandUpdateObjectModule : ModuleBase
    {
        public eXpandUpdateObjectModule()
        {
            InitializeComponent();
        }
    }
}