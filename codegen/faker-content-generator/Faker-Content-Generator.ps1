<#
.SYNOPSIS
  Generate C# `ContentGenerator` classes by reading DbContext source metadata (textual parsing).

.DESCRIPTION
  Scans a solution folder for a DbContext, extracts `DbSet<TEntity>` properties,
  finds matching entity source files, parses their public properties and emits
  C# generator classes implementing `IContentGenerator`.

.PARAMETER SolutionRoot
  Root folder to search for .cs files (default: current folder).

.PARAMETER DbContextFullName
  Optional fully-qualified DbContext name (e.g. "My.Namespace.MyDbContext"). If not provided,
  the script picks a matching class ending with 'DbContext' (heuristic).

.PARAMETER OutputDir
  Directory where .cs generator files will be written. Created if missing.
  Default: ".\GeneratedContentGenerators".

.PARAMETER OutputNamespace
  Namespace to use for generated classes. Default: "Faker.Generated".

.EXAMPLE
  powershell -File .\scripts\Generate-ContentGenerators.ps1 -SolutionRoot "C:\Projects\DEFRA\cads-data-service" `
    -DbContextFullName "Cads.Cds.SystemAdmin.Infrastructure.GraphQL.Schemas.Cads.Contexts.CadsGraphQLDbContext" `
    -OutputDir ".\src\Modules\Cads.Cds.SystemAdmin\Cads.Cds.SystemAdmin.Infrastructure\Faker\Generated" `
    -OutputNamespace "Cads.Cds.SystemAdmin.Infrastructure.Faker.Data.Generated"
#>

# PowerShell script: run from repo root
param(
    [string]$Domain = 'CtsTransactions',  # e.g. 'Cts', 'Cds', 'CtsAudit' etc. (used to locate DbContext and entity sources)'
    [string]$RootPath = '../../../cads-data-service/src/Modules/Cads.Cds.SystemAdmin/Cads.Cds.SystemAdmin.Infrastructure',
    [string]$OutputDir = "../../../cads-data-service/src/Modules/Cads.Cds.SystemAdmin/Cads.Cds.SystemAdmin.Infrastructure/Data/Faker/Schemas/" + $Domain + "/ContentGenerators",
    [string]$OutputNamespace = "Cads.Cds.SystemAdmin.Infrastructure.Data.Faker.Schemas." + $Domain + ".ContentGenerators"
)

$DbContextClassName = $Domain + "SystemAdminDbContext"  
$DbContextPath = $RootPath + '/Data/Schemas/' + $Domain + '/Contexts/' + $DbContextClassName + '.cs'

$RootNamespace = 'Cads.Cds.SystemAdmin.Infrastructure'
$ContextNamespace = $RootNamespace + '.Data.Schemas.' + $Domain + '.Contexts'
$EntitiesNamespace = $RootNamespace + '.Data.Schemas.' + $Domain + '.Entities'
$FakerNamespace = $RootNamespace + '.Data.Faker.Schemas.' + $Domain

# Helpers
function Read-AllCsFiles {
    param($root)
    Get-ChildItem -Path $root -Recurse -Include *.cs `
    | Where-Object { $_.FullName -notmatch '\\(bin|obj)\\' -and $_.Name -notmatch '\.g\.cs$|\.designer\.cs$' } `
    | Select-Object -ExpandProperty FullName
}

function Read-FileText([string]$path) {
    if (Test-Path $path) { 
        Get-Content -Raw -ErrorAction Stop $path 
    } 
    else { 
        "" 
    }
}

# Regex patterns closely following the T4 behavior
$dbClassRegex = [regex]'(?:\b(?:public|internal|private|protected)?\s*(?:partial\s+)?class\s+)([A-Za-z0-9_]+)\b'
$dbSetPropRegex = [regex]'\b\bpublic?\s*(?:virtual.*)\s+DbSet\s*<\s*([^>]+)\s*>\s+([A-Za-z0-9_]+)\s*\{'
$entityPropRegex = [regex]'\bpublic\s+([A-Za-z0-9_<>\.\?\[\]]+)\s+([A-Za-z0-9_]+)\s*\{\s*get\s*;\s*set\s*;\s*\}'

# Collect files
$allCs = Read-AllCsFiles -root $RootPath

if (-not $allCs) {
    Write-Error "No .cs files found beneath $RootPath. Aborting."
    exit 2
}

# Parse DbSet<TEntity> from chosen file
$dbContextText = Read-FileText $DbContextPath
$dbSetMatches = $dbSetPropRegex.Matches($dbContextText)
$entityTypeNames = [System.Collections.Generic.HashSet[string]]::new([System.StringComparer]::Ordinal)

foreach ($m in $dbSetMatches) {
    $entityTypeText = $m.Groups[1].Value.Trim()
    # handle "Namespace.Entity" or "Entity"
    $short = ($entityTypeText.Split(',')[0]).Trim()
    $short = $short.Split('.')[-1]
    if ($short -and -not $short.Contains('<')) {
        $null = $entityTypeNames.Add($short)
    }
}

# fallback heuristics (IQueryable/IEnumerable)
if ($entityTypeNames.Count -eq 0) {
    $enumRegex = [regex]'\bpublic\s+(?:IQueryable|IEnumerable)\s*<\s*([A-Za-z0-9_.]+)\s*>\s+([A-Za-z0-9_]+)\s*\{'
    $enumMatches = $enumRegex.Matches($dbContextText)
    foreach ($mm in $enumMatches) {
        $short = $mm.Groups[1].Value.Split('.')[-1].Trim()
        if ($short) { $null = $entityTypeNames.Add($short) }
    }
}

if ($entityTypeNames.Count -eq 0) {
    Write-Error "No entity types discovered in DbContext source. Aborting."
    exit 5
}

# Map entity short-name to source file (first match by class name)
$entityFiles = @{}
foreach ($name in $entityTypeNames) {
    $found = $allCs | Where-Object {
        $txt = Read-FileText $_
        $classMatch = $dbClassRegex.Match($txt)
        # check if any class name equal to entity name (including partials)
        ($txt -match ("class\s+$name\b"))
    } | Select-Object -First 1

    if ($found) {
        $entityFiles[$name] = $found
    }
    else {
        Write-Warning "Entity source not found for '$name' (searching by class name). Generated generator will use property-less stub."
    }
}

# Ensure output dir exists
$fullOut = Resolve-Path -Path $OutputDir -ErrorAction SilentlyContinue | Select-Object -ExpandProperty Path -ErrorAction SilentlyContinue
if (-not $fullOut) { New-Item -ItemType Directory -Path $OutputDir -Force | Out-Null; $fullOut = Resolve-Path $OutputDir }
Write-Host "Writing generated files to: $fullOut"

# For each entity, parse properties and write generator class
foreach ($kv in $entityFiles.GetEnumerator()) {
    $entity = $kv.Key
    $file = $kv.Value
    $txt = Read-FileText $file

    $props = @()
    foreach ($pm in $entityPropRegex.Matches($txt)) {
        $ptype = $pm.Groups[1].Value.Trim()
        $pname = $pm.Groups[2].Value.Trim()
        $props += [PSCustomObject]@{ Name = $pname; Type = $ptype }
    }

    $className = "${entity}ContentGenerator"
    $outPath = Join-Path $fullOut "$className.cs"

    # Build basic C# class content. This emits simple deterministic fake data based on index + seed.
    $propsJson = ($props | ForEach-Object { "{ Name='$($_.Name)'; Type='$($_.Type)'}" }) -join ", "

    $propAssignments = ""
    $entityProperties = ""
    $entityMapperProperties = ""

    foreach ($p in $props) {
        $pname = $p.Name
        $ptype = $p.Type.ToLower()
        if ($ptype -match 'int|long|short|byte') {
            $valExpr = "seed + i"
        }
        elseif ($ptype -match 'decimal|double|float') {
            $valExpr = "(decimal)(seed + i) + 0.5m"
        }
        elseif ($ptype -match 'datetime') {
            $valExpr = 'DateTime.UtcNow.AddDays(i).ToString("o")'
        }
        elseif ($ptype -match 'bool') {
            $valExpr = "( (seed + i) % 2 == 0 )"
        }
        else {
            # string or other: create a predictable string
            $valExpr = '"{0}_{1}".Replace("{0}", "' + $entity + '").Replace("{1}", (seed + i).ToString())'
        }

        # use dictionary[string, object]
        if ($ptype -match 'datetime') {
            $propAssignments += "                dict[""$pname""] = $($valExpr);`n"
        }
        elseif ($ptype -match 'bool') {
            $propAssignments += "                dict[""$pname""] = $($valExpr);`n"
        }
        elseif ($ptype -match 'int|long|short|byte|decimal|double|float') {
            $propAssignments += "                dict[""$pname""] = $($valExpr);`n"
        }
        else {
            $propAssignments += "                dict[""$pname""] = $($valExpr);`n"
        }

        $entityProperties += "    public $($p.Type) $($p.Name) { get; init; }`n"
        $entityMapperProperties += "        [""$($p.Name)""] = row.$($p.Name),`n"
    }
 
    $fakeEntityClassName = "${entity}Row"
    $fakeEntityClassExtensionsName = "${entity}RowExtensions"
    $classContent = @"
