using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace NetNote.ViewModel
{
    public class NoteModel
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
        public int Type { get; set; }

        [Display(Name = "密码")]
        public string Password { get; set; }
        [Display(Name = "附件")]
        public IFormFile Attachment { get; set; }
    }
}
