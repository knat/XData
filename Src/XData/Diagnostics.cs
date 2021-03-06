﻿using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using XData.IO.Text;

namespace XData {
    public enum DiagCode {
        None = 0,
        //parsing
        Parsing = -1000,
        AliasSysIsReserved,
        DuplicateUriAlias,
        InvalidUriReference,

        //object
        InvalidObject,

        //type
        InvalidTypeReference,
        TypeNotEqualToOrDeriveFrom,
        TypeIsAbstract,

        //simple type
        AtomValueRequiredForType,
        ListValueRequiredForType,
        InvalidAtomTypeLiteral,

        //facet
        LengthNotEqualTo,
        LengthNotGreaterThanOrEqualToMinLength,
        LengthNotLessThanOrEqualToMaxLength,
        PrecisionNotLessThanOrEqualTo,
        ScaleNotLessThanOrEqualTo,
        ValueNotGreaterThanOrEqualToMinValue,
        ValueNotGreaterThanMinValue,
        ValueNotLessThanOrEqualToMaxValue,
        ValueNotLessThanMaxValue,
        ValueNotInEnumeration,
        LiteralNotMatchWithPattern,

        //complex type
        AttributesNotAllowedForType,
        ChildrenNotAllowedForType,
        SimpleChildRequiredForType,
        ComplexChildrenRequiredForType,

        //attribute
        DuplicateAttributeName,
        RequiredAttributeNotMatched,
        RequiredAttributeNotSet,
        RedundantAttribute,
        AttributeValueNotSet,

        //element
        ElementIsAbstract,
        ElementValueNotSet,
        ComplexValueRequiredForElement,
        SimpleValueRequiredForElement,
        InvalidElementNode,
        GlobalElementNotSet,
        ElementNotEqualToOrSubstituteFor,

