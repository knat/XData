using System;
using System.Collections.Generic;
using XData.IO.Text;

namespace XData {
    public enum DiagCode {
        None = 0,
        //parsing
        Parsing = -1000,
        AliasSysIsReserved,
        DuplicateUriAlias,
        InvalidUriAlias,
        //object
        ObjectNotEqualTo,

        //types
        InvalidTypeName,
        TypeNotEqualToOrDeriveFrom,
        TypeIsAbstract,

        //simple types
        TypeRequiresAtomValue,
        TypeRequiresListValue,
        InvalidAtomTypeLiteral,

        //facets
        LengthNotEqualTo,
        LengthNotGreaterThanOrEqualTo,
        LengthNotLessThanOrEqualTo,
        PrecisionNotLessThanOrEqualTo,
        ScaleNotLessThanOrEqualTo,
        ValueNotGreaterThanOrEqualTo,
        ValueNotGreaterThan,
        ValueNotLessThanOrEqualTo,
        ValueNotLessThan,
        ValueNotInEnumeration,
        LiteralNotMatchWithPattern,

        //complex types
        AttributesNotAllowedForType,
        ChildrenNotAllowedForType,
        SimpleChildRequiredForType,
        ComplexChildrenRequiredForType,

        //attributes
        DuplicateAttributeName,
        RequiredAttributeNotFound,
        RedundantAttribute,
        AttributeIsNotNullable,

        //element
        ElementIsAbstract,
        ElementIsNotNullable,
        ComplexTypeValueRequiredForElement,
        SimpleValueRequiredForElement,
        InvalidElementNode,
        EntityElementIsNull,
        ElementNotEqualToOrSubstituteFor,

        //children
        RequiredChildNotMatched,
        ChildListCountNotGreaterThanOrEqualToMinOccurrence,
        RedundantElementNode,
        RequiredChildNotFound,
        RedundantChild,


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
                case DiagCode.InvalidUriAlias:
                    return "Invalid uri alias '{0}'.".InvFormat(_msgArgs);
                //object
                case DiagCode.ObjectNotEqualTo:
                    return "Object '{0}' not equal to '{1}'.".InvFormat(_msgArgs);
                //
                //types
                case DiagCode.InvalidTypeName:
                    return "Invalid type name '{0}'.".InvFormat(_msgArgs);
                case DiagCode.TypeNotEqualToOrDeriveFrom:
                    return "Type '{0}' not equal to or derive from '{1}'.".InvFormat(_msgArgs);
                case DiagCode.TypeIsAbstract:
                    return "Type '{0}' is abstract.".InvFormat(_msgArgs);
                //simple types
                case DiagCode.TypeRequiresAtomValue:
                    return "Type '{0}' requires atom value.".InvFormat(_msgArgs);
                case DiagCode.TypeRequiresListValue:
                    return "Type '{0}' requires list value.".InvFormat(_msgArgs);
                case DiagCode.InvalidAtomTypeLiteral:
                    return "Invalid atom type '{0}' literal '{1}'.".InvFormat(_msgArgs);
                //facets
                case DiagCode.LengthNotEqualTo:
                    return "Length '{0}' not equal to '{1}'.".InvFormat(_msgArgs);
                case DiagCode.LengthNotGreaterThanOrEqualTo:
                    return "Length '{0}' not greater than or equal to '{1}'.".InvFormat(_msgArgs);
                case DiagCode.LengthNotLessThanOrEqualTo:
                    return "Length '{0}' not less than or equal to '{1}'.".InvFormat(_msgArgs);
                case DiagCode.PrecisionNotLessThanOrEqualTo:
                    return "Precision '{0}' not less than or equal to '{1}'.".InvFormat(_msgArgs);
                case DiagCode.ScaleNotLessThanOrEqualTo:
                    return "Scale '{0}' not less than or equal to '{1}'.".InvFormat(_msgArgs);
                case DiagCode.ValueNotGreaterThanOrEqualTo:
                    return "Value '{0}' not greater than or equal to '{1}'.".InvFormat(_msgArgs);
                case DiagCode.ValueNotGreaterThan:
                    return "Value '{0}' not greater than '{1}'.".InvFormat(_msgArgs);
                case DiagCode.ValueNotLessThanOrEqualTo:
                    return "Value '{0}' not less than or equal to '{1}'.".InvFormat(_msgArgs);
                case DiagCode.ValueNotLessThan:
                    return "Value '{0}' not less than '{1}'.".InvFormat(_msgArgs);
                case DiagCode.ValueNotInEnumeration:
                    return "Value '{0}' not in enumeration '{1}'.".InvFormat(_msgArgs);
                case DiagCode.LiteralNotMatchWithPattern:
                    return "Literal '{0}' not match with pattern '{1}'.".InvFormat(_msgArgs);

