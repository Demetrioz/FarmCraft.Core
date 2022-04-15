namespace FarmCraft.Core.Config
{
    /// <summary>
    /// Properties that reside in the appsettings.json and are used 
    /// during setup or operation of FarmCraft services
    /// </summary>
    public class FarmCraftSettings
    {
        /// <summary>
        /// A list of connection strings used to establish connections with other
        /// services
        /// </summary>
        public Dictionary<string, string> ConnectionStrings { get; set; }

        /// <summary>
        /// A list of hosts that can be allowed to make requests to the FarmCraft 
        /// Service's API
        /// </summary>
        public string[] CORS { get; set; }

        /// <summary>
        /// Settings used for generating JWTs
        /// </summary>
        public JwtSettings JwtSettings { get; set; }

        /// <summary>
        /// Settings used to for  security related tasks within FarmCraft services
        /// </summary>
        public SecuritySettings SecuritySettings { get; set; }
    }
}