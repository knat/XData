using System;
using System.Collections.Generic;

namespace XData.Compiler {
    public abstract class Symbol {

    }
    public sealed class ProgramSymbol : Symbol {
        public ProgramSymbol() {
            NamespaceList = new List<NamespaceSymbol>();
        }
        public readonly List<NamespaceSymbol> NamespaceList;
    }
    public sealed class NamespaceSymbol : Symbol {
        public NamespaceSymbol(string uri) {
            Uri = uri;
            GlobalObjectList = new List<GlobalObjectSymbol>();
        }
        public readonly string Uri;
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