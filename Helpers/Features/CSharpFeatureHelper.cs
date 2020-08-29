using System;
using Serilog;
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
            Log.Information("Started extracting information about generated feature code from file: {Path}", path);

            var generatedStepDefinitionContent = File.ReadAllText(path);

            return CSharpSyntaxTree.ParseText(generatedStepDefinitionContent)
                .GetRoot()
                .DescendantNodes()
                .OfType<InvocationExpressionSyntax>()
                .Where(invokedMethod =>
                    invokedMethod.Expression is MemberAccessExpressionSyntax memberAccessExpressionSyntax &&
                    ApplicationConstants.GeneratedStepDefinitionMethods.Contains(memberAccessExpressionSyntax.Name
                        .ToString())
                ).Select(invokedMethod =>
                {
                    var memberAccessExpressionSyntax = invokedMethod.Expression as MemberAccessExpressionSyntax;
                    var methodArgumentTypeName = memberAccessExpressionSyntax?.Name.ToString();
                    var methodArgumentValue = invokedMethod.ArgumentList.Arguments.FirstOrDefault()?.ToString()
                        .Replace("\\\"", "\"");

                    Log.Information("Found method call of type {Type} with argument {Argument}",
                        methodArgumentTypeName, methodArgumentValue);

                    return new AttributeInformation
                    {
                        Type = methodArgumentTypeName,
                        Value = methodArgumentValue,
                        FeatureFileName = Path.GetFileNameWithoutExtension(path),
                        FeatureFilePath = path.Remove(path.LastIndexOf(".cs", StringComparison.InvariantCultureIgnoreCase))
                    };
                });
        }
    }
}