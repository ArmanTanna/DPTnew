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
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int UserId { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Language { get; set; }
        public int Status { get; set; }
        public string EmailStatus { get; set; }
        public string AccountNumber { get; set; }
        public string PrimaryContact { get; set; }
        public int RoleId { get; set; }
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
