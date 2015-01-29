using System;
using XData.IO.Text;

namespace XData.Compiler {
    internal enum DiagCodeEx {
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
        ValueRestrictionNotApplicable,

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
        RequiredAttributeNotRestricting,

        TypeNotEqualToOrDeriveFromRestricted,
        TypeNotEqualToOrDeriveFromSubstituted,

        DeletionNotAllowedInExtension,
        CannotDeleteAttributeBecauseItIsNotOptional,
        SubstitutedElementIsSealed,
        ElementIsNullableButSubstitutedIsNotNullable,
        MaxOccurrenceCannotBeZeroInExtension,
        ChildKindNotEqualToRestricted,
        MinOccurrenceNotEqualToOrGreaterThanRestricted,
        MaxOccurrenceNotEqualToOrLessThanRestricted,
        ElementNameNotEqualToRestricted,
        ElementIsNullableButRestrictedIsNotNullable,
        ElementNotEqualToOrSubstituteRestricted,
        CannotFindRestrictedChild,
        CannotDeleteChildBecauseItIsNotOptional,
        AmbiguousElementFullName,

        //
        //

    }
    internal struct DiagMsgEx {
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
                case DiagCodeEx.ValueRestrictionNotApplicable:
                    return "Value restriction not applicable.";


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
                    return "Cannot find the restricted attribute '{0}'.".InvFormat(_msgArgs);
                case DiagCodeEx.AttributeFullNameNotEqualToRestricted:
                    return "Attribute full name '{0}' not equal to the restricted attribute full name '{1}'.".InvFormat(_msgArgs);
                case DiagCodeEx.AttributeIsOptionalButRestrictedIsRequired:
                    return "Attribute is optional but the restricted is required.";
                case DiagCodeEx.AttributeIsNullableButRestrictedIsNotNullable:
                    return "Attribute is nullable but the restricted is not nullable.";
                case DiagCodeEx.AttributeDeclarationNotEqualToRestricted:
                    return "Attribute declaration '{0}' not equal to the restricted attribute '{1}'.";
                case DiagCodeEx.TypeNotEqualToOrDeriveFromRestricted:
                    return "Type '{0}' not equal to or derive from the restricted '{1}'.".InvFormat(_msgArgs);
                case DiagCodeEx.TypeNotEqualToOrDeriveFromSubstituted:
                    return "Type '{0}' not equal to or derive from the substituted '{1}'.".InvFormat(_msgArgs);


                case DiagCodeEx.RequiredAttributeNotRestricting:
                    return "Required attribute with member name '{0}' not restricting.".InvFormat(_msgArgs);
                case DiagCodeEx.DeletionNotAllowedInExtension:
                    return "Deletion not allowed in extension.";
                case DiagCodeEx.CannotDeleteAttributeBecauseItIsNotOptional:
                    return "Cannot delete attribute '{0}' because it is not optional.".InvFormat(_msgArgs);
                case DiagCodeEx.SubstitutedElementIsSealed:
                    return "Substituted element is sealed.";
                case DiagCodeEx.ElementIsNullableButSubstitutedIsNotNullable:
                    return "Element is nullable but the substituted element is not nullable.";
                case DiagCodeEx.MaxOccurrenceCannotBeZeroInExtension:
                    return "Max occurrence cannot be zero in extension.";
                case DiagCodeEx.ChildKindNotEqualToRestricted:
                    return "Child kind '{0}' not equal to the restricted '{1}'.".InvFormat(_msgArgs);
                case DiagCodeEx.MinOccurrenceNotEqualToOrGreaterThanRestricted:
                    return "Min occurrence '{0}' not equal to or greater than the restricted '{1}'.".InvFormat(_msgArgs);
                case DiagCodeEx.MaxOccurrenceNotEqualToOrLessThanRestricted:
                    return "Max occurrence '{0}' not equal to or less than the restricted '{1}'.".InvFormat(_msgArgs);
                case DiagCodeEx.ElementNameNotEqualToRestricted:
                    return "Element name '{0}' not equal to the restricted '{1}'.".InvFormat(_msgArgs);
                case DiagCodeEx.ElementIsNullableButRestrictedIsNotNullable:
                    return "Element iss nullable but the restricted is not nullable.";
                case DiagCodeEx.ElementNotEqualToOrSubstituteRestricted:
                    return "Element '{0}' not equal to or substitute the restricted '{1}'.".InvFormat(_msgArgs);
                case DiagCodeEx.CannotFindRestrictedChild:
                    return "Cannot find restricted child '{0}'.".InvFormat(_msgArgs);
                case DiagCodeEx.CannotDeleteChildBecauseItIsNotOptional:
                    return "Cannot delete child '{0}' because it is not optional.".InvFormat(_msgArgs);
                case DiagCodeEx.AmbiguousElementFullName:
                    return "Ambiguous element full name '{0}'.".InvFormat(_msgArgs);


                default:
                    throw new InvalidOperationException("Invalid code: " + Code.ToString());
            }
        }
    }

    internal sealed class DiagContextEx : DiagContext {
        [ThreadStatic]
        public static DiagContextEx Current;
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
