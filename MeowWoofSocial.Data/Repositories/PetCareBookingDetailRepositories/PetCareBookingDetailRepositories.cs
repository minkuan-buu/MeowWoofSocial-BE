using MeowWoofSocial.Data.Entities;
using MeowWoofSocial.Data.Repositories.GenericRepositories;
namespace MeowWoofSocial.Data.Repositories
{
    public class PetCareBookingDetailRepositories : GenericRepositories<PetCareBookingDetail>,
        IPetCareBookingDetailRepositories
    {
        public PetCareBookingDetailRepositories(MeowWoofSocialContext context)
            : base(context)
        {
        }
    }
}
