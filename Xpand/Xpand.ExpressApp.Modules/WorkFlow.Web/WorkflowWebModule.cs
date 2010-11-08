

using Xpand.ExpressApp.Web.SystemModule;

namespace Xpand.ExpressApp.Workflow.Web {
    public sealed class WorkflowWebModule : XpandModuleBase {
        public WorkflowWebModule() {
            RequiredModuleTypes.Add(typeof(WorkflowModule));
            RequiredModuleTypes.Add(typeof(XpandSystemAspNetModule));
        }
    }
}
