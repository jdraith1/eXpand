using System;
using System.Collections;
using System.Collections.Generic;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.NodeWrappers;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.Xpo;
using DevExpress.Xpo.Helpers;
using eXpand.ExpressApp.SystemModule;
using eXpand.Persistent.BaseImpl;
using Wintellect.Threading.AsyncProgModel;
using eXpand.Utils.Threading;
using ListView=DevExpress.ExpressApp.ListView;
using eXpand.Xpo.Collections;

namespace eXpand.ExpressApp.UpdateObject.Controllers{
    public partial class MassiveUpdateViewController : BaseViewController
    {
        public event EventHandler Committed;

        protected virtual void InvokeCommitted(EventArgs e)
        {
            EventHandler committedHandler = Committed;
            if (committedHandler != null) committedHandler(this, e);
        }

        public SingleChoiceAction MassiveUpdateChoiceAction
        {
            get { return massiveUpdateChoiceAction; }
        }

        public event EventHandler<EndEditEventArgs> ObjectEndEdit;

        protected virtual void InvokeObjectEndEdit(EndEditEventArgs e)
        {
            EventHandler<EndEditEventArgs> objectEndEditHandler = ObjectEndEdit;
            if (objectEndEditHandler != null) objectEndEditHandler(this, e);
        }



        public event EventHandler<ObjectManipulatingEventArgs> ObjectSaved;

        protected virtual void InvokeObjectSaved(ObjectManipulatingEventArgs e)
        {
            EventHandler<ObjectManipulatingEventArgs> objectSavedHandler = ObjectSaved;
            if (objectSavedHandler != null) objectSavedHandler(this, e);
        }

        public const string MassiveUpdateDetailViewId = "MassiveUpdateDetailViewId";
        public MassiveUpdateViewController()
        {
            InitializeComponent();
            RegisterActions(components);
            TargetViewType=ViewType.ListView;

        }

        protected override void OnActivated()
        {
            base.OnActivated();
            if (!Application.Info.GetChildNode("Options").GetAttributeBoolValue("UseServerMode"))
                throw new NotImplementedException(string.Format("ServerMode should be enabled in order to use {0}", typeof(eXpandUpdateObjectModule).Name));
            massiveUpdateChoiceAction.TargetViewType=ViewType.ListView;
            massiveUpdateChoiceAction.Active[MassiveUpdateDetailViewId + "Attribute"] =
                !string.IsNullOrEmpty(View.Info.GetAttributeValue(MassiveUpdateDetailViewId));
            
        }



