namespace FarmCraft.Core.Config
{
    /// <summary>
    /// Properties used for security related tasks in FarmCraft services,
    /// such as encryption and decryption
    /// </summary>
    public class SecuritySettings
    {
        /// <summary>
        /// An asymmetric public key that a web client can retrieve to 
        /// encrypt data
        /// </summary>
        public string PublicKey { get; set; }

        /// <summary>
        /// An asymmetric private key used by the API to decrypt data
        /// received from a web client
        /// </summary>
        public string PrivateKey { get; set; }
    }
}
