using System;
using System.ComponentModel.DataAnnotations;

namespace NetNote.ViewModel
{
    public class NoteViewModel
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "标题")]
        [MaxLength(100)]
        public string Title { get; set; }

        [Required]
        [Display(Name = "内容")]
        public string Content { get; set; }

        [Display(Name = "类型")]
        public string Type { get; set; }

        [Display(Name = "附件")]
        public string Attachment { get; set; }
    }
}
