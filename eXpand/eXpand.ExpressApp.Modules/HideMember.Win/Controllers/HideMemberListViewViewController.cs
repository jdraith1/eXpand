using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Win.Editors;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Grid;
using eXpand.ExpressApp.Attributes;
using eXpand.ExpressApp.SystemModule;
using eXpand.Xpo;

namespace eXpand.ExpressApp.HideMember.Win.Controllers
{
    public partial class HideMemberListViewViewController : BaseViewController
    {
        public const string STR_HideMemberModule = "HideMemberModule";
        private GridControl gridControl;
//        private readonly EditorButton editorButton = new EditorButton(ButtonPredefines.Plus);
        public HideMemberListViewViewController()
        {
            InitializeComponent();
            RegisterActions(components);
            TargetViewType=ViewType.ListView;
        }

        protected override void OnActivated()
        {
            base.OnActivated();
            
            if (SecuritySystem.CurrentUser is BasicUser )
                View.ControlsCreated += View_OnControlsCreated;
        }


        private void View_OnControlsCreated(object sender, EventArgs e)
        {
            gridControl = ( View.Control) as GridControl;
            if (gridControl != null) ((GridView) gridControl.MainView).CustomRowCellEdit += CustomRowCellEdit;
        }


        public static void SetEditorButtonKind(RepositoryItemButtonEdit repositoryItemButtonEdit, XPBaseObject baseObject, string propertyName)
        {
            EditorButton button = GetEditorButton(repositoryItemButtonEdit);
            if (button != null && baseObject != null)
            {
                MemberProtectedInfo isProtected = IsProtected(baseObject, propertyName);
                button.Appearance.BackColor2 = Color.FromArgb(0, 0, 0);
                if (isProtected.IsProtected && button.Kind != ButtonPredefines.Minus)
                {
                    button.Kind = ButtonPredefines.Minus;
                    button.Appearance.BackColor2=Color.BlueViolet;
                    if (isProtected.IsClassProtected)
                        button.Appearance.ForeColor=Color.Red;
                }
                else if (button.Kind != ButtonPredefines.Plus)
                    button.Kind = ButtonPredefines.Plus;
                repositoryItemButtonEdit.ButtonClick +=
                    (sender1, e1) => ChangeLockStatus(e1, baseObject, propertyName);
            }
        }

        public static MemberProtectedInfo IsProtected(XPBaseObject baseObject, string propertyName)
        {
            propertyName = propertyName.Replace("!", "");
            XPMemberInfo memberInfo = ReflectorHelper.GetXpMemberInfo(baseObject.Session, baseObject.GetType(),
                                                                      propertyName);

            XPClassInfo xpClassInfo = memberInfo.ReferenceType;
            FilterRecordAttribute attributeInfo = xpClassInfo != null
                                                      ? (FilterRecordAttribute)
                                                        xpClassInfo.FindAttributeInfo(
                                                            typeof (FilterRecordAttribute))
                                                      :
                                                          null;

            if ( memberInfo.IsPersistent)
            {
                if (attributeInfo != null &&attributeSatisfyUser(attributeInfo)&&
                    criteriaSatisfyMember(propertyName, baseObject, xpClassInfo, attributeInfo))
                    return new MemberProtectedInfo(true, true);
                var b = (bool?) ReflectorHelper.GetXpMemberInfoValue(propertyName +
                                                                     STR_HideMemberModule, baseObject);
                return new MemberProtectedInfo(false, b.HasValue && b.Value);
            }
            
            return new MemberProtectedInfo(false, false);
        }

        private static bool criteriaSatisfyMember(string propertyName, XPBaseObject baseObject, XPClassInfo xpClassInfo, FilterRecordAttribute attributeInfo)
        {
            return GetCriteriaValue(attributeInfo.TargetObjectCriteria,
                                    (XPBaseObject) ReflectorHelper.GetXpMemberInfoValue(propertyName, baseObject),
                                    xpClassInfo, baseObject.Session);
        }

        private static bool attributeSatisfyUser(FilterRecordAttribute attributeInfo)
        {
            return !(attributeInfo.ExcludeAdmin &&
                     ((BasicUser)SecuritySystem.CurrentUser).IsAdministrator);
        }

