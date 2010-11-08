using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Activities.Presentation.Metadata;
using System.Activities.Presentation.Toolbox;
using System.Reflection;
using System.ServiceModel.Activities;
using System.Activities.Statements;
using System.ServiceModel.Activities.Presentation.Factories;
using System.Activities;
using System.Activities.Presentation;
using System.Activities.Presentation.View;

namespace Xpand.ExpressApp.Workflow.Win.Controls
{
    /// <summary>
    /// Interaction logic for WpfWorkflowDesignerControl.xaml
    /// </summary>
    public partial class WpfWorkflowDesignerControl : UserControl
    {
        private WorkflowDesigner _workflowDesigner = new WorkflowDesigner();
        public WorkflowDesigner WorkflowDesigner
        {
            get
            {
                return _workflowDesigner;
            }
        }
        public WpfWorkflowDesignerControl()
        {
            InitializeComponent();
            RegisterMetadata();
            AddDesigner();
        }

        #region Private Methods

        /// <summary>
        /// Registers the metadata.
        /// </summary>
        private static void RegisterMetadata()
        {
            System.Activities.Core.Presentation.DesignerMetadata metaData = new System.Activities.Core.Presentation.DesignerMetadata();
            metaData.Register();
            AttributeTableBuilder builder = new AttributeTableBuilder();
            MetadataStore.AddAttributeTable(builder.CreateTable());
        }

        /// <summary>
        /// Creates the toolbox control.
        /// </summary>
        /// <returns></returns>
        private static ToolboxControl CreateToolboxControl()
        {
            //Create the ToolBoxControl
            ToolboxControl ctrl = new ToolboxControl();

            //Create a collection of category items
            ToolboxCategory systemCategory = new ToolboxCategory("System Activities");

            //Add Default System Activities
            var systemAssemblies = new List<Assembly>
                                       {
                                           typeof (Send).Assembly,
                                           typeof (Delay).Assembly,
                                           typeof (ReceiveAndSendReplyFactory).Assembly
                                       };

            var systemQuery = from asm in systemAssemblies
                              from type in asm.GetTypes()
                              where type.IsPublic &&
                              !type.IsNested &&
                              !type.IsAbstract &&
                              !type.ContainsGenericParameters &&
                              (typeof(Activity).IsAssignableFrom(type) ||
                              typeof(IActivityTemplateFactory).IsAssignableFrom(type))
                              orderby type.Name
                              select new ToolboxItemWrapper(type);

            systemQuery.ToList().ForEach(systemCategory.Add);



            //Adding the category to the ToolBox control.
            ctrl.Categories.Add(systemCategory);
            return ctrl;
        }

        /// <summary>
        /// Adds the designer.
        /// </summary>
        private void AddDesigner()
        {
            //Create an instance of WorkflowDesigner class
            this._workflowDesigner = new WorkflowDesigner();

            _workflowDesigner.Context.Items.Subscribe<Selection>(SelectionChanged);

            //Place the WorkflowDesigner in the middle column of the grid
            Grid.SetColumn(this._workflowDesigner.View, 1);
            Grid.SetRow(this._workflowDesigner.View, 1);
            // Flush the workflow when the model changes
            _workflowDesigner.ModelChanged += (s, e) =>
            {
                _workflowDesigner.Flush();
                textXAML.Text = _workflowDesigner.Text;
            };

            //Load a new Sequence as default.
            // TODO: DJA
            this._workflowDesigner.Load(new Sequence());

            //Add the WorkflowDesigner to the grid
            LayoutGrid.Children.Add(this._workflowDesigner.View);

            // Add the Property Inspector
            Grid.SetColumn(_workflowDesigner.PropertyInspectorView, 2);
            Grid.SetRow(this._workflowDesigner.PropertyInspectorView, 1);
            LayoutGrid.Children.Add(_workflowDesigner.PropertyInspectorView);

            // Add the toolbox
            ToolboxControl tc = CreateToolboxControl();
            Grid.SetColumn(tc, 0);
            Grid.SetRow(tc, 1);
            LayoutGrid.Children.Add(tc);
        }

        /// <summary>
        /// Selections the changed.
        /// </summary>
        /// <param name="selection">The selection.</param>
        private void SelectionChanged(Selection selection)
        {
            var modelItem = selection.PrimarySelection;
            var sb = new StringBuilder();

            while (modelItem != null)
            {
                var displayName = modelItem.Properties["DisplayName"];

                if (displayName != null)
                {
                    if (sb.Length > 0)
                        sb.Insert(0, " - ");
                    sb.Insert(0, displayName.ComputedValue);
                }

                modelItem = modelItem.Parent;
            }

            CurrentActivityName.Text = sb.ToString();
        }

        #endregion Private Methods
    }
}
