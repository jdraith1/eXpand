using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.RibbonUI.Win.Templates;
using DevExpress.ExpressApp.Win.Controls;
using DevExpress.ExpressApp.Win.Templates;
using DevExpress.Utils;
using DevExpress.Xpo;
using DevExpress.XtraBars;
using DevExpress.XtraEditors.Repository;
using eXpand.ExpressApp.UpdateObject.Controllers;

namespace eXpand.ExpressApp.UpdateObject.Win.Controllers
{
    public partial class MassiveUpdateViewController :
        UpdateObject.Controllers.MassiveUpdateViewController
    {
        private static MainForm mainForm;

        private readonly Dictionary<string, BarEditItemLink> barEditItemLinks =
            new Dictionary<string, BarEditItemLink>();

        public MassiveUpdateViewController()
        {
            InitializeComponent();
            RegisterActions(components);
        }


        protected override void OnActivated()
        {
            base.OnActivated();
            if (Frame.Template is MainForm)
                mainForm = (MainForm) Frame.Template;
        }

        protected override void InvokeCommitted(EventArgs e)
        {
            base.InvokeCommitted(e);
            var eventArgs = (ViewEventArgs) e;
            barEditItemLinks[eventArgs.ViewId].EditValue = 0;
            barEditItemLinks[eventArgs.ViewId].Bar.Manager.StatusBar.ItemLinks.Remove(barEditItemLinks[eventArgs.ViewId]);
            barEditItemLinks.Remove(eventArgs.ViewId);
        }

        protected override void InvokeObjectEndEdit(EndEditEventArgs e)
        {
            base.InvokeObjectEndEdit(e);
            RaiseProgress(e.Object);
        }


        protected override void InvokeObjectSaved(ObjectManipulatingEventArgs e)
        {
            base.InvokeObjectSaved(e);
            RaiseProgress(e.Object);
        }

        private void RaiseProgress(object viewId)
        {
            barEditItemLinks[viewId.ToString()].Bar.Manager.Form.Invoke(
                new RaiseProgressDelegate(
                    id => barEditItemLinks[id].EditValue = ((int) barEditItemLinks[id].EditValue) + 1),
                viewId.ToString());
        }

        private BarItemLink FindBarEditItem(IEnumerable barItemLinkCollection)
        {
            if (barEditItemLinks[View.Id] == null)
                barEditItemLinks[View.Id] = (barItemLinkCollection.Cast<BarItemLink>().OfType<BarEditItemLink>().Where(
                    barEditItem => ReferenceEquals(barEditItem.Item.Tag, View.Id))).FirstOrDefault();
            return barEditItemLinks[View.Id];
        }

        protected override Dictionary<IMemberInfo, object> GetUpdateDictionary(XPBaseObject createdXpBaseObject,
                                                                               int itemsCount, string viewId,
                                                                               UnitOfWork unitOfWork)
        {
            Dictionary<IMemberInfo, object> dictionary = base.GetUpdateDictionary(createdXpBaseObject, itemsCount,
                                                                                  viewId, unitOfWork);
            if (!barEditItemLinks.ContainsKey(View.Id))
                barEditItemLinks.Add(View.Id, null);
            bool bar = addProgressBar(itemsCount*(dictionary.Count + 1));
            return bar ? dictionary : null;
        }

        private bool addProgressBar(int max)
        {
            var template = mainForm as IBarManagerHolder;
            if (template != null)
            {
                BarManager barManager = template.BarManager;

                Bar bar = barManager.StatusBar;
                BarEditItem barEditItem = GetBarEditItem(template, max);
                if (bar != null)
                {
                    BarItemLink findBarEditItem = FindBarEditItem(bar.ItemLinks);
                    if (findBarEditItem != null)
                    {
                        ((RepositoryItemProgressBar) ((BarEditItem) findBarEditItem.Item).Edit).Maximum = max;
                        return false;
                    }
                    bar.ItemLinks.Add(barEditItem);
                    if (typeof (RibbonFormTemplateBase).IsAssignableFrom(template.GetType()))
                        ((RibbonFormTemplateBase) template).Ribbon.StatusBar.ItemLinks.Add(barEditItem);
                    FindBarEditItem(bar.ItemLinks);
                    return true;
                }
            }
            return false;
        }

//        private void ToolTipController_OnGetActiveObjectInfo(object sender, ToolTipControllerGetActiveObjectInfoEventArgs e)
//        {
//            if (e.SelectedControl is ProgressBarControl)
//                ((ToolTipController) sender).SetToolTip(e.SelectedControl,View.Id);
//        }

        private BarEditItem GetBarEditItem(IBarManagerHolder template, int max)
        {
            var sTooltip2 = new SuperToolTip();
            var args = new SuperToolTipSetupArgs();
            args.Title.Text = View.Id;
            sTooltip2.Setup(args);
            var repositoryItemProgressBar = new RepositoryItemProgressBar {Maximum = max};
            var barEditItem = new BarEditItem(template.BarManager)
                                  {
                                      Edit = repositoryItemProgressBar,
                                      Width = 500,
                                      Tag = View.Id,
                                      EditValue = 0,
                                      SuperTip = sTooltip2,
                                      AutoFillWidth = true,
                                  };

            return barEditItem;
        }
        #region Nested type: RaiseProgressDelegate
        private delegate void RaiseProgressDelegate(string viewId);
        #endregion
    }
}