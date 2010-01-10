using System.Security;
using DevExpress.Xpo;
using eXpand.ExpressApp.ModelArtifactState.Security.Permissions;

namespace eXpand.ExpressApp.UpdateObject.Security
{

    [NonPersistent]
    public class MassiveUpdateObjectPermission : ActionStateRulePermission
    {
        public override IPermission Copy()
        {
            return new MassiveUpdateObjectPermission();
        }
        public override string ToString()
        {
            return string.Format("{1}: {0}", Name, GetType().Name);
        }
    }
}
