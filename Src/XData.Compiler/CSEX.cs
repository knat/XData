using System;
using System.Collections.Generic;

namespace XData.Compiler {
    internal static class CSEX {
        //internal static readonly CSNamespaceNameNode XDataCSNamespaceName = new CSNamespaceNameNode { "XData" };

        internal static string ToClassName(this TypeKind kind) {
            return "X" + kind.ToString();
        }
    }
}
