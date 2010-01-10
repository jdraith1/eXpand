using System.Security;
using DevExpress.Xpo;
using eXpand.ExpressApp.Security.Permissions;

namespace eXpand.ExpressApp.HideMember.Win.Security
{
    public enum HideMemberPermissionModifier
    {
        Allow,Deny
    }
    [NonPersistent]
    public class HideMemberPermission:PermissionBase
    {
        public HideMemberPermission(HideMemberPermissionModifier modifier)
        {
            Modifier = modifier;
        }

        public HideMemberPermission()
        {
        }

        public override IPermission Copy()
        {
            return new HideMemberPermission();
        }

        public HideMemberPermissionModifier Modifier { get; set; }

        public override string ToString()
        {
            return string.Format("{0}({1})", base.ToString(), Modifier);
        }

    }
}