                //complex types
                case DiagCode.AttributesNotAllowedForType:
                    return "Attributes not allowed for type '{0}'.".InvFormat(_msgArgs);
                case DiagCode.ChildrenNotAllowedForType:
                    return "Children not allowed for type '{0}'.".InvFormat(_msgArgs);
                case DiagCode.SimpleChildRequiredForType:
                    return "Simple child required for type '{0}'.".InvFormat(_msgArgs);
                case DiagCode.ComplexChildrenRequiredForType:
                    return "Complex children required for type '{0}'.".InvFormat(_msgArgs);

                //attributes
                case DiagCode.DuplicateAttributeName:
                    return "Duplicate attribute name '{0}'.".InvFormat(_msgArgs);
                case DiagCode.RequiredAttributeNotFound:
                    return "Required attribute '{0}' not found.".InvFormat(_msgArgs);
                case DiagCode.RedundantAttribute:
                    return "Redundant attribute '{0}'.".InvFormat(_msgArgs);
                case DiagCode.AttributeIsNotNullable:
                    return "Attribute '{0}' is not nullable.".InvFormat(_msgArgs);

                //elements
                case DiagCode.ElementIsAbstract:
                    return "Element '{0}' is abstract.".InvFormat(_msgArgs);
                case DiagCode.ElementIsNotNullable:
                    return "Element '{0}' is not nullable.".InvFormat(_msgArgs);
                case DiagCode.ComplexTypeValueRequiredForElement:
                    return "Complex type value for element '{0}'.".InvFormat(_msgArgs);
                case DiagCode.SimpleValueRequiredForElement:
                    return "Simple value required for element '{0}'.".InvFormat(_msgArgs);
                case DiagCode.InvalidElementNode:
                    return "Invalid element '{0}'. '{1}' or its substitutor expected.".InvFormat(_msgArgs);
                case DiagCode.EntityElementIsNull:
                    return "Entity element is null.";
                case DiagCode.ElementNotEqualToOrSubstituteFor:
                    return "Element '{0}' not equal to or substitute for '{1}'.".InvFormat(_msgArgs);

                //children
                case DiagCode.RequiredChildNotMatched:
                    return "Required child '{0}' not matched.".InvFormat(_msgArgs);
                case DiagCode.ChildListCountNotGreaterThanOrEqualToMinOccurrence:
                    return "Child list '{0}' count '{1}' not greater than or equal to min occurrence '{2}'.".InvFormat(_msgArgs);
                case DiagCode.RedundantElementNode:
                    return "Redundant element '{0}'.".InvFormat(_msgArgs);
                case DiagCode.RequiredChildNotFound:
                    return "Required child '{0}' not found.".InvFormat(_msgArgs);
                case DiagCode.RedundantChild:
                    return "Redundant child '{0}'.".InvFormat(_msgArgs);

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
        public readonly DiagSeverity Severity;
        public readonly int RawCode;
        public readonly string Message;
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
                var sb = EX.AcquireStringBuilder();
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


    public class DiagContext : List<Diag> {
        public DiagContext() { }
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
        public virtual void Reset() {
            Clear();
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
    }


}
