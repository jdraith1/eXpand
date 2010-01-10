using System;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Win.Editors;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Mask;
using DevExpress.XtraEditors.Repository;

namespace eXpand.ExpressApp.HideMember.Win.PropertyEditors
{
    public class HideMemberStringPropertyEditor : StringPropertyEditor,IHideMemberPropertyEditor
    {
//        private bool protect;
//        private string protectedContentText;

        public HideMemberStringPropertyEditor(Type objectType, DictionaryNode info)
            : base(objectType, info)
        {
        }


//        protected override void OnCurrentObjectChanged()
//        {
//            base.OnCurrentObjectChanged();
//            
//            protect = HideMemberListViewViewController.IsProtected((XPBaseObject) CurrentObject, PropertyName);
//            if (View != null)
//            {
//
//                //here i will be after the detailview is loaded and the user press the next button. how can i force recreation ButtonEdit/ProtectedContentEdit Control
//                
//            }
//        }

        protected override object CreateControlCore()
        {
//            if (protect)
//                return new ProtectedContentEdit();
            return new ButtonEdit();
        }

//        protected override bool IsMemberReadOnly()
//        {
////            if (protect)
////                return true;
//            return base.IsMemberReadOnly();
//        }

        protected override RepositoryItem CreateRepositoryItem()
        {
//            if (protect)
//                return new RepositoryItemProtectedContentTextEdit();
            return new RepositoryItemButtonEdit();
        }

        protected override void SetupRepositoryItem(RepositoryItem item)
        {
//            if (protect)
//                ((RepositoryItemProtectedContentTextEdit) item).ProtectedContentText = protectedContentText;
//            else
//            {
            var repositoryItemButtonEdit = (RepositoryItemButtonEdit) item;
            repositoryItemButtonEdit.Buttons.Clear();
            if (!string.IsNullOrEmpty(EditMask))
            {
                repositoryItemButtonEdit.Mask.EditMask = EditMask;
                switch (EditMaskType)
                {
                    case EditMaskType.RegEx:
                        repositoryItemButtonEdit.Mask.UseMaskAsDisplayFormat = false;
                        repositoryItemButtonEdit.Mask.MaskType = MaskType.RegEx;
                        break;
                    default:
                        repositoryItemButtonEdit.Mask.MaskType = MaskType.Simple;
                        break;
                }
            }
//            }
            OnCustomSetupRepositoryItem(new CustomSetupRepositoryItemEventArgs(item));
        }


//        protected override void ReadValueCore()
//        {
//            if (!protect)
//                base.ReadValueCore();
//        }
    }
}