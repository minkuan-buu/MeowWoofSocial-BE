using MeowWoofSocial.Data.DTO.RequestModel;
using MeowWoofSocial.Data.DTO.ResponseModel;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeowWoofSocial.Business.Services.PostServices
{
    public interface IPostServices
    {
        Task<DataResultModel<PostCreateResModel>> CreatePost(PostCreateReqModel post, string token);
        Task<ListDataResultModel<PostDetailResModel>> GetNewsFeed(string token, NewsFeedReq newsFeedReq);
        Task<DataResultModel<PostUpdateResModel>> UpdatePost(PostUpdateReqModel postUpdateReq, string token);
        Task<DataResultModel<PostDetailResModel>> GetPostByID(Guid postId, string token);
        Task<DataResultModel<PostRemoveResModel>> RemovePost(PostRemoveReqModel postRemoveReq, string token);

    }
}
