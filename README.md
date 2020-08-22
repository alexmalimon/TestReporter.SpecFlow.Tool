# TestReporter.SpecFlow.Tool

TestReporter.SpecFlow.Tool is a .NET Core Global Tool uesd to generate HTML test report file for [SpecFlow](https://specflow.org/).

## Installation

```bash
dotnet tool install --global TestReporter.SpecFlow.Tool
```

## Usage

```text
Arguments: 
    
    -p, --project     Project name that will be displayed in report

    -s, --steps       Required. Path to folder with step definition files

    -f, --features    Required. Path to folder with feature files

    --help            Display this help screen.

    --version         Display version information.  
```

## Example

```text
specflow-report --features "Features" --steps "Steps" 

specflow-report --project "Test Project" --features "Features" --steps "Steps"
```