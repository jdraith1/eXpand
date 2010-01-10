using System.ComponentModel;
using DevExpress.ExpressApp;
using DevExpress.Utils;
using EditorBrowsableState=System.ComponentModel.EditorBrowsableState;

namespace eXpand.ExpressApp.HideMember
{
    [Description("Includes Property Editors and Controllers to enable Member,Record Hidding"), ToolboxTabName("eXpressApp"), EditorBrowsable(EditorBrowsableState.Always), Browsable(true), ToolboxItem(true)]
    public sealed partial class HideMemberModuleBase : ModuleBase
    {
        
        public HideMemberModuleBase()
        {
            InitializeComponent();
        }

//        public override void CustomizeXPDictionary(XPDictionary xpDictionary)
//        {
//            base.CustomizeXPDictionary(xpDictionary);
//            
//            var xpClassInfo = xpDictionary.CreateClass("HideMemberModuleInfo");
//            xpClassInfo.CreateMember("ReferenceType", typeof(string), new Attribute[] { new SizeAttribute(255) });
//            xpClassInfo.CreateMember("PropertyName", typeof(string), new Attribute[] { new SizeAttribute(255) });
//            xpClassInfo.CreateMember("IsHidden", typeof(bool),
//                                     new Attribute[] { new IndexedAttribute(new[] { "ReferenceType", "PropertyName" }) });
//
//            var session = Application.ObjectSpaceProvider.CreateUpdatingSession();
//            
//            foreach (XPClassInfo classInfo in xpDictionary.Classes)
//            {
//                foreach (XPMemberInfo memberInfo in classInfo.PersistentProperties)
//                {
//                    
//                    var findObject = session.FindObject(xpClassInfo,
//                                               new GroupOperator(new BinaryOperator("ReferenceType", classInfo.FullName),
//                                                                 new BinaryOperator("PropertyName", memberInfo.Name)));
//                    if (findObject== null)
//                    {
//                        var newObject = (XPBaseObject)xpClassInfo.CreateNewObject(session);
//                        newObject.SetMemberValue("ReferenceType",classInfo.FullName);
//                        newObject.SetMemberValue("PropertyName",memberInfo.Name);
//                    }
//                }
//            }
//        }
    }
}