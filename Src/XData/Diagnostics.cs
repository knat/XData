using System;
using System.Collections.Generic;
using XData.IO.Text;

namespace XData {
    public enum DiagCode {
        None = 0,
        Parsing = -1000,
        AliasSysIsReserved,
        //DuplicateDefaultUri,
        DuplicateUriAlias,
        InvalidUriAlias,
        ElementIsAbstract,
        ElementIsNotNullable,
        ElementRequiresComplexTypeValue,
        ElementRequiresSimpleTypeValue,
        InvalidTypeName,
        TypeDoesNotEqualToOrDeriveFrom,
        TypeIsAbstract,
        TypeProhibitsAttributes,
        TypeRequiresSimpleChild,
        TypeRequiresComplexChildren,
        TypeProhibitsChildren,
        RequiredChildMemberIsNotMatched,
        ChildListCountIsNotGreaterThanOrEqualToMinOccurs,



        //DuplicateAttributeFullName,


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
                case DiagCode.AliasSysIsReserved:
                    return "Alias 'sys' is reserved.";
                //case DiagCode.DuplicateDefaultUri:
                //    return "Duplicate default uri.";
                case DiagCode.DuplicateUriAlias:
                    return "Duplicate uri alias '{0}'.".InvFormat(_msgArgs);
                case DiagCode.InvalidUriAlias:
                    return "Invalid uri alias '{0}'.".InvFormat(_msgArgs);
                case DiagCode.ElementIsAbstract:
                    return "Element '{0}' is abstract.".InvFormat(_msgArgs);
                case DiagCode.ElementIsNotNullable:
                    return "Element '{0}' is not nullable.".InvFormat(_msgArgs);
                case DiagCode.ElementRequiresComplexTypeValue:
                    return "Element '{0}' requires complex type value.".InvFormat(_msgArgs);
                case DiagCode.ElementRequiresSimpleTypeValue:
                    return "Element '{0}' requires simple type value.".InvFormat(_msgArgs);
                case DiagCode.InvalidTypeName:
                    return "Invalid type name '{0}'.".InvFormat(_msgArgs);
                case DiagCode.TypeDoesNotEqualToOrDeriveFrom:
                    return "Type '{0}' does not equal to or derive from '{1}'.".InvFormat(_msgArgs);
                case DiagCode.TypeIsAbstract:
                    return "Type '{0}' is abstract.".InvFormat(_msgArgs);
                case DiagCode.TypeProhibitsAttributes:
                    return "Type '{0}' prohibits attributes.".InvFormat(_msgArgs);
                case DiagCode.TypeRequiresSimpleChild:
                    return "Type '{0}' requires simple child.".InvFormat(_msgArgs);
                case DiagCode.TypeRequiresComplexChildren:
                    return "Type '{0}' requires complex children.".InvFormat(_msgArgs);
                case DiagCode.TypeProhibitsChildren:
                    return "Type '{0}' prohibits children.".InvFormat(_msgArgs);
                case DiagCode.RequiredChildMemberIsNotMatched:
                    return "Required child member '{0}' is not matched.".InvFormat(_msgArgs);
                case DiagCode.ChildListCountIsNotGreaterThanOrEqualToMinOccurs:
                    return "Child list '{0}' count '{1}' is not greater than or equal to min occurs '{2}'.".InvFormat(_msgArgs);










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
        }
        public Diag(DiagSeverity severity, DiagMsg diagMsg, TextSpan textSpan, XObject obj) {
            Severity = severity;
            RawCode = (int)diagMsg.Code;
            Message = diagMsg.GetMessage();
            TextSpan = textSpan;
            Object = obj;
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


    public class Context {
        public Context() { }
        private List<Diag> _diagList;
        public List<Diag> DiagList {
            get {
                return _diagList ?? (_diagList = new List<Diag>());
            }
        }
        public void AddDiag(DiagSeverity severity, int rawCode, string message, TextSpan textSpan, XObject obj) {
            DiagList.Add(new Diag(severity, rawCode, message, textSpan, obj));
        }
        public void AddDiag(DiagSeverity severity, DiagMsg diagMsg, TextSpan textSpan, XObject obj) {
            DiagList.Add(new Diag(severity, diagMsg, textSpan, obj));
        }
        public void AddErrorDiag(DiagMsg diagMsg, TextSpan textSpan) {
            DiagList.Add(new Diag(DiagSeverity.Error, diagMsg, textSpan, null));
        }
        public void AddErrorDiag(DiagMsg diagMsg, XObject obj) {
            DiagList.Add(new Diag(DiagSeverity.Error, diagMsg, default(TextSpan), obj));
        }
        public bool HasDiags {
            get {
                return _diagList != null && _diagList.Count > 0;
            }
        }
        public bool HasErrorDiags {
            get {
                return HasErrorDiagsCore(0);
            }
        }
        private bool HasErrorDiagsCore(int index) {
            if (_diagList != null) {
                var count = _diagList.Count;
                for (; index < count; ++index) {
                    if (_diagList[index].IsError) {
                        return true;
                    }
                }
            }
            return false;
        }
        public virtual void Reset() {
            if (_diagList != null) {
                _diagList.Clear();
            }
        }


        public struct DiagsMarker {
            internal DiagsMarker(Context context) {
                Context = context;
                Index = context._diagList == null ? 0 : context._diagList.Count;
            }
            public readonly Context Context;
            public readonly int Index;
            public bool HasErrors {
                get {
                    return Context.HasErrorDiagsCore(Index);
                }
            }
            public void Restore() {
                var diagnostics = Context._diagList;
                if (diagnostics != null) {
                    diagnostics.RemoveRange(Index, diagnostics.Count - Index);
                }
            }
        }
        public DiagsMarker MarkDiags() {
            return new DiagsMarker(this);
        }
    }

}
