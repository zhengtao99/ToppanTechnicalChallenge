﻿//------------------------------------------------------------------------------
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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Data.Entity.Core.Objects;
    using System.Linq;
    
    public partial class ToppanTechnicalChallengeDBEntities : DbContext
    {
        public ToppanTechnicalChallengeDBEntities()
            : base("name=ToppanTechnicalChallengeDBEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<Country> Countries { get; set; }
        public virtual DbSet<University> Universities { get; set; }
        public virtual DbSet<UniversityBookmark> UniversityBookmarks { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<UsersUniversity> UsersUniversities { get; set; }
    
        public virtual int SP_CreateUniversity(Nullable<int> userId, Nullable<int> countryId, string name, string webpages, Nullable<System.DateTime> createdAt)
        {
            var userIdParameter = userId.HasValue ?
                new ObjectParameter("userId", userId) :
                new ObjectParameter("userId", typeof(int));
    
            var countryIdParameter = countryId.HasValue ?
                new ObjectParameter("countryId", countryId) :
                new ObjectParameter("countryId", typeof(int));
    
            var nameParameter = name != null ?
                new ObjectParameter("name", name) :
                new ObjectParameter("name", typeof(string));
    
            var webpagesParameter = webpages != null ?
                new ObjectParameter("webpages", webpages) :
                new ObjectParameter("webpages", typeof(string));
    
            var createdAtParameter = createdAt.HasValue ?
                new ObjectParameter("createdAt", createdAt) :
                new ObjectParameter("createdAt", typeof(System.DateTime));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("SP_CreateUniversity", userIdParameter, countryIdParameter, nameParameter, webpagesParameter, createdAtParameter);
        }
    
        public virtual ObjectResult<SP_GetUniversityById_Result> SP_GetUniversityById(Nullable<int> universityId, string username)
        {
            var universityIdParameter = universityId.HasValue ?
                new ObjectParameter("UniversityId", universityId) :
                new ObjectParameter("UniversityId", typeof(int));
    
            var usernameParameter = username != null ?
                new ObjectParameter("username", username) :
                new ObjectParameter("username", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<SP_GetUniversityById_Result>("SP_GetUniversityById", universityIdParameter, usernameParameter);
        }
    
        public virtual int SP_UpdateUniversityById(Nullable<int> universityId, string username, string universityName, string webpages, string countryName, Nullable<System.DateTime> modifiedDate)
        {
            var universityIdParameter = universityId.HasValue ?
                new ObjectParameter("universityId", universityId) :
                new ObjectParameter("universityId", typeof(int));
    
            var usernameParameter = username != null ?
                new ObjectParameter("username", username) :
                new ObjectParameter("username", typeof(string));
    
            var universityNameParameter = universityName != null ?
                new ObjectParameter("universityName", universityName) :
                new ObjectParameter("universityName", typeof(string));
    
            var webpagesParameter = webpages != null ?
                new ObjectParameter("webpages", webpages) :
                new ObjectParameter("webpages", typeof(string));
    
            var countryNameParameter = countryName != null ?
                new ObjectParameter("countryName", countryName) :
                new ObjectParameter("countryName", typeof(string));
    
            var modifiedDateParameter = modifiedDate.HasValue ?
                new ObjectParameter("modifiedDate", modifiedDate) :
                new ObjectParameter("modifiedDate", typeof(System.DateTime));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("SP_UpdateUniversityById", universityIdParameter, usernameParameter, universityNameParameter, webpagesParameter, countryNameParameter, modifiedDateParameter);
        }
    }
}