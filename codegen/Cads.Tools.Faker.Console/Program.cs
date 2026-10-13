using Cads.Tools.Faker;
using System;
using System.IO;
using System.Reflection;

internal class Program
{
    // Example usage:
    // 1) Build the project that contains your DbContext.
    // 2) Point the generator at the assembly and DbContext type.
    // 3) Run the generator to write generated faker files to disk.
    private static void Main(string[] args)
    {
        if (args.Length < 3)
        {
            Console.WriteLine("Usage: FakerGenerator <path-to-assembly.dll> <full.DbContext.TypeName> <output-directory>");
            return;
        }

        var asmPath = args[0];
        var dbContextTypeName = args[1];
        var outDir = args[2];

        var asm = Assembly.LoadFrom(asmPath);
        var dbContextType = asm.GetType(dbContextTypeName, throwOnError: true, ignoreCase: false);
        FakerGenerator.GenerateFakersFromDbContext(dbContextType, outDir);
        Console.WriteLine($"Generated fakers for DbContext {dbContextTypeName} into {Path.GetFullPath(outDir)}");
    }
}