//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ApiServer
{
    using System;
    using System.Collections.Generic;
    
    public partial class University
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Nullable<int> CountryId { get; set; }
        public string Webpages { get; set; }
        public System.DateTime CreatedAt { get; set; }
        public Nullable<System.DateTime> LastModified { get; set; }
        public bool IsActive { get; set; }
        public Nullable<System.DateTime> DeletedAt { get; set; }
        public bool IsBookmarked { get; set; }
    
        public virtual Country Country { get; set; }
        public virtual UsersUniversity UsersUniversity { get; set; }
    }
}