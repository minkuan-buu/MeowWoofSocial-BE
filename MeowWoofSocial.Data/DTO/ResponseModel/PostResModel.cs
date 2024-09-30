using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeowWoofSocial.Data.DTO.ResponseModel
{
    public class PostResModel
    {
    }

    public class PostAuthorResModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Avatar { get; set; }
    }

    public class PostCreateResModel
    {
        public Guid Id { get; set; }
        public PostAuthorResModel Author { get; set; } = null!;
        public string Content { get; set; } = null!;
        public List<PostAttachmentResModel> Attachments { get; set; } = new();
        public List<PostHashtagResModel> Hashtags { get; set; } = new();
        public DateTime CreateAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class PostAttachmentResModel
    {
        public Guid Id { get; set; }
        public string? Attachment { get; set; }
    }

    public class PostHashtagResModel
    {
        public Guid Id { get; set; }
        public string HashTag { get; set; } = null!;
    }
}
