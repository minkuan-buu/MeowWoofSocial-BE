using MeowWoofSocial.Data.DTO.RequestModel;
using MeowWoofSocial.Data.DTO.ResponseModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeowWoofSocial.Business.Services.ReactionServices
{
    public interface IPostReactionServices
    {
        Task<DataResultModel<CommentCreatePostResModel>> CreateComment(CommentCreateReqModel commentReq, string token);
        Task<DataResultModel<FeelingCreatePostResModel>> CreateFeeling(FeelingCreateReqModel feelingReq, string token);
        Task<DataResultModel<FeelingCreatePostResModel>> UpdateFeeling(FeelingCreateReqModel feelingReq, string token);
        Task<DataResultModel<CommentUpdateResModel>> UpdateComment(CommentUpdateReqModel commentUpdateReq, string token);
    }
}
