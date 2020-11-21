using System;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace NetNote.Models
{
    public class NoteContext : IdentityDbContext<NoteUser>
    {
        public NoteContext(DbContextOptions<NoteContext> options) : base(options)
        {

        }

        public DbSet<Note> Notes { get; set; }
        public DbSet<NoteType> NoteTypes { get; set; }

    }
}
