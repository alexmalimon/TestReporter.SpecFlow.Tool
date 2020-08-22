using System.IO;
using System.Linq;
using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using TestReporter.SpecFlow.Tool.Constants;
using TestReporter.SpecFlow.Tool.Models.Attributes;

namespace TestReporter.SpecFlow.Tool.Helpers.Features
{
    public static class CSharpFeatureHelper
    {
        public static IEnumerable<AttributeInformation> ExtractInformationFromFiles(
            IEnumerable<string> stepDefinitionGeneratedFilePaths) => 
            stepDefinitionGeneratedFilePaths.Select(ExtractInformationFromFile)
                .SelectMany(x => x);
        
        private static IEnumerable<AttributeInformation> ExtractInformationFromFile(string path)
        {
            var generatedStepDefinitionContent = File.ReadAllText(path);
            var invokedMethods = CSharpSyntaxTree.ParseText(generatedStepDefinitionContent)
                .GetRoot()
                .DescendantNodes()
                .OfType<InvocationExpressionSyntax>();
            
            return invokedMethods.Where(invokedMethod =>
                invokedMethod.Expression is MemberAccessExpressionSyntax memberAccessExpressionSyntax &&
                ApplicationConstants.GeneratedStepDefinitionMethods.Contains(memberAccessExpressionSyntax.Name.ToString())
            ).Select(invokedMethod =>
            {
                var memberAccessExpressionSyntax = invokedMethod.Expression as MemberAccessExpressionSyntax;
                var methodArgumentTypeName = memberAccessExpressionSyntax?.Name.ToString();
                var methodArgumentValue = invokedMethod.ArgumentList.Arguments.FirstOrDefault()?.ToString();

                return new AttributeInformation
                {
                    Type = methodArgumentTypeName,
                    Value = methodArgumentValue?.Replace("\\\"", "\"")
                };
            });
        }
    }
}