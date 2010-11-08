using Xpand.ExpressApp.SystemModule;

namespace Xpand.ExpressApp.Workflow {
    public sealed class WorkflowModule : XpandModuleBase {
        public WorkflowModule() {
            RequiredModuleTypes.Add(typeof(XpandSystemModule));
        }
    }
}
