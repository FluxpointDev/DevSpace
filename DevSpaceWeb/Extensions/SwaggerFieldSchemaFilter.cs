using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;
using System.Text.Json;

namespace DevSpaceWeb.Extensions;

/// <summary>
/// Allows for swagger to generate json models/examples without get setters.
/// </summary>
public class SwaggerFieldSchemaFilter : ISchemaFilter
{

    private readonly JsonSerializerOptions _jsonOptions;
    private readonly IServiceProvider _provider;

    public SwaggerFieldSchemaFilter(IServiceProvider provider)
    {
        _provider = provider;
        _jsonOptions = GetJsonOptions(provider);
    }

    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        if (schema.Type != "object")
        {
            return;
        }

        if (!_jsonOptions.IncludeFields)
        {
            return;
        }

        // This needs to be lazily evaluated, or it will cause infinite recursion
        SchemaGeneratorOptions generatorOptions = _provider.GetRequiredService<SchemaGeneratorOptions>();

        Type type = context.Type;
        FieldInfo[] fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance);

        foreach (FieldInfo field in fields)
        {
            if (field.IsInitOnly && _jsonOptions.IgnoreReadOnlyFields)
            {
                continue;
            }

            Type fieldType = field.FieldType;
            if (schema.Properties.ContainsKey(field.Name))
            {
                continue;
            }

            string name = _jsonOptions.PropertyNamingPolicy?.ConvertName(field.Name) ?? field.Name;
            schema.Properties[name] = context.SchemaGenerator.GenerateSchema(fieldType, context.SchemaRepository);

            if (generatorOptions.NonNullableReferenceTypesAsRequired && field.IsNonNullableReferenceType())
            {
                schema.Required.Add(name);
            }
        }
    }

    private static JsonSerializerOptions GetJsonOptions(IServiceProvider provider)
    {
        // Copied from https://github.com/domaindrivendev/Swashbuckle.AspNetCore/blob/340eef3e812cc012a16c70e183d748aebf3a898e/src/Swashbuckle.AspNetCore.SwaggerGen/DependencyInjection/SwaggerGenServiceCollectionExtensions.cs#L69
#if !NETSTANDARD2_0
        return provider.GetService<IOptions<Microsoft.AspNetCore.Mvc.JsonOptions>>()?.Value?.JsonSerializerOptions
#if NET8_0_OR_GREATER
               ?? provider.GetService<IOptions<Microsoft.AspNetCore.Http.Json.JsonOptions>>()?.Value?.SerializerOptions
#endif
#if NET7_0_OR_GREATER
               ?? JsonSerializerOptions.Default;
#else
                    ?? new JsonSerializerOptions();
#endif
#else
                serializerOptions = new JsonSerializerOptions();
#endif
    }

}
