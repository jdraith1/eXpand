using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.Persistent.BaseImpl;
using eXpand.ExpressApp.Attributes;
using eXpand.ExpressApp.SystemModule;

namespace eXpand.ExpressApp.HideMember.Controllers
{
    public partial class FilterRecordViewController : BaseViewController
    {
        public const string FilterRecordViewControllerCriteriaKey = "FilterRecordViewController";

        public FilterRecordViewController()
        {
            InitializeComponent();
            RegisterActions(components);
            TargetViewType = ViewType.ListView;
        }

        protected override void OnActivated()
        {
            base.OnActivated();
            var filterRecordAttribute = View.ObjectTypeInfo.FindAttribute<FilterRecordAttribute>();
            if (filterRecordAttribute != null && satisfyUser(filterRecordAttribute))
                ((ListView) View).CollectionSource.Criteria[FilterRecordViewControllerCriteriaKey] =
                    CriteriaOperator.Parse(filterRecordAttribute.TargetObjectCriteria);
        }

        private bool satisfyUser(FilterRecordAttribute attribute)
        {
            if (((BasicUser) SecuritySystem.CurrentUser).IsAdministrator && attribute.ExcludeAdmin)
                return false;
            return true;
        }
    }
}