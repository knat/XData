using System;
using System.Collections.Generic;
using XData.IO.Text;

namespace XData.Compiler {

    public enum DiagnosticCodeEx {
        None = 0,
        AliasIsReserved = -2000,
        UriIsReserved,
        DuplicateUriAlias,
        InvalidUriAlias,
        DuplicateImportAlias,
        InvalidImportUri,
        DuplicateNamespaceMember,
        InvalidQualifiableNameAlias,
        AmbiguousNameReference,
        InvalidNameReference,
        InvalidTypeNameReference,
        InvalidAttributeNameReference,
        InvalidElementNameReference,
        UInt64ValueRequired,
        ByteValueRequired,
        MaxValueMustEqualToOrBeGreaterThanMinValue,
        MaxValueMustBeGreaterThanZero,
        DuplicateMemberName,
        DuplicateAttributeFullName,
        InvalidIndicatorNamespaceName,
        InconsistentCSharpNamespaceName,
        CSNamespaceNameNotSpecifiedForNamespace,
        CircularReferenceDetected,
        SimpleTypeRequired,
        ListItemTypeNotAllowed,
    }

    public sealed class ContextEx : Context {
        [ThreadStatic]
        public static ContextEx Current;
        public sealed class ContextException : Exception { }
        private static readonly ContextException _contextException = new ContextException();
        public static void ErrorDiagnostic(DiagnosticCodeEx code, string errMsg, TextSpan textSpan) {
            Current.AddDiagnostic(DiagnosticSeverity.Error, (int)code, errMsg, textSpan, null);
        }
        public static void ErrorDiagnosticAndThrow(DiagnosticCodeEx code, string errMsg, TextSpan textSpan) {
            ErrorDiagnostic(code, errMsg, textSpan);
            throw _contextException;
        }
        public static void ThrowIfHasErrors() {
            if (Current.HasErrorDiagnostics) {
                throw _contextException;
            }
        }
        public static void WarningDiagnostic(DiagnosticCodeEx code, string errMsg, TextSpan textSpan) {
            Current.AddDiagnostic(DiagnosticSeverity.Warning, (int)code, errMsg, textSpan, null);
        }

    }
}
