using MeowWoofSocial.Data.Entities;
using MeowWoofSocial.Data.Repositories.GenericRepositories;

namespace MeowWoofSocial.Data.Repositories.TransactionRepositories;

public class TransactionRepositories : GenericRepositories<Transaction>, ITransactionRepositories
{
    public TransactionRepositories(MeowWoofSocialContext context)
        : base(context)
    {
    }
}