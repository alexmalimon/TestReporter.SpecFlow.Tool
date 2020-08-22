using System.IO;
using System.Linq;
using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using TestReporter.SpecFlow.Tool.Constants;
using TestReporter.SpecFlow.Tool.Models.Attributes;

namespace TestReporter.SpecFlow.Tool.Helpers.StepDefinitions
{
    public  static class StepDefinitionHelper
    {
        public static IEnumerable<AttributeInformation> ExtractInformationFromFiles(
            IEnumerable<string> stepDefinitionFilePaths) => 
            stepDefinitionFilePaths
                .Select(ExtractInformationFromFile)
                .SelectMany(x => x);
        
        private static IEnumerable<AttributeInformation> ExtractInformationFromFile(string path)
        {
            var stepDefinitionContent = File.ReadAllText(path);

            var attributes = CSharpSyntaxTree.ParseText(stepDefinitionContent)
                .GetRoot()
                .DescendantNodes()
                .OfType<AttributeSyntax>()
                .Where(a => ApplicationConstants.StepDefinitionAttributeMethods.Contains(a.Name.ToString()));

            return attributes.Select(attribute => new AttributeInformation
            {
                Type = attribute.Name.ToString(),
                Value = attribute?.ArgumentList?.Arguments.FirstOrDefault()
                    ?.ToFullString()
                    .Replace("@", "")
                    .Replace("\"\"", "\"")
            });
        }
    }
}