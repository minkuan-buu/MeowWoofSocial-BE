using AutoMapper;
using MeowWoofSocial.Business.ApplicationMiddleware;
using MeowWoofSocial.Business.Services.CloudServices;
using MeowWoofSocial.Data.DTO.Custom;
using MeowWoofSocial.Data.DTO.RequestModel;
using MeowWoofSocial.Data.DTO.ResponseModel;
using MeowWoofSocial.Data.Entities;
using MeowWoofSocial.Data.Enums;
using MeowWoofSocial.Data.Repositories.PetStoreProductAttachmentRepositories;
using MeowWoofSocial.Data.Repositories.PetStoreProductItemRepositories;
using MeowWoofSocial.Data.Repositories.PetStoreProductRepositories;
using MeowWoofSocial.Data.Repositories.PetStoreRepositories;
using MeowWoofSocial.Data.Repositories.UserRepositories;


namespace MeowWoofSocial.Business.Services.PetStoreProductServices;

public class PetStoreProductServices : IPetStoreProductServices
{
    private readonly IMapper _mapper;
    private readonly IPetStoreProductRepositories _petStoreProductRepo;
    private readonly IUserRepositories _userRepo;
    private readonly IPetStoreProductAttachmentRepositories _petStoreProductAttachmentRepo;
    private readonly ICloudStorage _cloudStorage;
    private readonly IPetStoreProductItemRepositories _petStoreProductItemRepo;
    
    public PetStoreProductServices(IMapper mapper, IPetStoreProductRepositories petStoreProductRepositories, IUserRepositories userRepositories, IPetStoreProductAttachmentRepositories petStoreProductAttachmentRepo, ICloudStorage cloudStorage, IPetStoreProductItemRepositories petStoreProductItemRepo)
    {
        _mapper = mapper;
        _petStoreProductRepo = petStoreProductRepositories;
        _userRepo = userRepositories;
        _petStoreProductAttachmentRepo = petStoreProductAttachmentRepo;
        _cloudStorage = cloudStorage;
        _petStoreProductItemRepo = petStoreProductItemRepo;
    }
    
    public async Task<DataResultModel<PetStoreProductCreateResModel>> CreatePetStoreProduct(PetStoreProductCreateReqModel petStoreProduct,
        string token)
        {
            var result = new DataResultModel<PetStoreProductCreateResModel>();
            try
            {
                Guid userId = new Guid(Authentication.DecodeToken(token, "userid"));
                var user = await _userRepo.GetSingle(x => x.Id == userId);

                if (user == null || user.Status.Equals(AccountStatusEnums.Inactive))
                {
                    throw new CustomException("You are banned from creating pet store product due to violate of terms!");
                }
                var NewPetStoreProductId = Guid.NewGuid();
                var petStoreProductEntity = _mapper.Map<PetStoreProduct>(petStoreProduct);
                petStoreProductEntity.Id = NewPetStoreProductId;
                petStoreProductEntity.PetStoreId = petStoreProduct.PetStoreId;
                petStoreProductEntity.CategoryId = petStoreProduct.CategoryId;
                petStoreProductEntity.CreateAt = DateTime.Now;
                petStoreProductEntity.Status = GeneralStatusEnums.Active.ToString();

                await _petStoreProductRepo.Insert(petStoreProductEntity);

                if (petStoreProduct.Attachment != null)
                {
                    string filePath = $"petstoreproduct/{NewPetStoreProductId}/attachments";
                    List<string> GetStringURL = await _cloudStorage.UploadFile(petStoreProduct.Attachment, filePath);
                    List<PetStoreProductAttachment> ListInsertAttachment = new();
                    foreach (var link in GetStringURL)
                    {
                        PetStoreProductAttachment newAttachment = new()
                        {
                            Id = Guid.NewGuid(),
                            PetStoreProductId = petStoreProductEntity.Id,
                            Attachment = link,
                            CreateAt = DateTime.Now
                        };
                        ListInsertAttachment.Add(newAttachment);
                    }

                    await _petStoreProductAttachmentRepo.InsertRange(ListInsertAttachment);
                }
                if(petStoreProduct.PetStoreProductItems != null)
                {
                List<PetStoreProductItem> listInsertProductItems = new();
                    foreach (var productItem in petStoreProduct.PetStoreProductItems)
                    {
                        var productItemId = Guid.NewGuid();
                        PetStoreProductItem newProductItem = new()
                        {
                            Id = productItemId,
                            ProductId = petStoreProductEntity.Id,
                            Name = productItem.Name,
                            Quantity = productItem.Quantity,
                            Price = productItem.Price,
                            Status = GeneralStatusEnums.Active.ToString()
                        };
                        listInsertProductItems.Add(newProductItem);
                    }
                    await _petStoreProductItemRepo.InsertRange(listInsertProductItems);
                }
                
                var newPetStoreProduct = await _petStoreProductRepo.GetSingle(x => x.Id == petStoreProductEntity.Id, includeProperties: "PetStoreProductAttachments,PetStore,Category.ParentCategory,PetStoreProductItems");
                
                var pettStoreProductResModel = _mapper.Map<PetStoreProductCreateResModel>(newPetStoreProduct);
                result.Data = pettStoreProductResModel;
            }
            catch (Exception ex)
            {
                throw new CustomException($"Error: {ex.Message}");
            }

            return result;
        }
    
