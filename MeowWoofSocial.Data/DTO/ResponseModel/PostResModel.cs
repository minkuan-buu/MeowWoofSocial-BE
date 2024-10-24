using Microsoft.AspNetCore.Http;
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
        public bool isFollow {  get; set; }
    }

    public class PostCreateResModel
    {
        public Guid Id { get; set; }
        public PostAuthorResModel Author { get; set; } = null!;
        public string Content { get; set; } = null!;
        public List<PostAttachmentResModel> Attachments { get; set; } = new();
        public List<PostHashtagResModel> Hashtags { get; set; } = new();
        public List<FeelingPostResModel> Feeling { get; set; } = new();
        public List<CommentPostResModel> Comment { get; set; } = new();
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
        public string? Hashtag { get; set; } 
    }

    public class PostDetailResModel
    {
        public Guid Id { get; set; }
        public PostAuthorResModel Author { get; set; }
        public string Content { get; set; } = null!;
        public List<PostAttachmentResModel> Attachments { get; set; } = new();
        public List<PostHashtagResModel> Hashtags { get; set; } = new();
        public string Status { get; set; } = null!;
        public List<FeelingPostResModel> Feeling { get; set; } = new();
        public List<CommentPostResModel> Comment { get; set; } = new();
        public DateTime CreateAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class FeelingPostResModel
    {
        public Guid Id { get; set; }
        public string TypeReact { get; set; } = null!;
        public PostAuthorResModel Author { get; set; }
    }

    public class CommentPostResModel
    {
        public Guid Id { get; set; }
        public string Content { get; set; } = null!;
        public string? Attachment { get; set; }
        public PostAuthorResModel Author { get; set; }
        public DateTime CreateAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class ReactionListResModel
    {
        public Guid Id { get; set; }
        public ReactionAuthorModel Author { get; set; }
        public string Type { get; set; }
        public string? Content { get; set; }
        public string? Attachment { get; set; }
        public string? TypeReact { get; set; }
        public DateTime CreateAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class ReactionAuthorModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
    }

    public class UserFollowingResModel
    {
        public Guid UserId { get; set; }
        public Guid FollowerId { get; set; }
    }

    public class PostUpdateResModel
    {
        public Guid Id { get; set; }
        public PostAuthorResModel Author { get; set; }
        public string Content { get; set; } = null!;
        public List<PostAttachmentResModel> Attachments { get; set; } = new();
        public List<PostHashtagResModel> Hashtags { get; set; } = new();
        public string Status { get; set; } = null!;
        public List<FeelingPostResModel> Feeling { get; set; } = new();
        public List<CommentPostResModel> Comment { get; set; } = new();
        public DateTime CreateAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class CommentCreatePostResModel
    {   
        public Guid Id { get; set; }
        public string Content { get; set; } = null!;
        public string? Attachment { get; set; } 
        public PostAuthorResModel Author { get; set; }
        public DateTime CreateAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class CommentUpdateResModel
    {
        public Guid Id { get; set; }
        public string Content { get; set; } = null!;
        public string? Attachment { get; set; }
        public PostAuthorResModel Author { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class FeelingCreatePostResModel
    {
        public Guid Id { get; set; }
        public string TypeReact { get; set; } = null!;
        public PostAuthorResModel Author { get; set; }
        public DateTime CreateAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class PostRemoveResModel
    {
        public Guid PostId { get; set; }
        public string Status { get; set; }
        public DateTime UpdateAt { get; set; }
    }
    public class CommentDeleteResModel
    {
        public Guid Id { get; set; }
    }

}
