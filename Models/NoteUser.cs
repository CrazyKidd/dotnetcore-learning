using System;
using Microsoft.AspNetCore.Identity;

namespace NetNote.Models
{
    public class NoteUser : IdentityUser
    {
        public string Picture { get; set; }
        public string Status { get; set; }
    }
}