    public async Task<DataResultModel<PetStoreProductUpdateResModel>> UpdatePetStoreProduct(PetStoreProductUpdateReqModel petStoreProductUpdateReq, string token)
    {
         var result = new DataResultModel<PetStoreProductUpdateResModel>();
            try
            {
                Guid userId = new Guid(Authentication.DecodeToken(token, "userid"));
                var petStoreProduct = await _petStoreProductRepo.GetSingle(x => x.Id == petStoreProductUpdateReq.Id, includeProperties: "PetStoreProductAttachments,PetStore,Category.ParentCategory,PetStoreProductItems");

                if (petStoreProduct == null)
                {
                    throw new CustomException("Pet store product not found or you do not have permission to update this pet store product");
                }

                if (petStoreProduct.Status == GeneralStatusEnums.Inactive.ToString())
                {
                    throw new CustomException("Cannot update an inactive pet store product");
                }
                if (petStoreProductUpdateReq.Name != null)
                {
                    petStoreProduct.Name = TextConvert.ConvertToUnicodeEscape(petStoreProductUpdateReq.Name);
                } else
                {
                    throw new CustomException("Name cannot null");
                }
                petStoreProduct.UpdateAt = DateTime.Now;
                await _petStoreProductRepo.Update(petStoreProduct);
                if (petStoreProduct.PetStoreProductAttachments.Count > 0)
                {
                    await _petStoreProductAttachmentRepo.DeleteRange(petStoreProduct.PetStoreProductAttachments);
                }
                if (petStoreProduct.PetStoreProductItems.Count > 0)
                {
                    await _petStoreProductItemRepo.DeleteRange(petStoreProduct.PetStoreProductItems);
                }
                
                var filePath = $"petstoreproduct/{petStoreProduct.Id}/attachments";
                await _cloudStorage.DeleteFilesInPathAsync(filePath);
                if (petStoreProductUpdateReq.Attachment != null && petStoreProductUpdateReq.Attachment.Count > 0)
                {
                    var attachments = await _cloudStorage.UploadFile(petStoreProductUpdateReq.Attachment, filePath);
                    List<PetStoreProductAttachment> ListAttachmentAdd = new();
                    foreach (var attachment in attachments)
                    {
                        var petStoreProductAttachment = new PetStoreProductAttachment
                        {
                            Id = Guid.NewGuid(),
                            PetStoreProductId = petStoreProduct.Id,
                            Attachment = attachment,
                            CreateAt = DateTime.Now
                        };
                        ListAttachmentAdd.Add(petStoreProductAttachment);
                    }
                    await _petStoreProductAttachmentRepo.InsertRange(ListAttachmentAdd);
                }
                if(petStoreProductUpdateReq.PetStoreProductItems != null && petStoreProductUpdateReq.PetStoreProductItems.Count > 0)
                {
                    List<PetStoreProductItem> listInsertProductItems = new();
                    foreach (var productItem in petStoreProductUpdateReq.PetStoreProductItems)
                    {
                        var productItemId = Guid.NewGuid();
                        PetStoreProductItem newProductItem = new()
                        {
                            Id = productItemId,
                            ProductId = petStoreProduct.Id,
                            Name = productItem.Name,
                            Quantity = productItem.Quantity,
                            Price = productItem.Price,
                            Status = GeneralStatusEnums.Active.ToString()
                        };
                        listInsertProductItems.Add(newProductItem);
                    }
                    await _petStoreProductItemRepo.InsertRange(listInsertProductItems);
                }
                var newPetStoreProduct = await _petStoreProductRepo.GetSingle(x => x.Id == petStoreProductUpdateReq.Id, includeProperties: "PetStoreProductAttachments,PetStore,Category.ParentCategory,PetStoreProductItems");
                var pettStoreProductResModel = _mapper.Map<PetStoreProductUpdateResModel>(newPetStoreProduct);
                result.Data = pettStoreProductResModel;

            }
            catch (Exception ex)
            {
                throw new CustomException($"An error occurred: {ex.Message}");
            }
            return result;
    }
    
