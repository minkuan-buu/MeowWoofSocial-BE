using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeowWoofSocial.Data.Enums
{
    public enum TransactionEnums
    {
        PAID,
        PENDING,
        CANCELLED,
        PROCESSING,
    }
    
    public enum PaymentMethodEnums
    {
        BankTransfer,
        CreditCard
    }
}
