namespace Ameex.Umbraco.Forms.Integrations.Crm.Salesforce.Common.Models
{
    public class AuthenticationRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Grant_type { get; set; }
        public string Client_id { get; set; }
        public string Client_secret { get; set; }

    }
}
