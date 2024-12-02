using AutoMapper;
using MeowWoofSocial.Business.ApplicationMiddleware;
using MeowWoofSocial.Data.DTO.Custom;
using MeowWoofSocial.Data.DTO.RequestModel;
using MeowWoofSocial.Data.DTO.ResponseModel;
using MeowWoofSocial.Data.Entities;
using MeowWoofSocial.Data.Enums;
using MeowWoofSocial.Data.Repositories;
using MeowWoofSocial.Data.Repositories.OrderRepositories;
using MeowWoofSocial.Data.Repositories.PetCareBookingRepositories;
using MeowWoofSocial.Data.Repositories.TransactionRepositories;
using MeowWoofSocial.Data.Repositories.UserRepositories;
using Net.payOS;
using Net.payOS.Types;
using Transaction = MeowWoofSocial.Data.Entities.Transaction;

namespace MeowWoofSocial.Business.Services.PetCareBookingServices
{
    public class PetCareBookingServices : IPetCareBookingServices
    {
        private readonly IMapper _mapper;
        private readonly IPetCareBookingRepositories _petCareBookingRepositories;
        private readonly IUserRepositories _userRepo;
        private readonly IPetCareBookingDetailRepositories _petCareBookingDetailRepositories;
        private readonly IOrderRepositories _orderRepositories;
        private readonly ITransactionRepositories _transactionRepositories;
        private PayOS _payOS;
        private const string MEMO_PREFIX = "DH";
        
        public PetCareBookingServices(IMapper mapper, IPetCareBookingRepositories petCareBookingRepositories, IUserRepositories userRepo, IPetCareBookingDetailRepositories petCareBookingDetailRepositories, IOrderRepositories orderRepositories, ITransactionRepositories transactionRepositories)
        {
            _payOS = new PayOS("421fdf87-bbe1-4694-a76c-17627d705a85", "7a2f58da-4003-4349-9e4b-f6bbfc556c9b", "da759facf68f863e0ed11385d3bf9cf24f35e2b171d1fa8bae8d91ce1db9ff0c");
            _mapper = mapper;
            _petCareBookingDetailRepositories = petCareBookingDetailRepositories;
            _petCareBookingRepositories = petCareBookingRepositories;
            _userRepo = userRepo;
            _petCareBookingRepositories = petCareBookingRepositories;
            _orderRepositories = orderRepositories;
            _transactionRepositories = transactionRepositories;
        }
        
        public async Task<string> CreatePetCareBooking(PetCareBookingCreateReqModel petCareBooking,
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
                var checkExist = await _petCareBookingRepositories.GetSingle(x => x.UserId == userId && x.Status == OrderEnums.Pending.ToString() && x.PetCareCategoryId == petCareBooking.PetCareCategoryId, includeProperties:"Order.Transactions");
                if (checkExist != null)
                {
                    return $"https://pay.payos.vn/web/{checkExist.Order.Transactions.FirstOrDefault(x => x.Status.Equals(TransactionEnums.PENDING.ToString()))?.PaymentLinkId}";
                }
                var newOrderId = Guid.NewGuid();
                var newOrder = new Order()
                {
                    Id = newOrderId,
                    UserId = userId,
                    CreatedAt = DateTime.Now,
                    Status = OrderEnums.Pending.ToString(),
                    Price = 200000,
                };
                await _orderRepositories.Insert(newOrder);
                
                var NewPetCareBookingId = Guid.NewGuid();
                var petCareBookingEntity = _mapper.Map<PetCareBooking>(petCareBooking);
                petCareBookingEntity.Id = NewPetCareBookingId;
                petCareBookingEntity.PetStoreId = petCareBooking.PetStoreId;
                petCareBookingEntity.UserId = petCareBooking.UserId;
                petCareBookingEntity.CreateAt = DateTime.Now;
                petCareBookingEntity.Status = OrderEnums.Pending.ToString();
                petCareBookingEntity.PetCareCategoryId = petCareBooking.PetCareCategoryId;
                petCareBookingEntity.OrderId = newOrderId;
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
                
                
                var order = await _orderRepositories.GetSingle(x => x.Id.Equals(newOrderId) && x.UserId.Equals(userId) && x.Status.Equals(OrderEnums.Pending.ToString()), includeProperties: "Transactions");
            
            // Kiểm tra xem có giao dịch PENDING hay không
                var pendingTransaction = order.Transactions.Any(x => x.Status.Equals(TransactionEnums.PENDING.ToString()));

                if (pendingTransaction) // Chỉ cần kiểm tra nếu có giao dịch đang chờ xử lý
                {
                    // Lấy PaymentLinkId của giao dịch đang chờ xử lý
                    var transaction = order.Transactions.FirstOrDefault(x => x.Status.Equals(TransactionEnums.PENDING.ToString()));
                    if (transaction != null)
                    {
                        return $"https://pay.payos.vn/web/{transaction.PaymentLinkId}";
                    }
                    else
                    {
                        // Trường hợp không có giao dịch PENDING thì trả về null hoặc thông báo lỗi
                        return "No pending transaction found.";
                    }
                }
                else
                {
                    // Nếu không có giao dịch PENDING thì tạo link thanh toán mới
                    var OrderPaymentRefId = int.Parse(GenerateRandomRefId());
                    List<ItemData> itemDatas = new()
                    {
                        new ItemData(
                            $"Booking {TextConvert.ConvertFromUnicodeEscape(newCareBooking.PetStore.Name)}", 1, 200000)
                    };

                    PaymentData paymentData = new PaymentData(OrderPaymentRefId, (int)order.Price, "",
                        itemDatas, cancelUrl: "https://meowwoofsocial.com/payment", returnUrl: "https://meowwoofsocial.com/payment");

                    CreatePaymentResult createPayment = await _payOS.createPaymentLink(paymentData);
                    Transaction NewTransaction = new Transaction()
                    {
                        Id = Guid.NewGuid(),
                        OrderId = order.Id,
                        OrderPaymentRefId = OrderPaymentRefId,
                        Status = TransactionEnums.PENDING.ToString(),
                        PaymentLinkId = createPayment.paymentLinkId,
                        CreatedAt = DateTime.Now,
                    };
                    await _transactionRepositories.Insert(NewTransaction);
                    return createPayment.checkoutUrl;
                }
            
            }
            catch (Exception ex)
            {
                throw new CustomException($"Error: {ex.Message}");
            }
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
        private string GenerateRandomRefId()
        {
            var random = new Random();
            return random.Next(10000000, 99999999).ToString();
        }
    }
}