using System;
using System.Collections.Generic;

namespace XData.Compiler {
    public enum DiagnosticCodeEx {
        None = 0,
        AliasSysIsReserved = -2000,
        DuplicateUriAlias,
        InvalidUriAlias,
        DuplicateImportAlias,
        DuplicateNamespaceMember,

        UInt64ValueRequired,
        ByteValueRequired,
        MaxValueMustEqualToOrBeGreaterThanMinValue,
        MaxValueMustBeGreaterThanZero,

    }
}
