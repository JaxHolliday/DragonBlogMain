using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace DragonBlog.Models
{
    public class BlogUser : IdentityUser
    {
        [Required]
        [StringLength(50)]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [StringLength(50)]
        [Display(Name = "Display Name")]
        public string DisplayName { get; set; }

        public ICollection<Comment> Comments { get; set; } /*= new HashSet<Comment>();*/

        public BlogUser()
        {
            Comments = new HashSet<Comment>();
            DisplayName = "New User";
        }
    }
}