// <auto-generated />
using System;
using System.Collections.Generic;
using System.Linq;
using Bogus;
using $ContextNamespace;
using $EntitiesNamespace;
using $FakerNamespace;

namespace $OutputNamespace;

public sealed record $fakeEntityClassName
{
$entityProperties
}

public class $className : IContentGenerator
{
    public string Table => "$entity";

    public IReadOnlyList<IReadOnlyDictionary<string, object>> GenerateBulk(IReadOnlyList<decimal> businessKeys, int seed)
    {
        var faker = CreateFaker(seed);
        return businessKeys
            .Select(key => (faker.Generate() with { TransType = "B", LocId = key }).ToColumnMap())
            .ToList();
    }

    public IReadOnlyDictionary<string, object> GenerateUpdate(decimal businessKey, int seed)
    {
        var faker = CreateFaker(seed);
        return (faker.Generate() with { TransType = "U", LocId = businessKey }).ToColumnMap();
    }

    protected void ApplyDefaultRules(Faker<$fakeEntityClassName> faker)
    {
        //TODO: Implement default rules for faker properties here. For example:
        //faker.RuleFor(x => x.PropertyName, f => f.Random.Int(1, 100));
    }

    protected void ApplyProfileOverrides(Faker<$fakeEntityClassName> faker)
    {
        //TODO: Implement profile-specific overrides for faker properties here. For example:
        //faker.RuleFor(x => x.PropertyName, f => f.Random.Int(100, 200));
    }

    private Faker<$fakeEntityClassName> CreateFaker(int seed)
    {
        var faker = new Faker<$fakeEntityClassName>();
        ApplyDefaultRules(faker);      // baseline first...
        ApplyProfileOverrides(faker);  // ...profile layered on top
        faker.UseSeed(seed);
        return faker;
    }
}

internal static class $fakeEntityClassExtensionsName
{
    // Maps to the actual CSV column contract. TBC against the real file spec —
    // assumes a 1:1 column mirror, excluding trans_id/imported_date/
    // cts_file_import_id/loc_aud_*/row_number, which the import pipeline owns.
    public static IReadOnlyDictionary<string, object?> ToColumnMap(this $fakeEntityClassName row) => new Dictionary<string, object?>
    {
    $entityMapperProperties
    };
}
"@

    # Write to file
    Set-Content -Path $outPath -Value $classContent -Encoding UTF8
    Write-Host "Generated $outPath (props: $($props.Count))"
}


Write-Host "Done. Review generated files and adjust fake-data expressions if you need richer Bogus-based generation."