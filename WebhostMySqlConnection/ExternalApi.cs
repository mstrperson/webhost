//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace WebhostMySQLConnection
{
    using System;
    using System.Collections.Generic;
    
    public partial class ExternalApi
    {
        public ExternalApi()
        {
            this.ApiRequestHeaders = new HashSet<ApiRequestHeader>();
            this.ServiceAccounts = new HashSet<ServiceAccount>();
        }
    
        public int id { get; set; }
        public string service { get; set; }
        public string BaseUrl { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string Info { get; set; }
    
        public virtual ICollection<ApiRequestHeader> ApiRequestHeaders { get; set; }
        public virtual ICollection<ServiceAccount> ServiceAccounts { get; set; }
    }
}
