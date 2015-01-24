﻿using System;
using XData.IO.Text;

namespace XData.Compiler {
    public enum DiagCodeEx {
        None = 0,
        AliasSysIsReserved = -2000,
        UriSystemIsReserved,
        DuplicateUriAlias,
        InvalidUriAlias,
        DuplicateImportAlias,
        InvalidImportUri,
        DuplicateNamespaceMember,
        InvalidQualifiableNameAlias,
        AmbiguousNameReference,
        InvalidNameReference,
        InvalidTypeNameReference,
        //InvalidAttributeNameReference,
        InvalidElementNameReference,
        UInt64ValueRequired,
        ByteValueRequired,
        MaxValueMustEqualToOrBeGreaterThanMinValue,
        MaxValueMustBeGreaterThanZero,
        DuplicateMemberName,
        DuplicateAttributeName,
        InvalidIndicatorNamespaceName,
        InconsistentCSharpNamespaceName,
        CSNamespaceNameNotSpecifiedForNamespace,
        CircularReferenceDetected,
        SimpleTypeRequired,
        ComplexTypeRequired,
        BaseTypeIsSealed,
        CannotExtendOrRestrictSysComplexType,
        CannotRestrictSysSimpleAtomListType,
        CannotExtendSimpleChildWithComplexChildren,
        CannotExtendChildrenWithSimpleChild,
        CannotRestrictSimpleChildWithComplexChildren,
        CannotRestrictComplexChildrenWithSimpleChild,
        CannotRestrictNullSimpleChild,
        CannotRestrictSimpleTypeWithAttributesOrChildren,
        CannotRestrictComplexTypeWithValueRestrictions,
        IsNotEqualToOrDeriveFrom,
        CannotFindRestrictedAttribute,
        AttributeFullNameNotEqualToRestricted,
        AttributeIsOptionalButRestrictedIsRequired,
        AttributeIsNullableButRestrictedIsNotNullable,
        AttributeDeclarationNotEqualToRestricted,
        AttributeTypeNotEqualToOrDeriveFromRestricted,
        RequiredAttributeNotRestricting,
        AttributeIsNullableButSubstitutedIsNotNullable,
        AttributeTypeNotEqualToOrDeriveFromSubstituted,

