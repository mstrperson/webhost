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
    
    public partial class RegistrationRequest
    {
        public int id { get; set; }
        public int StudentId { get; set; }
        public string MacAddress { get; set; }
        public string DeviceType { get; set; }
        public bool RequestCompleted { get; set; }
        public bool RequestDenied { get; set; }
    
        public virtual Student Student { get; set; }
    }
}
