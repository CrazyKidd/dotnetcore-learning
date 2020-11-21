using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
namespace NetNote.Models
{
    public class NoteType
    {
        public int id{get;set;}

        [Required]
        [MaxLength(50)]
        public string Name{get;set;}

        public List<Note> Notes{get;set;}
    }
}