        ValueRestrictionNotApplicable,
    }
    public struct DiagMsgEx {
        public DiagMsgEx(DiagCodeEx code) {
            Code = code;
            _msgArgs = null;
        }
        public DiagMsgEx(DiagCodeEx code, params string[] msgArgs) {
            Code = code;
            _msgArgs = msgArgs;
        }
        public readonly DiagCodeEx Code;
        private readonly string[] _msgArgs;
        public string GetMessage() {
            switch (Code) {
                case DiagCodeEx.AliasSysIsReserved:
                    return "Alias 'sys' is reserved.";
                case DiagCodeEx.UriSystemIsReserved:
                    return "Uri '" + InfoExtensions.SystemUri + "' is reserved.";
                case DiagCodeEx.DuplicateUriAlias:
                    return "Duplicate uri alias '{0}'.".InvFormat(_msgArgs);
                case DiagCodeEx.InvalidUriAlias:
                    return "Invalid uri alias '{0}'.".InvFormat(_msgArgs);
                case DiagCodeEx.DuplicateImportAlias:
                    return "Duplicate import alias '{0}'.".InvFormat(_msgArgs);
                case DiagCodeEx.DuplicateNamespaceMember:
                    return "Duplicate namespace member '{0}'.".InvFormat(_msgArgs);
                case DiagCodeEx.MaxValueMustEqualToOrBeGreaterThanMinValue:
                    return "Max value '{0}' must equal to or be greater than min value '{1}'.".InvFormat(_msgArgs);
                case DiagCodeEx.MaxValueMustBeGreaterThanZero:
                    return "Max value must be greater than zero.";
                case DiagCodeEx.UInt64ValueRequired:
                    return "UInt64 value required.";
                case DiagCodeEx.ByteValueRequired:
                    return "Byte value required.";
                case DiagCodeEx.DuplicateMemberName:
                    return "Duplicate member name '{0}'.".InvFormat(_msgArgs);
                case DiagCodeEx.DuplicateAttributeName:
                    return "Duplicate attribute name '{0}'.".InvFormat(_msgArgs);
                case DiagCodeEx.InvalidIndicatorNamespaceName:
                    return "Invalid indicator namespace name '{0}'.".InvFormat(_msgArgs);
                case DiagCodeEx.InconsistentCSharpNamespaceName:
                    return "Inconsistent C# namespace name '{0}' '{1}' and '{2}' '{3}'.".InvFormat(_msgArgs);
                case DiagCodeEx.CSNamespaceNameNotSpecifiedForNamespace:
                    return "C# namespace name is not specified for namespace '{0}'.".InvFormat(_msgArgs);
                case DiagCodeEx.InvalidImportUri:
                    return "Invalid import uri '{0}'.".InvFormat(_msgArgs);
                case DiagCodeEx.InvalidQualifiableNameAlias:
                    return "Invalid qualifiable name alias '{0}'.".InvFormat(_msgArgs);
                case DiagCodeEx.AmbiguousNameReference:
                    return "Ambiguous name reference '{0}'.".InvFormat(_msgArgs);
                case DiagCodeEx.InvalidNameReference:
                    return "Invalid name reference '{0}'.".InvFormat(_msgArgs);
                case DiagCodeEx.InvalidTypeNameReference:
                    return "Invalid type name reference '{0}'.".InvFormat(_msgArgs);
                //case DiagCodeEx.InvalidAttributeNameReference:
                //    return "Invalid attribute name reference '{0}'.".InvFormat(_msgArgs);
                case DiagCodeEx.InvalidElementNameReference:
                    return "Invalid element name reference '{0}'.".InvFormat(_msgArgs);
                case DiagCodeEx.CircularReferenceDetected:
                    return "Circular reference detected.";
                case DiagCodeEx.SimpleTypeRequired:
                    return "Simple type required.";
                case DiagCodeEx.ComplexTypeRequired:
                    return "Complex type required.";
                case DiagCodeEx.BaseTypeIsSealed:
                    return "Base type '{0}' is sealed.".InvFormat(_msgArgs);
                case DiagCodeEx.CannotExtendOrRestrictSysComplexType:
                    return "Cannot extend or restrict 'sys:ComplexType'.";
                case DiagCodeEx.CannotRestrictSysSimpleAtomListType:
                    return "Cannot restrict 'sys:SimpleType', 'sys:AtomType' or 'sys:ListType'.";
                case DiagCodeEx.CannotExtendSimpleChildWithComplexChildren:
                    return "Cannot extend simple child with complex children.";
                case DiagCodeEx.CannotExtendChildrenWithSimpleChild:
                    return "Cannot extend children with simple child.";
                case DiagCodeEx.CannotRestrictSimpleChildWithComplexChildren:
                    return "Cannot restrict simple child with complex children.";
                case DiagCodeEx.CannotRestrictComplexChildrenWithSimpleChild:
                    return "Cannot restrict complex children with simple child.";
                case DiagCodeEx.CannotRestrictNullSimpleChild:
                    return "Cannot restrict null simple child.";
                case DiagCodeEx.CannotRestrictSimpleTypeWithAttributesOrChildren:
                    return "Cannot restrict simple type with attributes or children.";
                case DiagCodeEx.CannotRestrictComplexTypeWithValueRestrictions:
                    return "Cannot restrict complex type with value restrictions.";
                case DiagCodeEx.IsNotEqualToOrDeriveFrom:
                    return "'{0} is not equal to or derive from '{1}'.";
                case DiagCodeEx.CannotFindRestrictedAttribute:
                    return "Cannot find the restricted attribute with member name '{0}'.".InvFormat(_msgArgs);
                case DiagCodeEx.AttributeFullNameNotEqualToRestricted:
                    return "Attribute full name '{0}' not equal to the restricted attribute full name '{1}'.".InvFormat(_msgArgs);
                case DiagCodeEx.AttributeIsOptionalButRestrictedIsRequired:
                    return "Attribute is optional but the restricted attribute is required.";
                case DiagCodeEx.AttributeIsNullableButRestrictedIsNotNullable:
                    return "Attribute is nullable but the restricted attribute is not nullable.";
                case DiagCodeEx.AttributeDeclarationNotEqualToRestricted:
                    return "Attribute declaration '{0}' not equal to the restricted attribute '{1}'.";
                case DiagCodeEx.AttributeTypeNotEqualToOrDeriveFromRestricted:
                    return "Attribute type '{0}' not equal to or derive from the restricted attribute type '{1}'.".InvFormat(_msgArgs);
                case DiagCodeEx.RequiredAttributeNotRestricting:
                    return "Required attribute with member name '{0}' not restricting.".InvFormat(_msgArgs);
                case DiagCodeEx.AttributeIsNullableButSubstitutedIsNotNullable:
                    return "Attribute is nullable but the substituted attribute is not nullable.";
                case DiagCodeEx.AttributeTypeNotEqualToOrDeriveFromSubstituted:
                    return "Attribute type '{0}' not equal to or derive from the substituted attribute type '{1}'.".InvFormat(_msgArgs);


                case DiagCodeEx.ValueRestrictionNotApplicable:
                    return "Value restriction not applicable.";



                default:
                    throw new InvalidOperationException("Invalid code: " + Code.ToString());
            }
        }
    }

    public sealed class ContextEx : Context {
        [ThreadStatic]
        public static ContextEx Current;
        public sealed class ContextException : Exception { }
        private static readonly ContextException _contextException = new ContextException();
        public static void ErrorDiag(DiagMsgEx diagMsg, TextSpan textSpan) {
            Current.AddDiag(DiagSeverity.Error, (int)diagMsg.Code, diagMsg.GetMessage(), textSpan, null);
        }
        public static void ErrorDiagAndThrow(DiagMsgEx diagMsg, TextSpan textSpan) {
            ErrorDiag(diagMsg, textSpan);
            throw _contextException;
        }
        public static void ThrowIfHasErrors() {
            if (Current.HasErrorDiags) {
                throw _contextException;
            }
        }
        public static void WarningDiag(DiagMsgEx diagMsg, TextSpan textSpan) {
            Current.AddDiag(DiagSeverity.Warning, (int)diagMsg.Code, diagMsg.GetMessage(), textSpan, null);
        }

    }
}