        private static bool GetCriteriaValue(string criteria, XPBaseObject baseObject,XPClassInfo xpClassInfo,Session session)
        {
            if (baseObject== null)
                return true;
            var collection = new XPCollection(session,xpClassInfo.ClassType,false){baseObject};
            collection.Filter = CriteriaOperator.Parse(criteria);
            return collection.Count==1;
        }

        private static EditorButton GetEditorButton(RepositoryItemButtonEdit repositoryItemButtonEdit)
        {
//            if (typeof (RepositoryItemButtonEdit).IsAssignableFrom(repositoryItemButtonEdit.GetType()))
//            {
            EditorButtonCollection buttons = repositoryItemButtonEdit.Buttons;

            EditorButton button = GetEditorButton(buttons);
            if (button == null)
            {
                repositoryItemButtonEdit.Buttons.Add(new EditorButton(ButtonPredefines.Plus));
                button = GetEditorButton(buttons);
            }
            return button;
//            }
        }

        public static EditorButton GetEditorButton(IEnumerable buttons)
        {
            return (from b in buttons.Cast<EditorButton>()
                    where b.Kind == ButtonPredefines.Plus || b.Kind == ButtonPredefines.Minus
                    select b).FirstOrDefault();
        }

        public override void CustomizeTypesInfo(ITypesInfo typesInfo)
        {
            XPDictionary xpDictionary = XafTypesInfo.XpoTypeInfoSource.XPDictionary;
            foreach (XPClassInfo classInfo in xpDictionary.Classes)
                foreach (XPMemberInfo memberInfo in classInfo.PersistentProperties)
                    try
                    {
                        if (classInfo.ClassType != null && classInfo.FindMember(memberInfo.Name + STR_HideMemberModule) == null &&
                            !memberInfo.Name.EndsWith(STR_HideMemberModule))
                        {
                            var attributes = new Attribute[]
                                                 {
                                                     new BrowsableAttribute(false),
                                                     new MemberDesignTimeVisibilityAttribute(false),
                                                     new NonIndexedAttribute(),
                                                     new NonCloneableAttribute()

                                                 };
                            classInfo.CreateMember(memberInfo.Name + STR_HideMemberModule, typeof(bool), attributes);
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }            

        }


        private void CustomRowCellEdit(object sender, CustomRowCellEditEventArgs e)
        {
            
            var baseObject = (XPBaseObject) ((GridView) sender).GetRow(e.RowHandle);

            if (((BasicUser) SecuritySystem.CurrentUser).IsAdministrator)
            {
                RepositoryItemButtonEdit clone;
                if (e.RepositoryItem.Clone() as RepositoryItemButtonEdit != null)
                    clone = e.RepositoryItem.Clone() as RepositoryItemButtonEdit;
                else if (e.RepositoryItem is RepositoryItemCheckEdit)
                    return;
                else
                {
                    clone = new RepositoryItemButtonEdit();
                    clone.Buttons.Clear();
                }
                SetEditorButtonKind(clone, baseObject, e.Column.FieldName);
                e.RepositoryItem = clone;
            }
            else
            {
                MemberProtectedInfo protectedInfo = IsProtected(baseObject, e.Column.FieldName);
                if (protectedInfo.IsProtected)
                    e.RepositoryItem = new RepositoryItemProtectedContentTextEdit();
            }
        }

        public static void ChangeLockStatus(ButtonPressedEventArgs e1, XPBaseObject baseObject, string propertyName)
        {
            if (e1.Button.Kind == ButtonPredefines.Plus)
            {        
                ReflectorHelper.SetXpMemberProperty(propertyName + STR_HideMemberModule, true,baseObject, true);
                //ReflectionHelper.SetMemberValue(baseObject, propertyName + STR_HideMemberModule, true);
//                baseObject.Save();
                e1.Button.Kind = ButtonPredefines.Minus;
            }
            else if (e1.Button.Kind == ButtonPredefines.Minus)
            {
                ReflectorHelper.SetXpMemberProperty(propertyName + STR_HideMemberModule, false, baseObject, false);
//                baseObject.Save();
                e1.Button.Kind = ButtonPredefines.Plus;
            }
        }


    }
}