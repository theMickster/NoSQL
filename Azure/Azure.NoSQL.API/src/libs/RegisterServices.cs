﻿using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.OpenApi.Models;
using System.Reflection;

namespace Azure.NoSQL.API.libs;

[ExcludeFromCodeCoverage]
internal static class RegisterServices
{
    [SuppressMessage("Minor Code Smell", "S1075:URIs should not be hardcoded", Justification = "Because we said so.")]
    internal static WebApplicationBuilder RegisterAspDotNetServices(this WebApplicationBuilder builder)
    {
        // ******* Access the configuration manager *******
        var config = builder.Configuration;

        builder.Services.AddControllers(options =>
        {
            options.ReturnHttpNotAcceptable = true;
        })
            .AddJsonOptions(options => options.JsonSerializerOptions.WriteIndented = true)
            .AddXmlSerializerFormatters()
            .AddXmlDataContractSerializerFormatters();

        builder.Services.AddApiVersioning(options =>
        {
            options.ReportApiVersions = true;
            options.AssumeDefaultVersionWhenUnspecified = true;
            options.ApiVersionReader = new UrlSegmentApiVersionReader();
        });

        builder.Services.AddVersionedApiExplorer(options =>
        {
            options.GroupNameFormat = "'v'VVV";
            options.SubstituteApiVersionInUrl = true;
        });


        builder.Services.AddEndpointsApiExplorer();

        builder.Services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc(
                "v1",
                MakeOpenApiInfo("Adventure Works API",
                                "v1",
                                "API",
                                new Uri("http://hello-world.info")));

            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                In = ParameterLocation.Header,
                Description = "Please enter a valid token",
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                BearerFormat = "JWT",
                Scheme = "Bearer"
            });

            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type=ReferenceType.SecurityScheme,
                            Id="Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
            });

            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";

            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

            if (File.Exists(xmlPath))
            {
                options.IncludeXmlComments(xmlPath);
            }

            options.TagActionsBy(api =>
            {
                if (api.GroupName != null)
                {
                    return new[] { api.GroupName };
                }

                if (api.ActionDescriptor is ControllerActionDescriptor controllerActionDescriptor)
                {
                    return new[] { controllerActionDescriptor.ControllerName };
                }

                throw new InvalidOperationException("Unable to determine tag for endpoint.");
            });

            options.DocInclusionPredicate((name, api) => true);
        });

        //builder.Services.AddAutoMapper(
        //    new[]
        //    {
        //        typeof(AddressEntityToAddressModelProfile).GetTypeInfo().Assembly
        //    });

        //builder.Services.AddValidatorsFromAssemblyContaining<CreateAddressValidator>();

        return builder;
    }

    #region Private Methods

    private static OpenApiInfo MakeOpenApiInfo(string title, string version, string description, Uri releaseNotes)
    {
        var oai = new OpenApiInfo
        {
            Title = title,
            Version = version,
            Contact = new OpenApiContact
            { Email = "bug.bashing.anonymous@outlook.com", Name = "Bug Bashing Anonymous" },
            Description = description
        };

        if (releaseNotes != null)
        {
            oai.Contact.Url = releaseNotes;
        }

        return oai;
    }

    //private static List<DatabaseConnectionString> GetDatabaseConnectionStrings(IConfiguration configuration)
    //{
    //    var defaultConnectionString =
    //        configuration.GetConnectionString(ConfigurationConstants.SqlConnectionDefaultConnectionName);

    //    var sqlAzureConnectionString =
    //        configuration.GetConnectionString(ConfigurationConstants.SqlConnectionSqlAzureConnectionName);

    //    if (string.IsNullOrWhiteSpace(defaultConnectionString))
    //    {
    //        throw new ConfigurationException(
    //            $"The required Configuration value for {ConfigurationConstants.SqlConnectionDefaultConnectionName} is missing." +
    //            "Please verify database configuration.");
    //    }

    //    var connectionStrings = new List<DatabaseConnectionString>
    //    {
    //        new()
    //        {
    //            ConnectionStringName = ConfigurationConstants.SqlConnectionDefaultConnectionName,
    //            ConnectionString = defaultConnectionString
    //        }
    //    };

    //    if (!string.IsNullOrWhiteSpace(sqlAzureConnectionString))
    //    {
    //        connectionStrings.Add(new DatabaseConnectionString
    //        {
    //            ConnectionStringName = ConfigurationConstants.SqlConnectionSqlAzureConnectionName,
    //            ConnectionString = sqlAzureConnectionString
    //        });
    //    }

    //    return connectionStrings;
    //}

    //private static string GetSqlConnectionString(IConfiguration configuration, IEnumerable<DatabaseConnectionString> connectionStrings)
    //{
    //    var settings = configuration.GetSection(EntityFrameworkCoreSettings.SettingsRootName);

    //    if (settings == null)
    //    {
    //        throw new ConfigurationException(
    //            $"The required Configuration settings keys for the Entity Framework Core Settings are missing." +
    //            "Please verify configuration.");
    //    }

    //    var connectionStringName = settings[ConfigurationConstants.CurrentConnectionStringNameKey] ??
    //                               ConfigurationConstants.SqlConnectionDefaultConnectionName;

    //    var currentConnectionString = connectionStrings.FirstOrDefault(x =>
    //        x.ConnectionStringName == connectionStringName);

    //    if (currentConnectionString == null)
    //    {
    //        throw new ConfigurationException(
    //            $"The required Configuration settings keys for the Entity Framework Core Settings are missing." +
    //            "Please verify configuration.");
    //    }

    //    return currentConnectionString.ConnectionString;
    //}

    #endregion Private Methods
}