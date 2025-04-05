using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;

namespace DevSpaceWeb.Extensions;

//[AttributeUsage(AttributeTargets.All, Inherited = true, AllowMultiple = false)]
//public class SwaggerNoAuthAttribute : Attribute
//{

//}

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
public class ShowInSwaggerAttribute : Attribute
{

}

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
public class ListParameterSwaggerAttribute : Attribute
{
    public ListParameterSwaggerAttribute(string name, string[] list)
    {
        Name = name;
        List = list;
    }

    public string Name;
    public string[] List;
}

//[AttributeUsage(AttributeTargets.All, Inherited = true, AllowMultiple = false)]
//public class SwaggerImageGenOutputttribute : Attribute
//{

//}


//[AttributeUsage(AttributeTargets.All, Inherited = true, AllowMultiple = false)]
//public class SwaggerListTagsAttribute : Attribute
//{
//    public SwaggerListTagsAttribute(ListType type)
//    {
//        Type = type;
//    }

//    public ListType Type;
//}

//public enum ListType
//{
//    ImageGenTemplates
//}


//[AttributeUsage(AttributeTargets.All, Inherited = true, AllowMultiple = false)]
//public class SwaggerRequestBodyAttribute : Attribute
//{
//    public SwaggerRequestBodyAttribute(Type type, string description, bool required = true, bool allowPlain = false)
//    {
//        Type = type;
//        Description = description;
//        Required = required;
//        AllowPlain = allowPlain;
//    }

//    public Type Type;
//    public string Description;
//    public bool Required;
//    public bool AllowPlain;
//}


public class SwaggerCheckAuthFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        if (context.MethodInfo.DeclaringType is null)
            return;

        //bool hasNoAuthorize = !context.MethodInfo.GetCustomAttributes<SwaggerNoAuthAttribute>(true).Any();
        //if (hasNoAuthorize)
        //    operation.Security.Clear();
        foreach (KeyValuePair<string, OpenApiResponse> i in operation.Responses)
        {
            i.Value.Content.Remove("text/plain");
            i.Value.Content.Remove("text/json");
        }

        if (operation.RequestBody != null)
        {
            operation.RequestBody.Content.Remove("text/json");
            operation.RequestBody.Content.Remove("application/*+json");
        }



        //if (hasAuthorize)
        //{
        //if (context.DocumentName == "gitbooks")
        //{
        //    if (operation.Parameters == null)
        //        operation.Parameters = new List<OpenApiParameter>();
        //    operation.Parameters.Add(new OpenApiParameter
        //    {
        //        In = ParameterLocation.Header,
        //        Name = "Authorization",
        //        Required = true,

        //    });
        //}


        //SwaggerListTagsAttribute? list = context.MethodInfo.GetCustomAttribute<SwaggerListTagsAttribute>(false);
        //if (list != null)
        //{
        //switch (list.Type)
        //{
        //case ListType.ImageGenTemplates:
        //operation.Parameters.First().Schema = context.SchemaGenerator.GenerateSchema(typeof(string), context.SchemaRepository);

        //OpenApiString[] List = _DB.Templates.Where(x => !x.Value.Owner).Select(x => new OpenApiString(x.Key)).ToArray();
        //operation.Parameters.First().Schema.Enum = List;
        //break;
        //case ListType.MinecraftSkinType:
        //    operation.Parameters.First().Schema = context.SchemaGenerator.GenerateSchema(typeof(string), context.SchemaRepository);

        //    operation.Parameters.First().Schema.Enum = new OpenApiString[]
        //    {
        //        new OpenApiString("full"),
        //        new OpenApiString("head"),
        //        new OpenApiString("head"),
        //        new OpenApiString("cube"),
        //        new OpenApiString("body"),
        //        new OpenApiString("all")
        //    };
        //    break;
        //}
        //}

        //if (context.MethodInfo.GetCustomAttribute<SwaggerImageGenOutputttribute>(false) != null)
        //{
        //    operation.Parameters.First(x => x.Name == "output").Schema.Enum = new OpenApiString[]
        //    {
        //        new OpenApiString("jpg"),
        //        new OpenApiString("png"),
        //        new OpenApiString("webp")
        //    };
        //}

        ListParameterSwaggerAttribute? List = context.MethodInfo.GetCustomAttribute<ListParameterSwaggerAttribute>();
        if (List != null)
        {
            OpenApiParameter? Parameter = operation.Parameters.FirstOrDefault(x => x.Name == List.Name);
            if (Parameter != null)
                Parameter.Schema.Enum = List.List.Select(x => new OpenApiString(x)).ToArray();
        }

        //SwaggerRequestBodyAttribute? body = context.MethodInfo.GetCustomAttribute<SwaggerRequestBodyAttribute>(false);
        //if (body != null)
        //{
        //    operation.RequestBody = new OpenApiRequestBody() { Description = body.Description, Required = body.Required };
        //    operation.RequestBody.Content.TryAdd(MediaTypeNames.Application.Json, new OpenApiMediaType
        //    {
        //        Schema = context.SchemaGenerator.GenerateSchema(body.Type, context.SchemaRepository),
        //        Encoding = new Dictionary<string, OpenApiEncoding>
        //        {
        //            { MediaTypeNames.Application.Json, new OpenApiEncoding { ContentType = MediaTypeNames.Application.Json } }
        //        }
        //    });

        //    if (body.AllowPlain)
        //    {
        //        operation.RequestBody.Content.TryAdd(MediaTypeNames.Text.Plain, new OpenApiMediaType
        //        {
        //            Schema = context.SchemaGenerator.GenerateSchema(typeof(string), context.SchemaRepository),
        //            Encoding = new Dictionary<string, OpenApiEncoding>
        //        {
        //            { MediaTypeNames.Text.Plain, new OpenApiEncoding { ContentType = MediaTypeNames.Text.Plain } }
        //        }
        //        });
        //    }
        //}


        OpenApiSecurityScheme security = new OpenApiSecurityScheme
        {
            Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "key" }
        };
        operation.Security = new List<OpenApiSecurityRequirement>
            {

                new OpenApiSecurityRequirement
                {
                    [security] = Array.Empty<string>()
                }
            };

    }
}
