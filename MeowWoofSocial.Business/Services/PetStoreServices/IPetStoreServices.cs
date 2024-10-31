using MeowWoofSocial.Data.DTO.RequestModel;
using MeowWoofSocial.Data.DTO.ResponseModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeowWoofSocial.Business.Services.PetStoreServices
{
    public interface IPetStoreServices
    {
        Task<DataResultModel<PetStoreCreateResModel>> CreatePetStore(PetStoreCreateReqModel petStore, string token);
        Task<DataResultModel<PetStoreUpdateResModel>> UpdatePetStore(PetStoreUpdateReqModel petStore, string token);
        Task<DataResultModel<PetStoreDeleteResModel>> DeletePetStore(PetStoreDeleteReqModel PetStoreDeleteReq, string token);
    }
}
