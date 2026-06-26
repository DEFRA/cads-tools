using Cads.Tools.Console.Helpers;
using System.CommandLine;
using System.Text;

var connectionStringOption = new Option<string>("--connection")
{
    Description = "Database connection string",
    Required = true,
};

var typeOption = new Option<string>("--type")
{
    Description = "The type of code to generate (class, enum)"
};

var outputFileOption = new Option<string>("--output")
{
    Description = "The path to the output file"
};


typeOption.Validators.Add(result =>
{
    var value = result.GetValueOrDefault<string>();
    if (string.IsNullOrWhiteSpace(value))
    {
        result.AddError("Type cannot be empty.");
    }

    if (value != "class" && value != "enum")
    {
        result.AddError("Type must be either 'class' or 'enum'.");
    }
});

var schemaOption = new Option<string>("--schema")
{
    Description = "The schema for the generated code"
};

var namespaceOption = new Option<string>("--namespace")
{
    Description = "The namespace for the generated code"
};

var verboseOption = new Option<bool>("--verbose")
{
    Description = "Enable verbose output"
};

var generateCommand = new Command("generate")
{
    Description = "Generate code based on the provided options",
    Arguments = { },
    Options = { connectionStringOption, namespaceOption, schemaOption, outputFileOption, verboseOption, typeOption }
};

var rootCommand = new RootCommand("A command-line tool for code generation")
{
    Options = { verboseOption },
    Subcommands = { generateCommand }
};

generateCommand.SetAction(async (parseResult) =>
{
    bool verbose = false;

    try
    {
        verbose = parseResult.GetValue<bool>(verboseOption);
        var connectionString = parseResult.GetValue<string>(connectionStringOption)!;
        var outputFile = parseResult.GetValue<string>(outputFileOption);
        var @namespace = parseResult.GetValue<string>(namespaceOption) ?? string.Empty;
        var schema = parseResult.GetValue<string>(schemaOption) ?? "public";
        var type = parseResult.GetValue<string>(typeOption) ?? "class";

        if (verbose) System.Console.WriteLine("Starting code generation...");
        if (verbose) System.Console.WriteLine($"Connecting to database: {connectionString}");

        var generatedCode = string.Empty;

        if (type == "enum")
        {
            generatedCode = await CodeGenerator.GenerateTableEnumAsync(connectionString, schema, @namespace, verbose);
        }
        else
        {
            generatedCode = await CodeGenerator.GenerateClassFromSchemaAsync(connectionString, schema, @namespace, verbose);
        }

        // Write to output file
        if (!string.IsNullOrEmpty(outputFile))
        {
            await File.WriteAllTextAsync(outputFile, generatedCode, Encoding.UTF8);
            if (verbose) System.Console.WriteLine($"Code generated successfully and written to: {outputFile}");
        }
        else
        {
            System.Console.WriteLine(generatedCode);
        }
    }
    catch (Exception ex)
    {
        System.Console.Error.WriteLine($"✗ Error: {ex.Message}");
        if (verbose) System.Console.Error.WriteLine($"Stack trace: {ex.StackTrace}");
        Environment.Exit(1);
    }
});

return await rootCommand.Parse(args).InvokeAsync();