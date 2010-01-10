namespace eXpand.ExpressApp.UpdateObject.Controllers{
    partial class MassiveUpdateViewController
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            DevExpress.ExpressApp.Actions.ChoiceActionItem choiceActionItem1 = new DevExpress.ExpressApp.Actions.ChoiceActionItem();
            DevExpress.ExpressApp.Actions.ChoiceActionItem choiceActionItem2 = new DevExpress.ExpressApp.Actions.ChoiceActionItem();
            this.massiveUpdateChoiceAction = new DevExpress.ExpressApp.Actions.SingleChoiceAction(this.components);
            // 
            // massiveUpdateChoiceAction
            // 
            this.massiveUpdateChoiceAction.Caption = "Update All";
            this.massiveUpdateChoiceAction.Id = "massiveUpdateChoiceAction";
            choiceActionItem1.ActiveItemsBehavior = DevExpress.ExpressApp.Actions.ActiveItemsBehavior.RequireActiveItems;
            choiceActionItem1.Caption = "Selected Items";
            choiceActionItem1.Data = "Selected";
            choiceActionItem1.VisibleItemsBehavior = DevExpress.ExpressApp.Actions.VisibleItemsBehavior.RequireVisibleItems;
            choiceActionItem2.ActiveItemsBehavior = DevExpress.ExpressApp.Actions.ActiveItemsBehavior.RequireActiveItems;
            choiceActionItem2.Caption = "All Items";
            choiceActionItem2.Data = "All Items";
            choiceActionItem2.VisibleItemsBehavior = DevExpress.ExpressApp.Actions.VisibleItemsBehavior.RequireVisibleItems;
            this.massiveUpdateChoiceAction.Items.Add(choiceActionItem1);
            this.massiveUpdateChoiceAction.Items.Add(choiceActionItem2);
            this.massiveUpdateChoiceAction.ItemType = DevExpress.ExpressApp.Actions.SingleChoiceActionItemType.ItemIsOperation;
            this.massiveUpdateChoiceAction.Execute += new DevExpress.ExpressApp.Actions.SingleChoiceActionExecuteEventHandler(this.massiveUpdateChoiceAction_Execute);

        }

        #endregion

        private DevExpress.ExpressApp.Actions.SingleChoiceAction massiveUpdateChoiceAction;

    }
}