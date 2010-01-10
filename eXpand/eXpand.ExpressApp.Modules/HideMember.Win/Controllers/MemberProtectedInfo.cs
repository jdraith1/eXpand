namespace eXpand.ExpressApp.HideMember.Win.Controllers
{
    public class MemberProtectedInfo
    {
        public MemberProtectedInfo(bool isClassProtected, bool isProtected)
        {
            IsClassProtected = isClassProtected;
            IsProtected = isProtected;
        }

        public bool IsClassProtected { get; set; }

        public bool IsProtected { get; set; }
    }
}