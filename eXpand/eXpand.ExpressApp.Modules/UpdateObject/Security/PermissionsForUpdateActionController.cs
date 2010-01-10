using DevExpress.ExpressApp;
using eXpand.ExpressApp.UpdateObject.Controllers;

namespace eXpand.ExpressApp.UpdateObject.Security
{
    public partial class PermissionsForUpdateActionController : ViewController
    {
        public PermissionsForUpdateActionController()
        {
            InitializeComponent();
            RegisterActions(components);
            TargetObjectType = typeof (MassiveUpdateObjectPermission);
            TargetViewType=ViewType.DetailView;
        }

        protected override void OnActivated()
        {
            base.OnActivated();
            View.ControlsCreated+=(sender,e) => ((MassiveUpdateObjectPermission)View.CurrentObject).ActionId =
                    Frame.GetController<MassiveUpdateViewController>().MassiveUpdateChoiceAction.Id;
        }

    }
}
