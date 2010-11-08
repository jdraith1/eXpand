using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Activities.Presentation;

namespace Xpand.ExpressApp.Workflow.Win.Controls
{
    public partial class WorkflowDesignerControl : UserControl
    {
        public WorkflowDesignerControl()
        {
            InitializeComponent();
        }

        public string XmlString
        {
            get
            {
                return HostedWorkflowDesigner.WorkflowDesigner.Text;
            }
            set
            {
                HostedWorkflowDesigner.WorkflowDesigner.Text = value;
            }
        }


    }
}
