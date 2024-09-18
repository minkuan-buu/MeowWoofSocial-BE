using System;
using System.Collections.Generic;

namespace MeowWoofSocial.Data.Entities;

public partial class UserBankInformation
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public string UserName { get; set; } = null!;

    public string IdentificationNumber { get; set; } = null!;

    public string BankName { get; set; } = null!;

    public string BankBranchName { get; set; } = null!;

    public string AccountNumber { get; set; } = null!;

    public string AccountHolderName { get; set; } = null!;

    public DateTime CreateAt { get; set; }

    public DateTime? UpdateAt { get; set; }

    public virtual User User { get; set; } = null!;
}
