using AutoMapper;
using MeowWoofSocial.Business.ApplicationMiddleware;
using MeowWoofSocial.Data.DTO.Custom;
using MeowWoofSocial.Data.DTO.RequestModel;
using MeowWoofSocial.Data.DTO.ResponseModel;
using MeowWoofSocial.Data.Entities;
using MeowWoofSocial.Data.Enums;
using MeowWoofSocial.Data.Repositories;
using MeowWoofSocial.Data.Repositories.PetCareBookingRepositories;
using MeowWoofSocial.Data.Repositories.UserRepositories;

namespace MeowWoofSocial.Business.Services.PetCareBookingServices
{
    public class PetCareBookingServices : IPetCareBookingServices
    {
        private readonly IMapper _mapper;
        private readonly IPetCareBookingRepositories _petCareBookingRepositories;
        private readonly IUserRepositories _userRepo;
        private readonly IPetCareBookingDetailRepositories _petCareBookingDetailRepositories;
        
        public PetCareBookingServices(IMapper mapper, IPetCareBookingRepositories petCareBookingRepositories, IUserRepositories userRepo, IPetCareBookingDetailRepositories petCareBookingDetailRepositories)
        {
            _mapper = mapper;
            _petCareBookingDetailRepositories = petCareBookingDetailRepositories;
            _petCareBookingRepositories = petCareBookingRepositories;
            _userRepo = userRepo;
            _petCareBookingRepositories = petCareBookingRepositories;
        }
        
        public async Task<DataResultModel<PetCareBookingCreateResModel>> CreatePetCareBooking(PetCareBookingCreateReqModel petCareBooking,
            string token)
        {
            var result = new DataResultModel<PetCareBookingCreateResModel>();
            try
            {
                Guid userId = new Guid(Authentication.DecodeToken(token, "userid"));
                var user = await _userRepo.GetSingle(x => x.Id == userId);

                if (user == null || user.Status.Equals(AccountStatusEnums.Inactive.ToString()))
                {
                    throw new CustomException("You are banned from booking pet care due to violate of terms!");
                }
                var NewPetCareBookingId = Guid.NewGuid();
                var petCareBookingEntity = _mapper.Map<PetCareBooking>(petCareBooking);
                petCareBookingEntity.Id = NewPetCareBookingId;
                petCareBookingEntity.PetStoreId = petCareBooking.PetStoreId;
                petCareBookingEntity.UserId = petCareBooking.UserId;
                petCareBookingEntity.CreateAt = DateTime.Now;
                petCareBookingEntity.Status = OrderEnums.Pending.ToString();
                petCareBookingEntity.PetCareCategoryId = petCareBooking.PetCareCategoryId;

                await _petCareBookingRepositories.Insert(petCareBookingEntity);

                
                if(petCareBooking.PetCareBookingDetails != null)
                {
                    List<PetCareBookingDetail> listInsertPetCareBookingDetails = new();
                    foreach (var petCareBookingDetail in petCareBooking.PetCareBookingDetails)
                    {
                        var PetCareBookingDetailId = Guid.NewGuid();
                        PetCareBookingDetail newPetCareBookingDetail = new()
                        {
                            Id = PetCareBookingDetailId,
                            BookingId = petCareBookingEntity.Id,
                            PetId = petCareBookingDetail.PetId,
                            TypeTakeCare = TextConvert.ConvertToUnicodeEscape(petCareBookingDetail.TypeTakeCare),
                            TypeOfDisease = TextConvert.ConvertToUnicodeEscape(petCareBookingDetail.TypeOfDisease),
                            Status = OrderEnums.Pending.ToString(),
                            BookingDate = petCareBookingDetail.BookingDate
                        };
                        listInsertPetCareBookingDetails.Add(newPetCareBookingDetail);
                    }
                    
                    await _petCareBookingDetailRepositories.InsertRange(listInsertPetCareBookingDetails);
                }
                
                var newCareBooking = await _petCareBookingRepositories.GetSingle(x => x.Id == petCareBookingEntity.Id, includeProperties: "PetCareCategory,PetCareBookingDetails,PetStore,User");
                
                var petCareBookingtResModel = _mapper.Map<PetCareBookingCreateResModel>(newCareBooking);
                result.Data = petCareBookingtResModel;
            }
            catch (Exception ex)
            {
                throw new CustomException($"Error: {ex.Message}");
            }

            return result;
        }
        
        public async Task<MessageResultModel> CancelPetCareBooking(Guid PetCareBookingId, string Token)
        {
            Guid UserId = new Guid(Authentication.DecodeToken(Token, "userid"));
            var petCareBooking = await _petCareBookingRepositories.GetSingle(x => x.Id.Equals(PetCareBookingId) && x.UserId.Equals(UserId));
            if (petCareBooking == null)
                throw new CustomException("Booking not found");
            petCareBooking.Status = OrderEnums.Cancelled.ToString();
            await _petCareBookingRepositories.Update(petCareBooking);
            var petCareBookingDetail = await _petCareBookingDetailRepositories.GetSingle(x => x.BookingId.Equals(PetCareBookingId));
            petCareBookingDetail.Status = OrderEnums.Cancelled.ToString();
            await _petCareBookingDetailRepositories.Update(petCareBookingDetail);
            return new MessageResultModel()
            {
                Message = "Ok"
            };
        }
    }
}