        public override Schema GetSchema()
        {
            return new Schema(new DictionaryXmlReader().ReadFromString(
                                  @"<Element Name=""Application"">
					                    <Element Name=""Views"">
						                    <Element Name=""ListView"">
                                                <Attribute Name=""" + MassiveUpdateDetailViewId + @""" RefNodeName=""{DevExpress.ExpressApp.Core.DictionaryHelpers.ViewIdRefNodeProvider}ClassName=@ClassName;ViewType=DetailView""/>
                                            </Element>
                                        </Element>
                                    </Element>"));
        }

        protected virtual IEnumerator<Int32> ProcessItems(AsyncEnumerator asyncEnumerator, IList items, XPBaseObject createdXpBaseObject,int itemsCount,string viewId)
        {
            using (var unitOfWork = new UnitOfWork(ObjectSpace.Session.DataLayer))
            {
                Dictionary<IMemberInfo, object> updateDictionary = GetUpdateDictionary(createdXpBaseObject, itemsCount, viewId, unitOfWork);
                if (updateDictionary != null)
                {
                    AsyncEnumeratorSyncHelper.BeginHelper(asyncEnumerator.End(), null, () =>
                                                                                       {

                                                                                           IList collectionSource = GetCollectionSource(items, unitOfWork, updateDictionary);

                                                                                           for (Int32 index = 0; index < itemsCount; index++)
                                                                                           {
                                                                                               var baseObject = ((XPBaseObject)collectionSource[index]);
                                                                                               var member = baseObject.ClassInfo.FindMember(eXpandBaseObject.CancelTriggerObjectChangedName);
                                                                                               if (member!= null)
                                                                                                   member.SetValue(baseObject, true);
                                                                                               SetObjectValues(baseObject, updateDictionary);
                                                                                               baseObject.Save();
                                                                                               InvokeObjectEndEdit(new EndEditEventArgs(viewId));
                                                                                               if (member != null)
                                                                                                   member.SetValue(baseObject, false);
                                                                                           }
                                                                                           unitOfWork.ObjectSaved += (sender, e) => InvokeObjectSaved(new ObjectManipulatingEventArgs(viewId));
                                                                                           unitOfWork.CommitChanges();
                                                                                       });

                    yield return 1;
                    AsyncEnumeratorSyncHelper.EndHelper(asyncEnumerator.DequeueAsyncResult());
                    InvokeCommitted(new ViewEventArgs(viewId));
                }
            }
        }

        private IList GetCollectionSource(ICollection items, Session unitOfWork, Dictionary<IMemberInfo, object> updateDictionary)
        {
            var source = (items as XpoServerModeGridObjectDataSource);
            if (source != null)
            {
                string displayAbleProperties = null;
                foreach (var pair in updateDictionary)
                    displayAbleProperties += pair.Key.Name + ";";
                return new XpoServerModeGridObjectDataSource(unitOfWork,
                                                             unitOfWork.GetClassInfo(
                                                                 View.ObjectTypeInfo.Type),
                                                             source.FullCriteria,
                                                             displayAbleProperties);
            }
            var baseObjects = new List<XPBaseObject>();
            foreach (var item in items)
                baseObjects.Add((XPBaseObject) unitOfWork.GetObjectByKey(item.GetType(), ((XPBaseObject) item).ClassInfo.KeyProperty.GetValue(item), true));
            return baseObjects;
        }

        private void SetObjectValues(XPBaseObject baseObject, Dictionary<IMemberInfo, object> updateDictionary)
        {
            foreach (var valuePair in updateDictionary)
                valuePair.Key.SetValue(baseObject,valuePair.Value);
//                baseObject.SetMemberValue(valuePair.Key, valuePair.Value);
        }

        protected virtual void UpdateObjects(IList items, ObjectSpace objectSpace, Dictionary<string, object> updateDictionary)
        {
            if (items is XPBaseCollection)
            {
                var pageSelector = new XPPageSelector((XPBaseCollection) items);
                for (int i = 0; i < pageSelector.PageCount; i++)
                {
                    pageSelector.CurrentPage = i;
                    UpdateList(pageSelector.Collection, objectSpace, updateDictionary);
                }
            }
            else
                UpdateList(items, objectSpace, updateDictionary);
        }

        private void UpdateList(IList items, ObjectSpace objectSpace, Dictionary<string, object> updateDictionary)
        {
            
            for (Int32 index = 0; index < items.Count; index++)
            {
                var baseObject = ((XPBaseObject) items[index]);
                baseObject=objectSpace.GetObject(baseObject);
                foreach (var valuePair in updateDictionary)
                    baseObject.SetMemberValue(valuePair.Key, valuePair.Value);
            }
        }

        protected virtual Dictionary<IMemberInfo, object> GetUpdateDictionary(XPBaseObject createdXpBaseObject, int itemsCount, string viewId,UnitOfWork unitOfWork)
        {
            var collection = new Dictionary<IMemberInfo, object>();
            var detailViewInfoNodeWrapper =
                new DetailViewInfoNodeWrapper(
                    Application.Info.GetChildNodeByPath("Views/DetailView[@ID='" + View.Info.GetAttributeValue(MassiveUpdateDetailViewId) + "']"));
            EditorsNodeWrapper editors = detailViewInfoNodeWrapper.Editors;
            foreach (DetailViewItemInfoNodeWrapper detailViewItem in editors.Items)
            {
                IMemberInfo memberInfo = View.ObjectTypeInfo.FindMember(detailViewItem.PropertyName);
                var value = memberInfo.GetValue(createdXpBaseObject);
                
                if (value is XPBaseObject)
                    value = unitOfWork.GetObjectByKey(value.GetType(), ((XPBaseObject) value).ClassInfo.KeyProperty.GetValue(value), true);
                collection.Add(memberInfo,value);
            }

            createdXpBaseObject.Delete();
            View.ObjectSpace.CommitChanges();

            return collection;
        }

        private void massiveUpdateChoiceAction_Execute(object sender, SingleChoiceActionExecuteEventArgs e)
        {
            ObjectSpace objectSpace = Application.CreateObjectSpace();
            DetailView detailView = Application.CreateDetailView(objectSpace,
                                                                 View.Info.GetAttributeValue(MassiveUpdateDetailViewId),
                                                                 false,
                                                                 Activator.CreateInstance(View.ObjectTypeInfo.Type, objectSpace.Session));
            
            var dialogController = new DialogController();
            dialogController.AcceptAction.Execute += (sender1,e1) => UpdateActionExecute(e.SelectedChoiceActionItem.Data,e1);
            e.ShowViewParameters.CreatedView=detailView;
            e.ShowViewParameters.Context=TemplateContext.PopupWindow;
            e.ShowViewParameters.TargetWindow=TargetWindow.NewModalWindow;
            e.ShowViewParameters.Controllers.Add(dialogController);
        }

        private void UpdateActionExecute(object data, SimpleActionExecuteEventArgs e)
        {
            var createdXpBaseObject = ((XPBaseObject)e.SelectedObjects[0]);
            var asyncEnumerator = new AsyncEnumerator();
            var items = ReferenceEquals(data, "Selected")? View.SelectedObjects:((ListView)View).CollectionSource.Collection;
            asyncEnumerator.BeginExecute(
                ProcessItems(asyncEnumerator, items, createdXpBaseObject,
                             typeof (XPBaseCollection).IsAssignableFrom(items.GetType())
                                 ? ((XPBaseCollection) items).GetCount()
                                 : items.Count, View.Id), asyncEnumerator.EndExecute);
        }
    }

    public class ViewEventArgs:EventArgs
    {
        public ViewEventArgs(string viewId)
        {
            this.viewId = viewId;
        }

        private readonly string viewId;
        public string ViewId
        {
            get { return viewId; }
        }    
    }
}