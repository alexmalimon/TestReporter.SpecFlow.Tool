﻿using CommandLine;

 namespace TestReporter.SpecFlow.Tool.Models.Console
{
    public class ConsoleArguments
    {
        [Option('p', "project", Required = false, HelpText = "Project name that will be displayed in report")]
        public string ProjectName { get; set; }
        
        [Option('s', "steps", Required = true, HelpText = "Path to folder with step definition files")]
        public string StepsFolderPath { get; set; }

        [Option('f', "features", Required = true, HelpText = "Path to folder with feature files")]
        public string FeaturesFolderPath { get; set; }
    }
}