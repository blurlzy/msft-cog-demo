
using System.Globalization;

namespace Msft.Demo.Console
{
    internal record EntraIdAuthConfig
    {
        public required string TenantId { get; init; }
        public required string ClientId { get; init; }
        public required string ClientSecret { get; init; }

        /// <summary>
        /// URL of the authority
        /// </summary>
        public string Authority
        {
            get
            {
                return String.Format(CultureInfo.InvariantCulture, "https://login.microsoftonline.com/{0}", TenantId);
            }
        }
    }

    // scope
    internal static class EntraIdScopes
    {
        // default scope for Azure Rest Api
        public const string RestApi = "https://management.azure.com/.default";
    }
}
