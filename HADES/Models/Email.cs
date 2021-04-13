using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace HADES.Models
{
    [Table("Email_EMA")]
    public class Email
    {
        [Column("EMA_id")]
        public int Id { get; set; }

        [Column("EMA_email")]
        public string Address { get; set; }

        [Column("EMA_expiration_date")]
        public bool ExpirationDate { get; set; }

        [Column("EMA_group_create")]
        public bool GroupCreate { get; set; }

        [Column("EMA_group_delete")]
        public bool GroupDelete { get; set; }

        [Column("EMA_member_add")]
        public bool GroupAdd { get; set; }

        [Column("EMA_member_add")]
        public bool MemberAdd { get; set; }

        [Column("EMA_member_remove")]
        public bool MemberRemoval { get; set; }

        [ForeignKey("EMA_UCF_id")]
        public virtual UserConfig UserConfig { get; set; }
    }
}