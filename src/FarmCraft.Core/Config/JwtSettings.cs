namespace FarmCraft.Core.Config
{
    /// <summary>
    /// Properties used to generate JWTs when authenticating
    /// with a FarmCraft service's API
    /// </summary>
    public class JwtSettings
    {
        /// <summary>
        /// The Issuer of the token
        /// </summary>
        public string Issuer { get; set; }

        /// <summary>
        /// The audience of the token
        /// </summary>
        public string Audience { get; set; }

        /// <summary>
        /// A secret key used to generate signing
        /// credentials
        /// </summary>
        public string SecretKey { get; set; }
    }
}
