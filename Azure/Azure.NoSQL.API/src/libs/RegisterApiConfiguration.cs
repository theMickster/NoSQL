using System.Diagnostics.CodeAnalysis;
using Azure.NoSQL.API.Helpers;
using Azure.NoSQL.API.libs.Exceptions;

namespace Azure.NoSQL.API.libs;

[ExcludeFromCodeCoverage]
internal static class RegisterApiConfiguration
{
    internal static void RegisterApplicationConfiguration(this IConfiguration configuration)
    {
        var akvClient = AzureKeyVaultDataHelper.GetAzureKeyVaultClient(configuration);

        if (akvClient == null)
        {
            throw new ConfigurationException(
                "Unable to create an Azure Key Vault secret helper. " +
                "A properly configured helper is required. " +
                "Please verify local or Azure resource configuration. ");
        }

        SecretHelper.SecretClient = akvClient;
    }
}
