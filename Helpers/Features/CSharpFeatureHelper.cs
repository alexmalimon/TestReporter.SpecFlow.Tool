using System;
using Serilog;
using System.IO;
using System.Linq;
using Microsoft.CodeAnalysis;
using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp;
using TestReporter.SpecFlow.Tool.Constants;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.CSharp.Scripting;
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
            Log.Information("Extracting information about generated feature code from file: {Path}", path);

            var generatedStepDefinitionContent = File.ReadAllText(path);

            var nodes = CSharpSyntaxTree.ParseText(generatedStepDefinitionContent)
                .GetRoot()
                .DescendantNodes()
                .ToList();

            var generatedStepDefinitions = ExtractInformationAboutGeneratedStepDefinitions(nodes, path);
            var generatedStepOutlines = ExtractInformationAboutGeneratedScenarioOutlines(nodes, path);
            return generatedStepDefinitions.Union(generatedStepOutlines);
        }

        private static IEnumerable<AttributeInformation> ExtractInformationAboutGeneratedStepDefinitions(
            IEnumerable<SyntaxNode> nodes, string path) =>
            nodes.OfType<InvocationExpressionSyntax>()
                .Where(invokedMethod =>
                    invokedMethod.Expression is MemberAccessExpressionSyntax memberAccessExpressionSyntax
                    && ApplicationConstants.GeneratedStepDefinitionMethods.Contains(memberAccessExpressionSyntax.Name
                        .ToString())
                    && invokedMethod.ArgumentList.Arguments.FirstOrDefault(arg =>
                        arg.ToString().Contains(ApplicationConstants.ExcludeExamplePattern)) == null
                )
                .Select(invokedMethod =>
                {
                    var memberAccessExpressionSyntax = invokedMethod.Expression as MemberAccessExpressionSyntax;
                    var methodArgumentTypeName = memberAccessExpressionSyntax?.Name.ToString();
                    var methodArgumentText = invokedMethod.ArgumentList.Arguments
                        .FirstOrDefault()
                        ?.ToString();

                    var methodArgumentValue = CSharpScript.EvaluateAsync<string>(methodArgumentText).Result;

                    Log.Information("Found method call of type {Type} with argument {Argument}",
                        methodArgumentTypeName, methodArgumentValue);

                    return new AttributeInformation
                    {
                        Type = methodArgumentTypeName,
                        Value = methodArgumentValue,
                        FeatureFileName = Path.GetFileNameWithoutExtension(path),
                        FeatureFilePath =
                            path.Remove(path.LastIndexOf(".cs", StringComparison.InvariantCultureIgnoreCase))
                    };
                });


        private static IEnumerable<AttributeInformation> ExtractInformationAboutGeneratedScenarioOutlines(
            List<SyntaxNode> nodes, string path)
        {
            var methodCalList = nodes
                .OfType<InvocationExpressionSyntax>()
                .Where(invokedMethod =>
                    invokedMethod.Expression is MemberAccessExpressionSyntax memberAccessExpressionSyntax
                    && ApplicationConstants.GeneratedStepDefinitionMethods.Contains(memberAccessExpressionSyntax.Name
                        .ToString())
                    && invokedMethod.ArgumentList.Arguments.FirstOrDefault(arg =>
                        arg.ToString().Contains(ApplicationConstants.ExcludeExamplePattern)) != null
                ).ToList();

            return methodCalList.Select(x =>
            {
                var memberAccessExpressionSyntax = x.Expression as MemberAccessExpressionSyntax;
                var methodArgumentTypeName = memberAccessExpressionSyntax?.Name.ToString();

                var formattedStringSyntax = x.ArgumentList.Arguments.FirstOrDefault();
                var invokeExpression = formattedStringSyntax?.Expression as InvocationExpressionSyntax;
                var formatStringText = $"return {invokeExpression?.ToFullString()};";

                var formatArgumentsNames = invokeExpression
                    ?.ArgumentList
                    .Arguments
                    .Skip(1)
                    .Select(arg => arg.ToFullString())
                    .ToList();

                var argumentNameValues = ExtractMethodInfoFromInvocation(x, nodes);
                var resultMap = argumentNameValues
                    .Select(arg => arg.Where(k => formatArgumentsNames?.Contains(k.Item1) == true)
                        .Select(v => new KeyValuePair<string, string>(v.Item1, v.Item2.ToFullString())).ToList())
                    .ToList();

                return resultMap.Select(args => args.ToDictionary(k => k.Key, v => v.Value)
                        .Aggregate(string.Empty, (state, kvp) =>
                            state + "\n" + $"var {kvp.Key} = {kvp.Value};")).Select(dict => CSharpScript
                        .RunAsync<string>(dict).Result
                        .ContinueWithAsync<string>(formatStringText).Result.ReturnValue)
                    .Select(r => new AttributeInformation
                    {
                        Type = methodArgumentTypeName,
                        Value = r,
                        FeatureFileName = Path.GetFileNameWithoutExtension(path),
                        FeatureFilePath =
                            path.Remove(path.LastIndexOf(".cs", StringComparison.InvariantCultureIgnoreCase))
                    });
            }).SelectMany(x => x.ToList()).ToList();
        }

        private static IEnumerable<List<ValueTuple<string, ArgumentSyntax>>> ExtractMethodInfoFromInvocation(
            SyntaxNode invocationExpressionSyntax, IEnumerable<SyntaxNode> nodes)
        {
            var methodInfo = FindMethodExpressionSyntax(invocationExpressionSyntax);
            var methodName = methodInfo.Identifier.Text;

            var methodParameters = methodInfo.ParameterList
                .Parameters
                .Select(x => x.Identifier.Text)
                .ToList();

            return nodes.OfType<InvocationExpressionSyntax>()
                .Where(im =>
                    im.Expression is MemberAccessExpressionSyntax mi
                    && mi.Name.ToString() == methodName)
                .Select(x => methodParameters.Zip(x.ArgumentList.Arguments).Take(methodParameters.Count - 1).ToList())
                .ToList();
        }

        private static MethodDeclarationSyntax FindMethodExpressionSyntax(SyntaxNode expressionSyntax) =>
            expressionSyntax switch
            {
                MethodDeclarationSyntax e => e,
                _ => FindMethodExpressionSyntax(expressionSyntax.Parent)
            };
    }
}