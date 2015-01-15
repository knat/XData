using System;
using System.Collections.Generic;

namespace XData.Compiler {
    public abstract class Symbol {

    }
    public sealed class ProgramSymbol : Symbol {
        public ProgramSymbol(bool needGenCode) {
            NeedGenCode = needGenCode;
            NamespaceList = new List<NamespaceSymbol>();
        }
        public readonly bool NeedGenCode;
        public readonly List<NamespaceSymbol> NamespaceList;
    }
    public sealed class NamespaceSymbol : Symbol {
        public NamespaceSymbol(string uri, CSNamespaceNameNode csNamespaceName, bool isCSNamespaceRef) {
            Uri = uri;
            CSNamespaceName = csNamespaceName;
            IsCSNamespaceRef = isCSNamespaceRef;
            GlobalObjectList = new List<GlobalObjectSymbol>();
        }
        public readonly string Uri;
        public readonly CSNamespaceNameNode CSNamespaceName;
        public readonly bool IsCSNamespaceRef;
        public readonly List<GlobalObjectSymbol> GlobalObjectList;

    }
    public abstract class ObjectSymbol : Symbol {

    }
    public abstract class GlobalObjectSymbol : Symbol {

        public readonly string Name;
    }
    public class TypeSymbol : GlobalObjectSymbol {

    }
    public class SimpleTypeSymbol : TypeSymbol {

    }
    public sealed class AtomicTypeSymbol : SimpleTypeSymbol {

    }
    public sealed class ListTypeSymbol : SimpleTypeSymbol {

    }
    public sealed class ComplexTypeSymbol : TypeSymbol {

    }
}