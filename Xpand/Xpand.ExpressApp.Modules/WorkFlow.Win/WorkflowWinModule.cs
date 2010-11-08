using DevExpress.ExpressApp;
using Xpand.ExpressApp.Win.SystemModule;

namespace Xpand.ExpressApp.Workflow.Win
{
    public sealed class WorkflowWinModule : ModuleBase
    {
        public WorkflowWinModule() {
            RequiredModuleTypes.Add(typeof(XpandSystemWindowsFormsModule));
            RequiredModuleTypes.Add(typeof(WorkflowModule));
        }
    }
}
