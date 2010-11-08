using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Win.Editors;
using DevExpress.ExpressApp.Model;
using Xpand.ExpressApp.Workflow.Win.Controls;

namespace Xpand.ExpressApp.Workflow.Win.Editors
{
    [PropertyEditor(typeof(String), false)]
    public class WorkflowPropertyEditor : WinPropertyEditor
    {
        public WorkflowPropertyEditor(Type objectType, IModelMemberViewItem info)
            : base(objectType, info)
        {
            ControlBindingProperty = "XmlString";
        }

        protected override object CreateControlCore()
        {
            return new WorkflowDesignerControl();
        }

        public new WorkflowDesignerControl Control
        {
            get { return ((WorkflowDesignerControl)base.Control); }
        }
    }
}
