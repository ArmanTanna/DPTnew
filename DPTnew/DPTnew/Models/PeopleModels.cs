using DPTnew.Localization;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Globalization;
using System.Web.Security;

namespace DPTnew.Models
{


    [Table("DPT_PeopleWithRoles")]
    public class People
    {
        [Key]
        [Display(Name = "UserId", ResourceType = typeof(Resource))]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int UserId { get; set; }
        [Display(Name = "Email", ResourceType = typeof(Resource))]
        public string Email { get; set; }
        [Display(Name = "FirstName", ResourceType = typeof(Resource))]
        public string FirstName { get; set; }
        [Display(Name = "LastName", ResourceType = typeof(Resource))]
        public string LastName { get; set; }
        [Display(Name = "Language", ResourceType = typeof(Resource))]
        public string Language { get; set; }
        [Display(Name = "Status", ResourceType = typeof(Resource))]
        public int Status { get; set; }
        [Display(Name = "EmailStatus", ResourceType = typeof(Resource))]
        public string EmailStatus { get; set; }
        [Display(Name = "AccountNumber", ResourceType = typeof(Resource))]
        public string AccountNumber { get; set; }
        [Display(Name = "AccountName", ResourceType = typeof(Resource))]
        public string AccountName { get; set; }
        [Display(Name = "PrimaryContact", ResourceType = typeof(Resource))]
        public string PrimaryContact { get; set; }
        public string FirstNameK { get; set; }
        public string LastNameK { get; set; }
        public int RoleId { get; set; }
        [Display(Name = "RoleName", ResourceType = typeof(Resource))]
        public string RoleName { get; set; }
    }

    [Table("webpages_UsersInRoles")]
    public class PeopleRoles
    {
        [Key]
        [Column(Order = 0)]
        public int UserId { get; set; }
        [Key]
        [Column(Order = 1)]
        public int RoleId { get; set; }
    }

    [Table("webpages_Roles")]
    public class MainWebRoles
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int RoleId { get; set; }
        public string RoleName { get; set; }
    }
}
