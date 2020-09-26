# TestReporter.SpecFlow.Tool

[![NuGet version (TestReporter.SpecFlow.Tool)](https://img.shields.io/nuget/v/TestReporter.SpecFlow.Tool.svg?style=flat-square&logo=nuget)](https://www.nuget.org/packages/TestReporter.SpecFlow.Tool/)

TestReporter.SpecFlow.Tool is a .NET Core Global Tool used to generate HTML report file for [SpecFlow](https://specflow.org/) step definitions usage.

## Installation

```text
dotnet tool install --global TestReporter.SpecFlow.Tool
```

## Usage

```text
Arguments:

  -p, --project    Required. Path to the SpecFlow project folder

  -o, --output     Path to directory, where test report file will be saved

  --help           Display this help screen.

  --version        Display version information.
```

## Examples

#### Generate report for project and save HTML in current folder:
```text
specflow-report --project "Test project folder"
```

#### Generate report for project and save HTML in output folder:
```text
specflow-report --project "Test project folder" --output "Report Output folder"
```