    public async Task<DataResultModel<PetStoreProductDeleteResModel>> DeletePetStoreProduct(PetStoreProductDeleteReqModel PetStoreDeleteReq, string token)
    {
        try
        {
            Guid userId = new Guid(Authentication.DecodeToken(token, "userid"));
            var petStore = await _petStoreProductRepo.GetSingle(c => c.Id == PetStoreDeleteReq.PetStoreProductId);

            if (petStore == null || petStore.Id != PetStoreDeleteReq.PetStoreProductId)
            {
                throw new CustomException("PetStoreProduct not found or does not belong to the user.");
            }

            await _petStoreProductRepo.Delete(petStore);

            var result = _mapper.Map<PetStoreProductDeleteResModel>(petStore);
            return new DataResultModel<PetStoreProductDeleteResModel> { Data = result };
        }
        catch (Exception ex)
        {
            throw new CustomException($"Error deleting PetStore: {ex.Message}");
        }
    }
    
    public async Task<ListDataResultModel<GetAllPetStoreProductResModel>> GetAllPetStoreProduct(PetStoreProductReq petStoreProductReq)
    {
        try
        {
            List<GetAllPetStoreProductResModel> petStoreProductRes = new();
            DateTime? lastPetStoreProductCreateAt = null;

            if (petStoreProductReq.lastPetStoreProductId.HasValue)
            {
                var lastPetStoreProduct = await _petStoreProductRepo.GetSingle(x => x.Id == petStoreProductReq.lastPetStoreProductId.Value);
                lastPetStoreProductCreateAt = lastPetStoreProduct?.CreateAt;
            }

            var allPetStoreProducts = await _petStoreProductRepo.GetList(
                x => x.Status.Equals(GeneralStatusEnums.Active.ToString()),
                includeProperties: "PetStoreProductAttachments,PetStoreProductItems,PetStoreProductItems.OrderDetails"
            );

            allPetStoreProducts = allPetStoreProducts.OrderByDescending(p => p.CreateAt).ToList();

            if (lastPetStoreProductCreateAt.HasValue)
            {
                allPetStoreProducts = allPetStoreProducts
                    .Where(p => p.CreateAt < lastPetStoreProductCreateAt.Value)
                    .ToList();
            }

            allPetStoreProducts = allPetStoreProducts
                .Where(p => p.Status.Equals(GeneralStatusEnums.Active.ToString()))
                .Take(petStoreProductReq.PageSize)
                .ToList();

            return new ListDataResultModel<GetAllPetStoreProductResModel>
            {
                Data = allPetStoreProducts.Select(MapPetStoreProduct).ToList(),
            };
        }
        catch (Exception ex)
        {
            throw new CustomException($"An error occurred while fetching posts: {ex.Message}");
        }
    }
    
    private GetAllPetStoreProductResModel MapPetStoreProduct(PetStoreProduct petStoreProduct)
        {
            return new GetAllPetStoreProductResModel
            {
                Id = petStoreProduct.Id,
                Name = TextConvert.ConvertFromUnicodeEscape(petStoreProduct.Name),
                Attachments = petStoreProduct.PetStoreProductAttachments.Select(x => new PetStoreProductAttachmentResModel()
                {
                    Id = x.Id,
                    Attachment = x.Attachment
                }).ToList(),
                Price = petStoreProduct.PetStoreProductItems.Select(x => x.Price).FirstOrDefault(),
                TotalSales = petStoreProduct.PetStoreProductItems
                    .SelectMany(x => x.OrderDetails)
                    .Where(od => od.Status.Equals(TransactionEnums.Success.ToString()))
                    .Sum(od => od.Quantity)
            };
        }

    public async Task<DataResultModel<PetStoreProductCreateResModel>> GetPetStoreProductById(Guid petStoreProductId)
    {
        var result = new DataResultModel<PetStoreProductCreateResModel>();
        try
        {
            var newPetStoreProduct = await _petStoreProductRepo.GetSingle(x => x.Id == petStoreProductId,
                includeProperties: "PetStoreProductAttachments,PetStore,Category.ParentCategory,PetStoreProductItems");

            var pettStoreProductResModel = _mapper.Map<PetStoreProductCreateResModel>(newPetStoreProduct);
            result.Data = pettStoreProductResModel;
        }
        catch (Exception ex)
        {
            throw new CustomException($"Error: {ex.Message}");
        }

        return result;
    }
}
