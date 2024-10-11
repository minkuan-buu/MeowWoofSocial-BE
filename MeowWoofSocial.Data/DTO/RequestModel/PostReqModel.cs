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

    public class NewsFeedReq
    {
        public int PageSize { get; set; }
        public Guid? lastPostId { get; set; } = null;
    }

    public class PostUpdateReqModel
    {
        public Guid Id { get; set; }
        public string Content { get; set; }
        public List<IFormFile>? Attachments { get; set; }
        public string[]? HashTag { get; set; }
    }

    public class CommentCreateReqModel
    {
        public Guid PostId { get; set; }
        public string? Content { get; set; }
        public IFormFile? Attachment { get; set; }
    }

    public class CommentUpdateReqModel
    {
        public Guid Id { get; set; }
        public string? Content { get; set; }
        public IFormFile? Attachment { get; set; }
    }

    public class FeelingCreateReqModel
    {
        public Guid PostId { get; set; }
        public string? TypeReact { get; set; }
    }

    public class PostRemoveReqModel
    {
        public Guid PostId { get; set; }
    }

    public class CommentDeleteReqModel
    {
        public Guid CommentId { get; set; }
    }

}
