using AutoMapper;
using MeowWoofSocial.Business.ApplicationMiddleware;
using MeowWoofSocial.Business.Services.CloudServices;
using MeowWoofSocial.Data.DTO.Custom;
using MeowWoofSocial.Data.DTO.RequestModel;
using MeowWoofSocial.Data.DTO.ResponseModel;
using MeowWoofSocial.Data.Entities;
using MeowWoofSocial.Data.Enums;
using MeowWoofSocial.Data.Repositories.UserFollowingRepositories;
using MeowWoofSocial.Data.Repositories.UserPetRepositories;
using MeowWoofSocial.Data.Repositories.UserRepositories;


namespace MeowWoofSocial.Business.Services.UserPetServices
{
    public class UserPetServices : IUserPetServices
    {

        private readonly IUserPetRepositories _userPetRepo;
        private readonly IMapper _mapper;
        private readonly ICloudStorage _cloudStorage;
        private readonly IUserRepositories _userRepo;

        public UserPetServices(IUserPetRepositories userPetRepo, IMapper mapper, ICloudStorage cloudStorage, IUserRepositories userRepo)
        {
            _userPetRepo = userPetRepo;
            _mapper = mapper;
            _cloudStorage = cloudStorage;
            _userRepo = userRepo;
        }
        
        public async Task<DataResultModel<UserPetCreateResMdoel>> CreateUserPet(UserPetCreateReqMdoel uerPetReq, string token)
        {
            var result = new DataResultModel<UserPetCreateResMdoel>();
            try
            {
                Guid userId = new Guid(Authentication.DecodeToken(token, "userid"));
                var user = await _userRepo.GetSingle(x => x.Id == userId);

                if (user == null || user.Status.Equals(AccountStatusEnums.Inactive))
                {
                    throw new CustomException("You are banned from creating pet due to violate of terms!");
                }

                var newUserPetId = Guid.NewGuid();
                var userPetEntity = _mapper.Map<UserPet>(uerPetReq);
                userPetEntity.Id = newUserPetId;
                userPetEntity.UserId = userId;
                userPetEntity.Name = TextConvert.ConvertToUnicodeEscape(uerPetReq.Name);
                userPetEntity.Type = TextConvert.ConvertToUnicodeEscape(uerPetReq.Type);
                userPetEntity.Breed = TextConvert.ConvertToUnicodeEscape(uerPetReq.Breed);
                userPetEntity.Age = TextConvert.ConvertToUnicodeEscape(uerPetReq.Age);
                userPetEntity.Gender = TextConvert.ConvertToUnicodeEscape(uerPetReq.Gender);
                userPetEntity.Weight = uerPetReq.Weight;
                string filePath = $"user/{userId}/pet/{newUserPetId}/attachment";
                if (uerPetReq.Attachment != null)
                {
                    var attachments = await _cloudStorage.UploadSingleFile(uerPetReq.Attachment, filePath);
                    userPetEntity.Attachment = attachments;
                }
                userPetEntity.CreateAt = DateTime.Now;

                await _userPetRepo.Insert(userPetEntity);
                result.Data = _mapper.Map<UserPetCreateResMdoel>(userPetEntity);
            }
            catch (Exception ex)
            {
                throw new CustomException($"An error occurred: {ex.Message}");
            }
            return result;
        }
    }
}