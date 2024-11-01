using MeowWoofSocial.Data.Entities;
using MeowWoofSocial.Data.Repositories.GenericRepositories;

namespace MeowWoofSocial.Data.Repositories.PetStoreProductAttachmentRepositories;

public class PetStoreProductAttachmentRepositories : GenericRepositories<PetStoreProductAttachment>, IPetStoreProductAttachmentRepositories
{
    public PetStoreProductAttachmentRepositories(MeowWoofSocialContext context)
        : base(context)
    {
    }
}
