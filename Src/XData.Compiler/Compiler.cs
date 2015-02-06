using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace XData.Compiler {
    public static class XDataCompiler {
        public static bool Compile(List<string> schemaFilePathList, List<string> indicatorFilePathList,
            out DiagContext context, out string code) {
            context = null;
            code = null;
            var schemaFileCount = schemaFilePathList.CountOrZero();
            var indicatorFileCount = indicatorFilePathList.CountOrZero();
            if (schemaFileCount == 0 && indicatorFileCount == 0) {
                return true;
            }
            context = new DiagContextEx();
            List<CompilationUnitNode> compilationUnitList = null;
            List<IndicatorCompilationUnitNode> indicatorCompilationUnitList = null;
            try {
                if (schemaFileCount > 0) {
                    compilationUnitList = new List<CompilationUnitNode>();
                    foreach (var filePath in schemaFilePathList) {
                        using (var reader = new StreamReader(filePath)) {
                            CompilationUnitNode cuNode;
                            if (Parser.Parse(filePath, reader, context, out cuNode)) {
                                compilationUnitList.Add(cuNode);
                            }
                            else {
                                return false;
                            }
                        }
                    }
                }
                if (indicatorFileCount > 0) {
                    indicatorCompilationUnitList = new List<IndicatorCompilationUnitNode>();
                    foreach (var filePath in indicatorFilePathList) {
                        using (var reader = new StreamReader(filePath)) {
                            IndicatorCompilationUnitNode cuNode;
                            if (Parser.Parse(filePath, reader, context, out cuNode)) {
                                indicatorCompilationUnitList.Add(cuNode);
                            }
                            else {
                                return false;
                            }
                        }
                    }
                }
                if (!Core((DiagContextEx)context, compilationUnitList, indicatorCompilationUnitList, out code)) {
                    return false;
                }
                return true;
            }
            catch (Exception ex) {
                context.AddDiag(DiagSeverity.Error, (int)DiagCodeEx.InternalCompilerError, "Internal compiler error: " + ex.ToString(), default(IO.Text.TextSpan), null);
            }
            return false;
        }
        public const string GeneratedFileBanner = @"//
//Auto-generated, DO NOT EDIT.
//Visit https://github.com/knat/XData for more information.
//
";
        private static bool Core(DiagContextEx context,
            List<CompilationUnitNode> compilationUnitList, List<IndicatorCompilationUnitNode> indicatorCompilationUnitList, out string code) {
            code = null;
            DiagContextEx.Current = context;
            try {
                var nsList = new List<NamespaceNode>();
                if (compilationUnitList != null) {
                    foreach (var cu in compilationUnitList) {
                        if (cu.NamespaceList != null) {
                            nsList.AddRange(cu.NamespaceList);
                        }
                    }
                }
                var indicatorList = new List<IndicatorNode>();
                if (indicatorCompilationUnitList != null) {
                    foreach (var cu in indicatorCompilationUnitList) {
                        if (cu.IndicatorList != null) {
                            indicatorList.AddRange(cu.IndicatorList);
                        }
                    }
                }
                //
                var nsSet = new LogicalNamespaceSet();
                foreach (var ns in nsList) {
                    var uri = ns.Uri;
                    LogicalNamespace logicalNS;
                    if (!nsSet.TryGetValue(uri, out logicalNS)) {
                        logicalNS = new LogicalNamespace();
                        nsSet.Add(uri, logicalNS);
                    }
                    logicalNS.Add(ns);
                    ns.LogicalNamespace = logicalNS;
                }
                if (indicatorList.Count > 0) {
                    foreach (var indicator in indicatorList) {
                        LogicalNamespace logicalNS;
                        if (!nsSet.TryGetValue(indicator.Uri, out logicalNS)) {
                            DiagContextEx.ErrorDiagAndThrow(new DiagMsgEx(DiagCodeEx.InvalidIndicatorUri, indicator.Uri),
                                indicator.UriNode.TextSpan);
                        }
                        if (logicalNS.CSharpNamespaceName == null) {
                            logicalNS.CSharpNamespaceName = indicator.CSharpNamespaceName;
                            logicalNS.IsCSharpNamespaceRef = indicator.IsRef;
                        }
                        else if (logicalNS.IsCSharpNamespaceRef != indicator.IsRef || logicalNS.CSharpNamespaceName != indicator.CSharpNamespaceName) {
                            DiagContextEx.ErrorDiagAndThrow(new DiagMsgEx(DiagCodeEx.InconsistentCSharpNamespaceName,
                                logicalNS.IsCSharpNamespaceRef ? "&" : "=", logicalNS.CSharpNamespaceName.ToString(), indicator.IsRef ? "&" : "=", indicator.CSharpNamespaceName.ToString()),
                                indicator.CSharpNamespaceName.TextSpan);
                        }

                    }
                    foreach (var logicalNS in nsSet.Values) {
                        if (logicalNS.CSharpNamespaceName == null) {
                            DiagContextEx.ErrorDiagAndThrow(new DiagMsgEx(DiagCodeEx.CSNamespaceNameNotSpecifiedForNamespace, logicalNS.Uri),
                                indicatorList[0].TextSpan);
                        }
                    }
                }
                else {
                    var idx = 0;
                    foreach (var logicalNS in nsSet.Values) {
                        logicalNS.CSharpNamespaceName = new CSharpNamespaceNameNode() { "__fake_ns__" + (idx++).ToInvString() };
                        logicalNS.IsCSharpNamespaceRef = true;
                    }
                }
                if (nsList.Count == 0) {
                    return true;
                }
                //
                foreach (var ns in nsList) {
                    ns.ResolveImports(nsSet);
                }
                foreach (var logicalNS in nsSet.Values) {
                    logicalNS.CheckDuplicateMembers();
                }
                foreach (var ns in nsList) {
                    ns.Resolve();
                }
                //
                var nsSymbolList = new List<NamespaceSymbol>();
                foreach (var logicalNS in nsSet.Values) {
                    logicalNS.NamespaceSymbol = new NamespaceSymbol(logicalNS.Uri, logicalNS.CSharpNamespaceName, logicalNS.IsCSharpNamespaceRef);
                    nsSymbolList.Add(logicalNS.NamespaceSymbol);
                }
                foreach (var ns in nsList) {
                    ns.CreateSymbols();
                }
                var needGenCode = indicatorList.Count > 0;
                if (needGenCode) {
                    var memberList = new List<MemberDeclarationSyntax>();
                    foreach (var ns in nsSymbolList) {
                        ns.Generate(memberList);
                    }
                    //>internal sealed class XDataProgramInfo : ProgramInfo {
                    //>    private XDataProgramInfo() { }
                    //>    public static readonly XDataProgramInfo Instance = new XDataProgramInfo();
                    //>    protected override List<NamespaceInfo> GetNamespaces() {
                    //>        return new List<NamespaceInfo>() {
                    //>            ...
                    //>        };
                    //>    }
                    //>}
                    memberList.Add(CS.Class(null, CS.InternalSealedTokenList, "XDataProgramInfo", new[] { CSEX.ProgramInfoName },
                        CS.Constructor(CS.PrivateTokenList, "XDataProgramInfo", null, null),
                        CS.Field(CS.PublicStaticReadOnlyTokenList, CS.IdName("XDataProgramInfo"), "Instance", CS.NewObjExpr(CS.IdName("XDataProgramInfo"))),
                        CS.Method(CS.ProtectedOverrideTokenList, CS.ListOf(CSEX.NamespaceInfoName), "GetNamespaces", null,
                            CS.ReturnStm(CS.NewObjExpr(CS.ListOf(CSEX.NamespaceInfoName), null, nsSymbolList.Select(i => i.InfoExpr))))));
                    code = GeneratedFileBanner + SyntaxFactory.CompilationUnit(default(SyntaxList<ExternAliasDirectiveSyntax>), default(SyntaxList<UsingDirectiveSyntax>), default(SyntaxList<AttributeListSyntax>),
                          SyntaxFactory.List(memberList)).NormalizeWhitespace().ToString();
                }
                return true;
            }
            catch (DiagContextEx.ContextException) { }
            return false;
        }
    }
}