        //child
        RequiredChildNotMatched,
        RequiredChildNotSet,
        ChoiceNotSet,
        RedundantChild,
        ChildListCountNotGreaterThanOrEqualToMinOccurrence,


    }
    public struct DiagMsg {
        public DiagMsg(DiagCode code) {
            Code = code;
            _msgArgs = null;
        }
        public DiagMsg(DiagCode code, params string[] msgArgs) {
            Code = code;
            _msgArgs = msgArgs;
        }
        public readonly DiagCode Code;
        private readonly string[] _msgArgs;
        public string GetMessage() {
            switch (Code) {
                //parsing
                case DiagCode.AliasSysIsReserved:
                    return "Alias 'sys' is reserved.";
                case DiagCode.DuplicateUriAlias:
                    return "Duplicate uri alias '{0}'.".InvFormat(_msgArgs);
                case DiagCode.InvalidUriReference:
                    return "Invalid uri reference '{0}'.".InvFormat(_msgArgs);

                //object
                case DiagCode.InvalidObject:
                    return "Invalid object '{0}'. '{1}' expected.".InvFormat(_msgArgs);

                //type
                case DiagCode.InvalidTypeReference:
                    return "Invalid type reference '{0}'.".InvFormat(_msgArgs);
                case DiagCode.TypeNotEqualToOrDeriveFrom:
                    return "Type '{0}' not equal to or derive from '{1}'.".InvFormat(_msgArgs);
                case DiagCode.TypeIsAbstract:
                    return "Type '{0}' is abstract.".InvFormat(_msgArgs);

                //simple type
                case DiagCode.AtomValueRequiredForType:
                    return "Atom value required for type '{0}'.".InvFormat(_msgArgs);
                case DiagCode.ListValueRequiredForType:
                    return "List value required for type '{0}'.".InvFormat(_msgArgs);
                case DiagCode.InvalidAtomTypeLiteral:
                    return "Invalid atom type '{0}' literal '{1}'.".InvFormat(_msgArgs);

                //facet
                case DiagCode.LengthNotEqualTo:
                    return "Length '{0}' not equal to '{1}'.".InvFormat(_msgArgs);
                case DiagCode.LengthNotGreaterThanOrEqualToMinLength:
                    return "Length '{0}' not greater than or equal to min length '{1}'.".InvFormat(_msgArgs);
                case DiagCode.LengthNotLessThanOrEqualToMaxLength:
                    return "Length '{0}' not less than or equal to max length '{1}'.".InvFormat(_msgArgs);
                case DiagCode.PrecisionNotLessThanOrEqualTo:
                    return "Precision '{0}' not less than or equal to '{1}'.".InvFormat(_msgArgs);
                case DiagCode.ScaleNotLessThanOrEqualTo:
                    return "Scale '{0}' not less than or equal to '{1}'.".InvFormat(_msgArgs);
                case DiagCode.ValueNotGreaterThanOrEqualToMinValue:
                    return "Value '{0}' not greater than or equal to min value '{1}'.".InvFormat(_msgArgs);
                case DiagCode.ValueNotGreaterThanMinValue:
                    return "Value '{0}' not greater than min value '{1}'.".InvFormat(_msgArgs);
                case DiagCode.ValueNotLessThanOrEqualToMaxValue:
                    return "Value '{0}' not less than or equal to max value '{1}'.".InvFormat(_msgArgs);
                case DiagCode.ValueNotLessThanMaxValue:
                    return "Value '{0}' not less than max value '{1}'.".InvFormat(_msgArgs);
                case DiagCode.ValueNotInEnumeration:
                    return "Value '{0}' not in enumeration '{1}'.".InvFormat(_msgArgs);
                case DiagCode.LiteralNotMatchWithPattern:
                    return "Literal '{0}' not match with pattern '{1}'.".InvFormat(_msgArgs);

                //complex type
                case DiagCode.AttributesNotAllowedForType:
                    return "Attributes not allowed for type '{0}'.".InvFormat(_msgArgs);
                case DiagCode.ChildrenNotAllowedForType:
                    return "Children not allowed for type '{0}'.".InvFormat(_msgArgs);
                case DiagCode.SimpleChildRequiredForType:
                    return "Simple child required for type '{0}'.".InvFormat(_msgArgs);
                case DiagCode.ComplexChildrenRequiredForType:
                    return "Complex children required for type '{0}'.".InvFormat(_msgArgs);

                //attribute
                case DiagCode.DuplicateAttributeName:
                    return "Duplicate attribute name '{0}'.".InvFormat(_msgArgs);
                case DiagCode.RequiredAttributeNotMatched:
                    return "Required attribute '{0}' not matched.".InvFormat(_msgArgs);
                case DiagCode.RequiredAttributeNotSet:
                    return "Required attribute '{0}' not set.".InvFormat(_msgArgs);
                case DiagCode.RedundantAttribute:
                    return "Redundant attribute '{0}'.".InvFormat(_msgArgs);
                case DiagCode.AttributeValueNotSet:
                    return "Attribute '{0}' value not set.".InvFormat(_msgArgs);

                //element
                case DiagCode.ElementIsAbstract:
                    return "Element '{0}' is abstract.".InvFormat(_msgArgs);
                case DiagCode.ElementValueNotSet:
                    return "Element '{0}' value not set.".InvFormat(_msgArgs);
                case DiagCode.ComplexValueRequiredForElement:
                    return "Complex value required for element '{0}'.".InvFormat(_msgArgs);
                case DiagCode.SimpleValueRequiredForElement:
                    return "Simple value required for element '{0}'.".InvFormat(_msgArgs);
                case DiagCode.InvalidElementNode:
                    return "Invalid element '{0}'. '{1}' or its substitutor expected.".InvFormat(_msgArgs);
                case DiagCode.GlobalElementNotSet:
                    return "Global element not set.";
                case DiagCode.ElementNotEqualToOrSubstituteFor:
                    return "Element '{0}' not equal to or substitute for '{1}'.".InvFormat(_msgArgs);

                //child
                case DiagCode.RequiredChildNotMatched:
                    return "Required child '{0}' not matched.".InvFormat(_msgArgs);
                case DiagCode.RequiredChildNotSet:
                    return "Required child '{0}' not set.".InvFormat(_msgArgs);
                case DiagCode.ChoiceNotSet:
                    return "Choice '{0}' not set.".InvFormat(_msgArgs);
                case DiagCode.RedundantChild:
                    return "Redundant child '{0}'.".InvFormat(_msgArgs);
                case DiagCode.ChildListCountNotGreaterThanOrEqualToMinOccurrence:
                    return "Child list '{0}' count '{1}' not greater than or equal to min occurrence '{2}'.".InvFormat(_msgArgs);

                default:
                    throw new InvalidOperationException("Invalid code: " + Code.ToString());
            }
        }
    }
    public enum DiagSeverity : byte {
        None = 0,
        Error = 1,
        Warning = 2,
        Info = 3
    }
    [DataContract(Namespace = Extensions.SystemUri)]
    public struct Diag {
        public Diag(DiagSeverity severity, int rawCode, string message, TextSpan textSpan, XObject obj) {
            Severity = severity;
            RawCode = rawCode;
            Message = message;
            TextSpan = textSpan;
            Object = obj;
            if (!textSpan.IsValid && obj != null) {
                TextSpan = obj.TextSpan;
            }
        }
        public Diag(DiagSeverity severity, DiagMsg diagMsg, TextSpan textSpan, XObject obj)
            : this(severity, (int)diagMsg.Code, diagMsg.GetMessage(), textSpan, obj) {
        }
        [DataMember]
        public readonly DiagSeverity Severity;
        [DataMember]
        public readonly int RawCode;
        [DataMember]
        public readonly string Message;
        [DataMember]
        public readonly TextSpan TextSpan;//opt
        public readonly XObject Object;//opt
        public bool IsError {
            get {
                return Severity == DiagSeverity.Error;
            }
        }
        public bool IsWarning {
            get {
                return Severity == DiagSeverity.Warning;
            }
        }
        public bool IsInfo {
            get {
                return Severity == DiagSeverity.Info;
            }
        }
        public DiagCode Code {
            get {
                return (DiagCode)RawCode;
            }
        }
        public bool HasTextSpan {
            get {
                return TextSpan.IsValid;
            }
        }
        public bool HasObject {
            get {
                return Object != null;
            }
        }
        public bool IsValid {
            get {
                return Severity != DiagSeverity.None;
            }
        }
        public override string ToString() {
            if (IsValid) {
                var sb = Extensions.AcquireStringBuilder();
                sb.Append(Severity.ToString());
                sb.Append(' ');
                sb.Append(RawCode.ToInvString());
                sb.Append(": ");
                sb.Append(Message);
                if (HasTextSpan) {
                    sb.Append("\r\n    ");
                    sb.Append(TextSpan.ToString());
                }
                return sb.ToStringAndRelease();
            }
            return null;
        }
    }
    [CollectionDataContract(Namespace = Extensions.SystemUri)]
    public class DiagContext : List<Diag> {
        public void AddDiag(DiagSeverity severity, int rawCode, string message, TextSpan textSpan, XObject obj) {
            Add(new Diag(severity, rawCode, message, textSpan, obj));
        }
        public void AddDiag(DiagSeverity severity, DiagMsg diagMsg, TextSpan textSpan, XObject obj) {
            Add(new Diag(severity, diagMsg, textSpan, obj));
        }
        public void AddErrorDiag(DiagMsg diagMsg, TextSpan textSpan) {
            Add(new Diag(DiagSeverity.Error, diagMsg, textSpan, null));
        }
        public void AddErrorDiag(DiagMsg diagMsg, XObject obj) {
            Add(new Diag(DiagSeverity.Error, diagMsg, default(TextSpan), obj));
        }
        public bool HasDiags {
            get {
                return Count > 0;
            }
        }
        public bool HasErrorDiags {
            get {
                return HasErrorDiagsCore(0);
            }
        }
        private bool HasErrorDiagsCore(int index) {
            var count = Count;
            for (; index < count; ++index) {
                if (this[index].IsError) {
                    return true;
                }
            }
            return false;
        }
        public struct DiagMarker {
            internal DiagMarker(DiagContext context) {
                Context = context;
                Index = context.Count;
            }
            public readonly DiagContext Context;
            public readonly int Index;
            public bool HasErrors {
                get {
                    return Context.HasErrorDiagsCore(Index);
                }
            }
            public void Restore() {
                Context.RemoveRange(Index, Context.Count - Index);
            }
        }
        public DiagMarker MarkDiags() {
            return new DiagMarker(this);
        }

        private struct ValidationResult {
            public ValidationResult(XObject obj, bool result) {
                Object = obj;
                Result = result;
            }
            public readonly XObject Object;
            public readonly bool Result;
        }
        private List<ValidationResult> _validationResultList;
        internal bool? GetValidationResult(XObject obj) {
            if (_validationResultList.CountOrZero() > 0) {
                foreach (var i in _validationResultList) {
                    if (i.Object == obj) {
                        return i.Result;
                    }
                }
            }
            return null;
        }
        internal void SetValidationResult(XObject obj, bool result) {
            Extensions.CreateAndAdd(ref _validationResultList, new ValidationResult(obj, result));
        }
        public virtual void Reset() {
            Clear();
            if (_validationResultList != null) {
                _validationResultList.Clear();
            }
        }

    }


}
