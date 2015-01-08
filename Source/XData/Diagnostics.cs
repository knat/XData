using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XData.TextIO;

namespace XData {
    public enum DiagnosticSeverity : byte {
        None = 0,
        Error = 1,
        Warning = 2,
        Info = 3
    }
    public enum DiagnosticCode {
        None = 0,
        Parsing = -1000,
        //DuplicateDefaultUri,
        //DuplicateUriAlias,
        //InvalidUriAlias,
        ElementIsAbstract,
        ElementIsNotNullable,
        ElementRequiresComplexValue,
        ElementRequiresSimpleValue,
        InvalidTypeName,
        TypeDoesNotEqualToOrDeriveFrom,
        TypeIsAbstract,
        TypeDoesNotAllowAttributes,

        DuplicateAttributeFullName,

        Max
    }
    [Serializable]
    public struct Diagnostic {
        public Diagnostic(DiagnosticSeverity severity, int rawCode, string message, TextSpan textSpan, XObject obj) {
            //if (!severity.IsValid()) throw new ArgumentOutOfRangeException("severity");
            //if (message == null) throw new ArgumentNullException("message");
            Severity = severity;
            RawCode = rawCode;
            Message = message;
            TextSpan = textSpan;
            Object = obj;
        }
        public Diagnostic(DiagnosticSeverity severity, DiagnosticCode code, string message, TextSpan textSpan, XObject obj)
            : this(severity, (int)code, message, textSpan, obj) {
            //if (!code.IsValid()) throw new ArgumentOutOfRangeException("code");
        }
        public readonly DiagnosticSeverity Severity;
        public readonly int RawCode;
        public readonly string Message;
        public readonly TextSpan TextSpan;//opt
        public readonly XObject Object;//opt
        public bool IsError {
            get {
                return Severity == DiagnosticSeverity.Error;
            }
        }
        public bool IsWarning {
            get {
                return Severity == DiagnosticSeverity.Warning;
            }
        }
        public bool IsInfo {
            get {
                return Severity == DiagnosticSeverity.Info;
            }
        }
        public DiagnosticCode Code {
            get {
                return (DiagnosticCode)RawCode;
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
                return Severity != DiagnosticSeverity.None;
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
    [Serializable]
    public class Context {
        public Context() { }
        private List<Diagnostic> _diagnostics;
        public List<Diagnostic> Diagnostics {
            get {
                return _diagnostics ?? (_diagnostics = new List<Diagnostic>());
            }
        }
        public void AddDiagnostic(DiagnosticSeverity severity, int rawCode, string message, TextSpan textSpan, XObject obj) {
            Diagnostics.Add(new Diagnostic(severity, rawCode, message, textSpan, obj));
        }
        public void AddDiagnostic(DiagnosticSeverity severity, DiagnosticCode code, string message, TextSpan textSpan, XObject obj) {
            Diagnostics.Add(new Diagnostic(severity, code, message, textSpan, obj));
        }
        public void AddErrorDiagnostic(DiagnosticCode code, string message, TextSpan textSpan) {
            Diagnostics.Add(new Diagnostic(DiagnosticSeverity.Error, code, message, textSpan, null));
        }
        public void AddErrorDiagnostic(DiagnosticCode code, string message, XObject obj) {
            Diagnostics.Add(new Diagnostic(DiagnosticSeverity.Error, code, message, default(TextSpan), obj));
        }
        public bool HasDiagnostics {
            get {
                return _diagnostics != null && _diagnostics.Count > 0;
            }
        }
        public bool HasErrorDiagnostics {
            get {
                return HasErrorDiagnosticsCore(0);
            }
        }
        private bool HasErrorDiagnosticsCore(int startIndex) {
            if (_diagnostics != null) {
                for (var i = startIndex; i < _diagnostics.Count; i++) {
                    if (_diagnostics[i].IsError) {
                        return true;
                    }
                }
            }
            return false;
        }
        public virtual void Reset() {
            if (_diagnostics != null) {
                _diagnostics.Clear();
            }
        }


        public struct DiagnosticsMarker {
            internal DiagnosticsMarker(Context context) {
                Context = context;
                DiagnosticsIndex = context._diagnostics == null ? 0 : context._diagnostics.Count;
            }
            public readonly Context Context;
            public readonly int DiagnosticsIndex;
            public bool HasErrors {
                get {
                    return Context.HasErrorDiagnosticsCore(DiagnosticsIndex);
                }
            }
            public void Restore() {
                var diagnostics = Context._diagnostics;
                if (diagnostics != null) {
                    diagnostics.RemoveRange(DiagnosticsIndex, diagnostics.Count - DiagnosticsIndex);
                }
            }
        }
        public DiagnosticsMarker MarkDiagnostics() {
            return new DiagnosticsMarker(this);
        }
    }

}
