﻿using System;
using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using eXpand.Persistent.Base.PersistentMetaData;
using eXpand.Utils.Helpers;

namespace eXpand.ExpressApp.WorldCreator.PersistentTypesHelpers {
    public class CodeTemplateQuery {
        public static ICodeTemplate FindDefaultTemplate(TemplateType templateType, Session session, Type codeTemplateType, CodeDomProvider codeDomProvider){
            const ICodeTemplate template = null;
            var binaryOperator = new BinaryOperator(template.GetPropertyName(x => x.TemplateType),templateType);
            var isDefault = new BinaryOperator(template.GetPropertyName(x => x.IsDefault),true);
            var provider = new BinaryOperator(template.GetPropertyName(x => x.CodeDomProvider),codeDomProvider);
            return session.FindObject(PersistentCriteriaEvaluationBehavior.InTransaction,
                                      codeTemplateType, new GroupOperator(binaryOperator, isDefault,provider))
                   as ICodeTemplate;
        }
    }
}