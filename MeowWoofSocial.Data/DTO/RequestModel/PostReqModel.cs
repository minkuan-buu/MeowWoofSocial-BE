using Microsoft.AspNetCore.Http;

namespace MeowWoofSocial.Data.DTO.RequestModel
{
    public class PostReqModel
    {
    }

    public class PostCreateReqModel
    {
        public string Content { get; set; } = null!;
        public List<IFormFile>? Attachment { get; set; }
        public string[]? HashTag { get; set; }
    }

    public class PostAttachmentReqModel
    {
        public Guid Id { get; set; }
        public Guid PostId { get; set; }
        public string Attachment { get; set; } = null!;
    }

    public class PostHashtagReqModel
    {
        public Guid Id { get; set; }
        public Guid PostId { get; set; }
        public string HashTag { get; set; } = null!;
    }

}
