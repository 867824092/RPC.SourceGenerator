using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Text;

namespace RPC.Server.SourceGenerator {
    internal class InterfaceSyntaxReceiver : ISyntaxReceiver {
        public List<InterfaceDeclarationSyntax> CandidateInterfaces { get; } = new List<InterfaceDeclarationSyntax>();
        public void OnVisitSyntaxNode(SyntaxNode syntaxNode) {
            if(syntaxNode is InterfaceDeclarationSyntax interfaceDeclarationSyntax) {
                //判断interfaceDeclarationSyntax 是否继承了IRPCService
                if(interfaceDeclarationSyntax.BaseList != null) {
                    foreach(var baseType in interfaceDeclarationSyntax.BaseList.Types) {
                        if(baseType.Type is IdentifierNameSyntax identifierNameSyntax) {
                            if(identifierNameSyntax.Identifier.Text == "IRPCService") {
                                CandidateInterfaces.Add(interfaceDeclarationSyntax);
                                return;
                            }
                        }
                    }
                }
            }
        }
    }
}
