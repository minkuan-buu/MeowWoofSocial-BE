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

        public async Task<DataResultModel<UserPetUpdateResMdoel>> UpdateUserPet(UserPetUpdateReqMdoel userPetUpdateReq,
            string token)
        {
            var result = new DataResultModel<UserPetUpdateResMdoel>();
            try
            {
                Guid userId = new Guid(Authentication.DecodeToken(token, "userid"));
                var userPet = await _userPetRepo.GetSingle(x => x.Id == userPetUpdateReq.Id && x.UserId == userId);

                if (userPetUpdateReq == null)
                {
                    throw new CustomException("Pet not found or you do not have permission to update this Pet Store");
                }
                if (!string.IsNullOrEmpty(userPet.Attachment))
                {
                    await _cloudStorage.DeleteFilesInPathAsync(userPet.Attachment);
                }
                userPet.Name = TextConvert.ConvertToUnicodeEscape(userPetUpdateReq.Name ?? string.Empty);
                userPet.Type = TextConvert.ConvertToUnicodeEscape(userPetUpdateReq.Type ?? string.Empty);
                userPet.Breed = TextConvert.ConvertToUnicodeEscape(userPetUpdateReq.Breed ?? string.Empty);
                userPet.Age = TextConvert.ConvertToUnicodeEscape(userPetUpdateReq.Age ?? string.Empty);
                userPet.Gender = TextConvert.ConvertToUnicodeEscape(userPetUpdateReq.Gender ?? string.Empty);
                userPet.Weight = userPetUpdateReq.Weight;
                
                string filePath = $"user/{userId}/pet/{userPet.Id}/attachment";
                if (userPetUpdateReq.Attachment != null)
                {
                    var attachments = await _cloudStorage.UploadSingleFile(userPetUpdateReq.Attachment, filePath);
                    userPet.Attachment = attachments;
                }
                userPet.UpdateAt = DateTime.Now;

                await _userPetRepo.Update(userPet);

                var updatedUserPet = await _userPetRepo.GetSingle(x => x.Id == userPetUpdateReq.Id, includeProperties: "User");

                result.Data = _mapper.Map<UserPetUpdateResMdoel>(updatedUserPet);

            }
            catch (Exception ex)
            {
                throw new CustomException($"An error occurred: {ex.Message}");
            }
            return result;
        }
        
        public async Task<MessageResultModel> DeleteUserPet(Guid PetId , string Token)
        {
            Guid UserId = new Guid(Authentication.DecodeToken(Token, "userid"));
            var userPet = await _userPetRepo.GetSingle(x => x.Id.Equals(PetId) && x.UserId.Equals(UserId));
            if (userPet == null)
                throw new CustomException("UserPet not found");
            if (!string.IsNullOrEmpty(userPet.Attachment))
            {
                await _cloudStorage.DeleteFilesInPathAsync(userPet.Attachment);
            }
            await _userPetRepo.Delete(userPet);
            return new MessageResultModel()
            {
                Message = "Ok"
            };
        }
        
        public async Task<ListDataResultModel<UserPetModel>> GetUserPetByUserID(Guid userId, string token)
        {
            var result = new ListDataResultModel<UserPetModel>();
            try
            {
                Guid UserId = new Guid(Authentication.DecodeToken(token, "userid"));
                var userPet = await _userPetRepo.GetList(x => x.UserId.Equals(UserId));
                result.Data = _mapper.Map<List<UserPetModel>>(userPet);
            }
            catch (Exception ex)
            {
                throw new CustomException($"An error occurred: {ex.Message}");
            }
            return result;
        }
        
    }
}