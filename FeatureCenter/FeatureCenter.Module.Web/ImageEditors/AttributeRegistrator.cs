﻿using System;
using System.Collections.Generic;
using DevExpress.ExpressApp.DC;
using Xpand.ExpressApp.Attributes;

namespace FeatureCenter.Module.Web.ImageEditors {
    public class AttributeRegistrator : Xpand.ExpressApp.Core.AttributeRegistrator {
        public override IEnumerable<Attribute> GetAttributes(ITypeInfo typesInfo) {
            if (Object.Equals(typesInfo.Type, typeof (PictureObject))) {
                var xpandNavigationItemAttribute = new XpandNavigationItemAttribute("ImageEditors/Thumbnails", "PictureObject_ListView");
                yield return xpandNavigationItemAttribute;
                yield return new WhatsNewAttribute(new DateTime(2011, 2, 5), xpandNavigationItemAttribute);
            }
            else if (Object.Equals(typesInfo.Type, typeof (PictureMasterObject))) {
                var xpandNavigationItemAttribute = new XpandNavigationItemAttribute("ImageEditors/NCarousel", "PictureMasterObject_DetailView",
                                                                                    "Title='masterobject'");
                yield return xpandNavigationItemAttribute;
                yield return new WhatsNewAttribute(new DateTime(2011, 2, 4), xpandNavigationItemAttribute);
            }

        }
    }
}
