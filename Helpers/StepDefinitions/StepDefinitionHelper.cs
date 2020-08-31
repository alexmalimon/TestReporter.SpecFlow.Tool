using Serilog;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using TestReporter.SpecFlow.Tool.Constants;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using TestReporter.SpecFlow.Tool.Models.Attributes;

namespace TestReporter.SpecFlow.Tool.Helpers.StepDefinitions
{
    public static class StepDefinitionHelper
    {
        public static IEnumerable<AttributeInformation> ExtractInformationFromFiles(
            IEnumerable<string> stepDefinitionFilePaths) =>
            stepDefinitionFilePaths
                .Select(ExtractInformationFromFile)
                .SelectMany(x => x);

        private static IEnumerable<AttributeInformation> ExtractInformationFromFile(string path)
        {
            Log.Information("Started extracting information about step definitions from file: {Path}", path);

            var stepDefinitionContent = File.ReadAllText(path);

            return CSharpSyntaxTree.ParseText(stepDefinitionContent)
                .GetRoot()
                .DescendantNodes()
                .OfType<AttributeSyntax>()
                .Where(a => ApplicationConstants.StepDefinitionAttributeMethods.Contains(a.Name.ToString()))
                .Select(attribute =>
                {
                    var argumentType = attribute.Name.ToString();
                    var attributeArgumentText = attribute.ArgumentList?.Arguments
                        .FirstOrDefault()
                        ?.ToFullString();

                    var argumentValue = CSharpScript.EvaluateAsync<string>(attributeArgumentText).Result;

                    Log.Information("Found attribute of type {Type} with argument {Argument}",
                        argumentType, argumentValue);

                    return new AttributeInformation
                    {
                        Type = argumentType,
                        Value = argumentValue
                    };
                });
        }
    }
}