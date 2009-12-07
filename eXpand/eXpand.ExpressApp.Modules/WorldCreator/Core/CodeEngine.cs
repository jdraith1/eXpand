﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using eXpand.Persistent.Base.PersistentMetaData;
using eXpand.Persistent.Base.PersistentMetaData.PersistentAttributeInfos;

namespace eXpand.ExpressApp.WorldCreator.Core {
    public static class CodeEngine {
        public static string GenerateCode(ICodeTemplateInfo codeTemplateInfo) {
            string code = codeTemplateInfo.TemplateInfo.TemplateCode+"";
            code = code.Replace("$ASSEMBLYNAME$", codeTemplateInfo.PersistentAssemblyInfo.Name);
            code = code.Replace("$CLASSNAME$", codeTemplateInfo.TemplateInfo.Name);
            return code;
        }

        public static string GenerateCode(IPersistentMemberInfo persistentMemberInfo) {
            string code = persistentMemberInfo.CodeTemplateInfo.TemplateInfo.TemplateCode;
            if (code != null) {
                code = code.Replace("$TYPEATTRIBUTES$", GetAttributesCode(persistentMemberInfo));
                code = code.Replace("$PROPERTYNAME$", persistentMemberInfo.Name);
                code = code.Replace("$PROPERTYTYPE$", GetPropertyType(persistentMemberInfo));
            }
            return code;
        }

        public static string GenerateCode(IPersistentClassInfo persistentClassInfo) {

            string code = persistentClassInfo.CodeTemplateInfo.TemplateInfo.TemplateCode;
            string attributesCode = GetAttributesCode(persistentClassInfo);
            if (code != null) {
                code = code.Replace("$ASSEMBLYNAME$", persistentClassInfo.PersistentAssemblyInfo.Name);
                code = code.Replace("$TYPEATTRIBUTES$", attributesCode);
                code = code.Replace("$CLASSNAME$", persistentClassInfo.Name);
                code = code.Replace("$BASECLASSNAME$", getBaseType(persistentClassInfo) + getInterfacesCode(persistentClassInfo));
                code = GetAllMembersCode(persistentClassInfo, code);
                return code;
            }
            return null;
        }

        static string GetAllMembersCode(IPersistentClassInfo persistentClassInfo, string code) {
            string allMemberGeneratedCode = GetMembersCode(persistentClassInfo.OwnMembers);
            int anchorsFound=0;
            for (int i = code.Length-1; i > -1; i--) {
                if (code[i] == '}' && anchorsFound < 2)
                    anchorsFound++;
                if (anchorsFound==2) {
                    code = code.Substring(0, i) + allMemberGeneratedCode + code.Substring(i);
                    break;
                }
            }
            return code;
        }

        static string getInterfacesCode(IPersistentClassInfo persistentClassInfo) {
            return persistentClassInfo.Interfaces.Aggregate<IInterfaceInfo, string>(null, (current, interfaceInfo) => current + ("," + interfaceInfo.Type.FullName));
        }


        static string GetAttributesCode(IPersistentTypeInfo persistentClassInfo) {
            return (persistentClassInfo.TypeAttributes.Aggregate<IPersistentAttributeInfo, string>(null, (current, persistentAttributeInfo) => current + GenerateCode(persistentAttributeInfo) + Environment.NewLine)+"").TrimEnd(Environment.NewLine.ToCharArray());
        }


        static string getBaseType(IPersistentClassInfo persistentClassInfo) {
            return persistentClassInfo.BaseTypeFullName??persistentClassInfo.GetDefaultBaseClass().FullName;
        }

        static string GetMembersCode(IEnumerable<IPersistentMemberInfo> persistentMemberInfos) {
            Func<IPersistentMemberInfo, string> codeSelector = persistentMemberInfo => GenerateCode(persistentMemberInfo);
            Func<string, string, string> aggrecator = (current, code) => current + (code + Environment.NewLine);
            return persistentMemberInfos.Select(codeSelector).Aggregate(null, aggrecator);
        }

        static string GetPropertyType(IPersistentMemberInfo persistentMemberInfo) {
            if (persistentMemberInfo is IPersistentCoreTypeMemberInfo)
                return Type.GetType("System."+((IPersistentCoreTypeMemberInfo)persistentMemberInfo).DataType).FullName;
            if (persistentMemberInfo is IPersistentReferenceMemberInfo)
                return ((IPersistentReferenceMemberInfo) persistentMemberInfo).ReferenceTypeFullName;
            if (persistentMemberInfo is IPersistentCollectionMemberInfo)
                return ((IPersistentCollectionMemberInfo)persistentMemberInfo).CollectionTypeFullName;
            throw new NotImplementedException(persistentMemberInfo.GetType().FullName);
        }

        static string GenerateCode(IPersistentAttributeInfo persistentAttributeInfo) {
            AttributeInfo attributeInfo = persistentAttributeInfo.Create();
            var attribute = (Attribute)Activator.CreateInstance(attributeInfo.Constructor.DeclaringType, attributeInfo.InitializedArgumentValues);
            string args = attributeInfo.InitializedArgumentValues.Length>0
                              ? attributeInfo.InitializedArgumentValues.Select(
                                    argumentValue =>
                                    argumentValue is string ? @"""" + argumentValue + @"""" : argumentValue).Aggregate
                                    <object, string>(null, (current, o) => current + (o + ",")).TrimEnd(',')
                              : null;
            return string.Format("[{0}({1})]", attribute.GetType().FullName, args);
        }

        public static string GenerateCode(IPersistentAssemblyInfo persistentAssemblyInfo) {
            string generateCode = persistentAssemblyInfo.PersistentClassInfos.
                Aggregate<IPersistentClassInfo, string>(null, (current, persistentClassInfo) => current + (GenerateCode(persistentClassInfo) + Environment.NewLine));
            return groupUsings(generateCode,persistentAssemblyInfo.CodeDomProvider);
        }

        static string groupUsings(string generateCode,CodeDomProvider codeDomProvider) {
            var regex = new Regex(codeDomProvider == CodeDomProvider.CSharp ? "(using [^;]*;\r\n)*" : "(Imports [^\r\n]*\r\n)*");
            if (generateCode != null) {
                string s =
                    regex.Matches(generateCode).Cast<Match>().Where(match1 => !string.IsNullOrEmpty(match1.Value)).Select(match => match.Value).Distinct().Aggregate<string, string>(null, (current, match) => current + match);
                return s +Environment.NewLine+ regex.Replace(generateCode, "");
            }
            return null;
        }

    }
}