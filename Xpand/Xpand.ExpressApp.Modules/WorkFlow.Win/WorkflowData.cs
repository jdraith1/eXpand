using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.Base;
using DevExpress.Xpo;

namespace Xpand.ExpressApp.Workflow.Win
{
    [DefaultClassOptions]
    public class WorkflowData : BaseObject
    {
        public WorkflowData(Session session) : base(session) { }

        private string name;
        public string Name
        {
            get { return name; }
            set { SetPropertyValue("Name", ref name, value); }
        }

        private string content;
        [Size(SizeAttribute.Unlimited)]
        [Custom("PropertyEditorType", "Xpand.ExpressApp.Workflow.Win.Editors.WorkflowPropertyEditor")]
        public string Content
        {
            get { return content; }
            set { SetPropertyValue("Content", ref content, value); }
        }
    }
}
