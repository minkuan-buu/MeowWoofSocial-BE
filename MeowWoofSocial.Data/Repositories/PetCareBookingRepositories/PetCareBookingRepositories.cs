using MeowWoofSocial.Data.Entities;
using MeowWoofSocial.Data.Repositories.GenericRepositories;

namespace MeowWoofSocial.Data.Repositories.PetCareBookingRepositories
{
    public class PetCareBookingRepositories : GenericRepositories<PetCareBooking>, IPetCareBookingRepositories
    {
        public PetCareBookingRepositories(MeowWoofSocialContext context)
            : base(context)
        {
        }
    }
}