using Microsoft.EntityFrameworkCore;
using System.Collections;
using System.Reflection;
using System.Text;

namespace Cads.Tools.Faker
{
    public static class FakerGenerator
    {
        /// <summary>
        /// Scans the provided DbContext type for DbSet<TEntity> properties and generates Bogus faker classes.
        /// Output: one strongly-typed faker class per entity in the specified outputDirectory.
        /// Requires a reference to the assembly that contains the DbContext and to the Bogus NuGet package.
        /// </summary>
        public static void GenerateFakersFromDbContext(Type dbContextType, string outputDirectory, string generatedNamespace = "Generated.Fakers")
        {
            if (dbContextType == null) throw new ArgumentNullException(nameof(dbContextType));
            if (!typeof(DbContext).IsAssignableFrom(dbContextType)) throw new ArgumentException("Type must derive from DbContext", nameof(dbContextType));
            Directory.CreateDirectory(outputDirectory);

            var dbSetOpenType = typeof(DbSet<>);
            var props = dbContextType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.PropertyType.IsGenericType && p.PropertyType.GetGenericTypeDefinition() == dbSetOpenType)
                .ToArray();

            foreach (var p in props)
            {
                var entityType = p.PropertyType.GetGenericArguments()[0];
                var code = BuildFakerForEntity(entityType, generatedNamespace);
                var filename = Path.Combine(outputDirectory, $"{entityType.Name}Faker.Generated.cs");
                File.WriteAllText(filename, code, Encoding.UTF8);
            }
        }

        static string BuildFakerForEntity(Type entityType, string ns)
        {
            var sb = new StringBuilder();
            sb.AppendLine("using System;");
            sb.AppendLine("using Bogus;");
            sb.AppendLine("using System.Collections.Generic;");
            sb.AppendLine();
            sb.AppendLine($"namespace {ns}");
            sb.AppendLine("{");
            sb.AppendLine($"    public class {entityType.Name}Faker : Faker<{GetTypeName(entityType)}>");
            sb.AppendLine("    {");
            sb.AppendLine($"        public {entityType.Name}Faker()");
            sb.AppendLine("        {");
            sb.AppendLine("            // Seed for repeatability (optional)");
            sb.AppendLine("            // Randomizer.Seed = new Random(123456);");
            sb.AppendLine();

            var props = entityType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                // avoid indexers
                .Where(pi => pi.GetIndexParameters().Length == 0)
                // skip primary key auto-generated patterns if desired (optional)
                .ToArray();

            foreach (var prop in props)
            {
                // Skip navigation collections to avoid deep graph generation
                if (IsCollection(prop.PropertyType))
                {
                    sb.AppendLine($"            // {prop.Name}: collection (skipped)");
                    continue;
                }

                // Optional: skip properties that are complex navigation types to avoid cycles
                if (IsEntityType(prop.PropertyType, entityType.Assembly))
                {
                    sb.AppendLine($"            // {prop.Name}: navigation to another entity (generate stub or skip to avoid cycles)");
                    sb.AppendLine($"            // RuleFor(x => x.{prop.Name}, f => new {prop.PropertyType.Name}());");
                    continue;
                }

                var rule = BuildRuleForProperty(prop);
                if (!string.IsNullOrEmpty(rule))
                {
                    sb.AppendLine($"            {rule}");
                }
                else
                {
                    sb.AppendLine($"            // {prop.Name}: unsupported type {GetTypeName(prop.PropertyType)}, left default");
                }
            }

            sb.AppendLine("        }");
            sb.AppendLine("    }");
            sb.AppendLine("}");
            return sb.ToString();
        }

        static string BuildRuleForProperty(PropertyInfo prop)
        {
            var name = prop.Name;
            var t = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;
            var faker = "f"; // Bogus faker variable

            if (t == typeof(string))
            {
                // heuristics based on property name
                var lname = name.ToLowerInvariant();
                if (lname.Contains("email")) return $"RuleFor(x => x.{name}, {faker} => {faker}.Internet.Email());";
                if (lname.Contains("first") && lname.Contains("name")) return $"RuleFor(x => x.{name}, {faker} => {faker}.Name.FirstName());";
                if (lname.Contains("last") && lname.Contains("name")) return $"RuleFor(x => x.{name}, {faker} => {faker}.Name.LastName());";
                if (lname.Contains("name")) return $"RuleFor(x => x.{name}, {faker} => {faker}.Name.FindName());";
                if (lname.Contains("url") || lname.Contains("uri")) return $"RuleFor(x => x.{name}, {faker} => {faker}.Internet.Url());";
                if (lname.Contains("phone")) return $"RuleFor(x => x.{name}, {faker} => {faker}.Phone.PhoneNumber());";
                return $"RuleFor(x => x.{name}, {faker} => {faker}.Lorem.Sentence());";
            }

            if (t == typeof(int) || t == typeof(short) || t == typeof(byte))
                return $"RuleFor(x => x.{name}, {faker} => {faker}.Random.Int(0, 1000));";

            if (t == typeof(long))
                return $"RuleFor(x => x.{name}, {faker} => {faker}.Random.Long(0, 1000000L));";

            if (t == typeof(double) || t == typeof(float))
                return $"RuleFor(x => x.{name}, {faker} => {faker}.Random.Double(0, 1000));";

            if (t == typeof(decimal))
                return $"RuleFor(x => x.{name}, {faker} => Convert.ToDecimal({faker}.Random.Double(0, 1000)));";

            if (t == typeof(bool))
                return $"RuleFor(x => x.{name}, {faker} => {faker}.Random.Bool());";

            if (t == typeof(DateTime))
                return $"RuleFor(x => x.{name}, {faker} => {faker}.Date.Past(5));";

            if (t == typeof(DateTimeOffset))
                return $"RuleFor(x => x.{name}, {faker} => {faker}.Date.PastOffset(5));";

            if (t == typeof(Guid))
                return $"RuleFor(x => x.{name}, {faker} => Guid.NewGuid());";

            if (t.IsEnum)
            {
                var enumTypeName = GetTypeName(t);
                return $"RuleFor(x => x.{name}, {faker} => {faker}.PickRandom<{enumTypeName}>());";
            }

            // Nullable enums handled above by unwrapping Nullable<>
            return null;
        }

        static bool IsCollection(Type t)
        {
            if (t == typeof(string)) return false;
            return typeof(IEnumerable).IsAssignableFrom(t) && t != typeof(byte[]);
        }

        static bool IsEntityType(Type t, Assembly entitiesAssembly)
        {
            // crude heuristic: same assembly as entities assembly and class type
            var nt = Nullable.GetUnderlyingType(t) ?? t;
            return nt.IsClass && nt.Assembly == entitiesAssembly && nt != typeof(string);
        }

        static string GetTypeName(Type t)
        {
            if (t.IsGenericType)
            {
                var generic = t.GetGenericTypeDefinition();
                var args = string.Join(", ", t.GetGenericArguments().Select(GetTypeName));
                var name = generic.Name;
                var idx = name.IndexOf('`');
                if (idx > 0) name = name.Substring(0, idx);
                return $"{name}<{args}>";
            }
            return string.IsNullOrEmpty(t.FullName) ? t.Name : t.FullName;
        }
    }
}