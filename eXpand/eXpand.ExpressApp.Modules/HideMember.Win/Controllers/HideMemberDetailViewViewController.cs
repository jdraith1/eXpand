using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Security;
using DevExpress.ExpressApp.Win.Editors;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;
using DevExpress.XtraEditors;
using DevExpress.XtraLayout;
using eXpand.ExpressApp.Core;
using eXpand.ExpressApp.HideMember.Win.Security;
using eXpand.ExpressApp.SystemModule;

namespace eXpand.ExpressApp.HideMember.Win.Controllers
{
    public partial class HideMemberDetailViewViewController : BaseViewController
    {
        private readonly Dictionary<string, ControlHelper> controlHelpers = new Dictionary<string, ControlHelper>();


        public HideMemberDetailViewViewController()
        {
            InitializeComponent();
            RegisterActions(components);
            TargetViewType = ViewType.DetailView;
        }

        private DetailView detailView { get; set; }

        private void Initialize(string name)
        {
            if (!controlHelpers.ContainsKey(name))
                controlHelpers.Add(name, new ControlHelper());
            InitLayoutControltem(name);
            InitDefaultControl(name);
            InitNewControl(name);
        }

        protected override void OnActivated()
        {
            base.OnActivated();
//            if (SecuritySystem.CurrentUser is BasicUser)
//            {
                View.ControlsCreated += View_OnControlsCreated;
                View.CurrentObjectChanged += View_CurrentObjectChanged;
                detailView = (DetailView) View;
//            }
        }


        private void View_OnControlsCreated(object sender, EventArgs e)
        {
            if (hasHidePermission())
                SetEditorButtons(detailView.GetPropertyEditors(typeof (ButtonEdit)));
            else
                hideMembers();
        }

        private bool hasHidePermission()
        {
            if (!(SecuritySystem.Instance is ISecurityComplex))
                return ((BasicUser)SecuritySystem.CurrentUser).IsAdministrator;
            return SecuritySystem.IsGranted(new HideMemberPermission(HideMemberPermissionModifier.Allow));
        }

        protected override void OnDeactivating()
        {
            base.OnDeactivating();
            View.CurrentObjectChanged -= View_CurrentObjectChanged;
            detailView = null;
        }


        private void View_CurrentObjectChanged(object sender, EventArgs e)
        {
            hideMembers();
        }

        private void hideMembers()
        {
            if (SecuritySystem.CurrentUser != null)
            {
                ICollection<PropertyEditor> propertyEditors = detailView.GetPropertyEditors(typeof (ButtonEdit));
                if (!hasHidePermission())
                    ReplaceEditorControl((from editor in propertyEditors
                                          select editor.PropertyName).ToList());
                else
                    SetEditorButtons(propertyEditors);
            }
        }

        private void SetEditorButtons(IEnumerable<PropertyEditor> propertyEditors)
        {
            foreach (DXPropertyEditor propertyEditor in propertyEditors)
                HideMemberListViewViewController.SetEditorButtonKind(
                    ((ButtonEdit) propertyEditor.Control).Properties, (XPBaseObject) View.CurrentObject,
                    propertyEditor.PropertyName);
        }


        private void ReplaceEditorControl(IEnumerable<string> propertyNames)
        {
            var xpBaseObject = View.CurrentObject as XPBaseObject;
            if (xpBaseObject!= null)
            {
                foreach (string name in propertyNames)
                {
                    Initialize(name);
                    if (HideMemberListViewViewController.IsProtected(xpBaseObject, name).IsProtected)
                        ReplaceEditorControl(controlHelpers[name].NewControl, controlHelpers[name]);
                    else
                        ReplaceEditorControl(controlHelpers[name].DefaultControl, controlHelpers[name]);
                }
            }
        }

        private void ReplaceEditorControl(Control newControl, ControlHelper controlHelper)
        {
            if (controlHelper.LayoutControlItem != null)
            {
                Control oldControl = controlHelper.LayoutControlItem.Control;
                if (ReferenceEquals(newControl, oldControl))
                    return;

                bool enabled = newControl.Enabled;

                controlHelper.LayoutControlItem.Owner.BeginUpdate();
                controlHelper.LayoutControlItem.BeginInit();

                controlHelper.LayoutControlItem.Control = newControl;
                oldControl.Parent = null;

                controlHelper.LayoutControlItem.EndInit();
                controlHelper.LayoutControlItem.Owner.EndUpdate();

                controlHelper.LayoutControlItem.Control.Enabled = enabled;
            }
        }

        private void InitLayoutControltem(string name)
        {
            if (controlHelpers[name].LayoutControlItem == null)
            {
                int hash = FindControlHashByPropertyName(name);
                if (hash != 0)
                    controlHelpers[name].LayoutControlItem = FindLayoutControlItemByControlHash(hash, name);
            }
        }

        private void InitDefaultControl(string name)
        {
            if (controlHelpers[name].DefaultControl == null)
            {
                DetailViewItem item = detailView.FindItem(name);
                if (item != null)
                    controlHelpers[name].DefaultControl = (Control) item.Control;
            }
        }

        private void InitNewControl(string name)
        {
            if (controlHelpers[name].NewControl == null)
                controlHelpers[name].NewControl = new ProtectedContentEdit();
        }

        private int FindControlHashByPropertyName(string name)
        {
            DetailViewItem item = detailView.FindItem(name);
            if (item != null)
                return item.Control.GetHashCode();
            return 0;
        }

        private LayoutControlItem FindLayoutControlItemByControlHash(int hash, string name)
        {
            foreach (object obj in (((LayoutControl) (detailView.Control))).Items)
                if (obj is LayoutControlItem)
                {
                    var item = (LayoutControlItem) obj;
                    if (item.Control != null && item.Control.GetHashCode() == hash)
                    {
                        controlHelpers[name].LayoutControlItem = item;
                        return controlHelpers[name].LayoutControlItem;
                    }
                }
            return null;
        }
        #region Nested type: ControlHelper
        public class ControlHelper
        {
            public Control DefaultControl { get; set; }


            public LayoutControlItem LayoutControlItem { get; set; }

            public Control NewControl { get; set; }
        }
        #endregion
    }
}