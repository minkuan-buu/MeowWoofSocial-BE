using System;
using System.Collections.Generic;

namespace MeowWoofSocial.Data.Entities;

public partial class Category
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public string Description { get; set; } = null!;

    public Guid? ParentCategoryId { get; set; }

    public string Status { get; set; } = null!;

    public virtual ICollection<Category> InverseParentCategory { get; set; } = new List<Category>();

    public virtual Category? ParentCategory { get; set; }

    public virtual ICollection<PetStoreProduct> PetStoreProducts { get; set; } = new List<PetStoreProduct>();
}
