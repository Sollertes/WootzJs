﻿using System.Collections.Generic;
using System.Linq;
using Roslyn.Compilers.CSharp;

namespace WootzJs.Compiler
{
    public class YieldClassGenerator 
    {
        private Compilation compilation;
        private MethodDeclarationSyntax node;
        private MethodSymbol method;
        
        public const string state = "$state";

        public YieldClassGenerator(Compilation compilation, MethodDeclarationSyntax node)
        {
            this.compilation = compilation;
            this.node = node;

            method = compilation.GetSemanticModel(node.SyntaxTree).GetDeclaredSymbol(node);
        }

        public ClassDeclarationSyntax CreateEnumerator()
        {
            var stateGenerator = new YieldStateGenerator(compilation, node);
            stateGenerator.GenerateStates();
            var states = stateGenerator.States;

            var members = new List<MemberDeclarationSyntax>();

            var stateField = Cs.Field(Cs.Int(), state);
            members.Add(stateField);

            var thisField = Cs.Field(Syntax.IdentifierName(method.ContainingType.Name), "$this");
            members.Add(thisField);

            foreach (var parameter in node.ParameterList.Parameters)
            {
                var parameterField = Cs.Field(parameter.Type, parameter.Identifier);
                members.Add(parameterField);
            }
            foreach (var variable in stateGenerator.HoistedVariables)
            {
                var variableField = Cs.Field(variable.Item2, variable.Item1);
                members.Add(variableField);
            }

            var className = "YieldEnumerator$" + method.GetMemberName();

            var constructorParameters = new[] { Syntax.Parameter(Syntax.Identifier("$this")).WithType(thisField.Declaration.Type) }.Concat(node.ParameterList.Parameters.Select(x => Syntax.Parameter(x.Identifier).WithType(x.Type)));
            var constructor = Syntax.ConstructorDeclaration(className)
                .AddModifiers(Cs.Public())
                .WithParameterList(constructorParameters.ToArray())
                .WithBody(
                    Syntax.Block(
                        // Assign fields
                        constructorParameters.Select(x => Cs.Express(Cs.Assign(Cs.This().Member(x.Identifier), Syntax.IdentifierName(x.Identifier))))
                    )
                    .AddStatements(
                        Cs.Express(Cs.Assign(Cs.This().Member(state), Cs.Integer(1)))
                    )
                );
            members.Add(constructor);

            var ienumerator = Syntax.ParseTypeName("System.Collections.IEnumerator");

            var getEnumerator = Syntax.MethodDeclaration(ienumerator, "GetEnumerator")
                .AddModifiers(Cs.Public(), Cs.Override())
                .WithBody(Syntax.Block(Cs.Return(Cs.This())));
            members.Add(getEnumerator);

            // Generate the MoveNext method, which looks something like:
            // while (true)
            // {
            //     switch (state) 
            //     {
            //         case 0: ...
            //         case 1: ...
            //     }
            // }
            
            var moveNextBody = Syntax.LabeledStatement("$top", Cs.While(Cs.True(), 
                Cs.Switch(Cs.This().Member(state), states.Select((x, i) => 
                    Cs.Section(Cs.Integer(i), x.Statements.ToArray())).ToArray())));

            var moveNext = Syntax.MethodDeclaration(Cs.Bool(), "MoveNext")
                .AddModifiers(Cs.Public(), Cs.Override())
                .WithBody(Syntax.Block(moveNextBody));
            members.Add(moveNext);

            var baseTypes = new[] { Syntax.ParseTypeName("System.YieldIterator<object>") };
            var result = Syntax.ClassDeclaration(className).WithBaseList(baseTypes).WithMembers(members.ToArray());

            if (method.TypeParameters.Any())
            {
                result = result.WithTypeParameterList(node.TypeParameterList);
            }

            return result;
        }
    }
}