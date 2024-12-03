using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace MeowWoofSocial.Data.Entities;

public partial class MeowWoofSocialContext : DbContext
{
    public MeowWoofSocialContext(DbContextOptions<MeowWoofSocialContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Cart> Carts { get; set; }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<Notification> Notifications { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<OrderDetail> OrderDetails { get; set; }

    public virtual DbSet<Otp> Otps { get; set; }

    public virtual DbSet<PetCareBooking> PetCareBookings { get; set; }

    public virtual DbSet<PetCareBookingDetail> PetCareBookingDetails { get; set; }

    public virtual DbSet<PetCareCategory> PetCareCategories { get; set; }

    public virtual DbSet<PetStore> PetStores { get; set; }

    public virtual DbSet<PetStoreProduct> PetStoreProducts { get; set; }

    public virtual DbSet<PetStoreProductAttachment> PetStoreProductAttachments { get; set; }

    public virtual DbSet<PetStoreProductItem> PetStoreProductItems { get; set; }

    public virtual DbSet<PetStoreProductRating> PetStoreProductRatings { get; set; }

    public virtual DbSet<PetStoreRating> PetStoreRatings { get; set; }

    public virtual DbSet<Post> Posts { get; set; }

    public virtual DbSet<PostAttachment> PostAttachments { get; set; }

    public virtual DbSet<PostHashtag> PostHashtags { get; set; }

    public virtual DbSet<PostReaction> PostReactions { get; set; }

    public virtual DbSet<PostStored> PostStoreds { get; set; }

    public virtual DbSet<Report> Reports { get; set; }

    public virtual DbSet<Transaction> Transactions { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserAddress> UserAddresses { get; set; }

    public virtual DbSet<UserBankInformation> UserBankInformations { get; set; }

    public virtual DbSet<UserFollowing> UserFollowings { get; set; }

    public virtual DbSet<UserPet> UserPets { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Cart>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Cart_pk");

            entity.ToTable("Cart");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

            entity.HasOne(d => d.Order).WithMany(p => p.Carts)
                .HasForeignKey(d => d.OrderId)
                .HasConstraintName("Cart_Order_Id_fk");

            entity.HasOne(d => d.ProductItem).WithMany(p => p.Carts)
                .HasForeignKey(d => d.ProductItemId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Cart_PetStoreProductItem_Id_fk");

            entity.HasOne(d => d.User).WithMany(p => p.Carts)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Cart_User_Id_fk");
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Category__3214EC07A943DC61");

            entity.ToTable("Category");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Attachment)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.Description)
                .HasMaxLength(511)
                .IsUnicode(false);
            entity.Property(e => e.Name).HasMaxLength(50);
            entity.Property(e => e.Status)
                .HasMaxLength(30)
                .IsUnicode(false);

            entity.HasOne(d => d.ParentCategory).WithMany(p => p.InverseParentCategory)
                .HasForeignKey(d => d.ParentCategoryId)
                .HasConstraintName("FK__Category__Parent__619B8048");
        });

        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Notifica__3214EC0723415CBD");

            entity.ToTable("Notification");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Content)
                .HasMaxLength(1000)
                .IsUnicode(false);
            entity.Property(e => e.UpdateAt).HasColumnType("datetime");

            entity.HasOne(d => d.Post).WithMany(p => p.Notifications)
                .HasForeignKey(d => d.PostId)
                .HasConstraintName("FK__Notificat__PostI__70DDC3D8");

            entity.HasOne(d => d.User).WithMany(p => p.Notifications)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Notificat__UserI__787EE5A0");
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Order__3214EC07AA426123");

            entity.ToTable("Order");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.Price).HasColumnType("decimal(15, 3)");
            entity.Property(e => e.Status)
                .HasMaxLength(30)
                .IsUnicode(false);
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

            entity.HasOne(d => d.UserAddress).WithMany(p => p.Orders)
                .HasForeignKey(d => d.UserAddressId)
                .HasConstraintName("Order_UserAddress_Id_fk");

            entity.HasOne(d => d.User).WithMany(p => p.Orders)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Order__UserId__656C112C");
        });

        modelBuilder.Entity<OrderDetail>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__OrderDet__3214EC074A4196BA");

            entity.ToTable("OrderDetail");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.CreateAt).HasColumnType("datetime");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.UnitPrice).HasColumnType("decimal(15, 3)");
            entity.Property(e => e.UpdateAt).HasColumnType("datetime");

            entity.HasOne(d => d.Order).WithMany(p => p.OrderDetails)
                .HasForeignKey(d => d.OrderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__OrderDeta__Order__6383C8BA");

            entity.HasOne(d => d.ProductItem).WithMany(p => p.OrderDetails)
                .HasForeignKey(d => d.ProductItemId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__OrderDeta__Produ__6754599E");
        });

        modelBuilder.Entity<Otp>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("OTP_pk");

            entity.ToTable("OTP");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Code)
                .HasMaxLength(6)
                .IsUnicode(false);
            entity.Property(e => e.ExpiredDate).HasColumnType("datetime");
            entity.Property(e => e.IsUsed).HasColumnName("isUsed");
            entity.Property(e => e.Status)
                .HasMaxLength(30)
                .IsUnicode(false);

            entity.HasOne(d => d.User).WithMany(p => p.Otps)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("OTP_User_Id_fk");
        });

        modelBuilder.Entity<PetCareBooking>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__PetCareB__3214EC07396B062F");

            entity.ToTable("PetCareBooking");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.CreateAt).HasColumnType("datetime");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasOne(d => d.Order).WithMany(p => p.PetCareBookings)
                .HasForeignKey(d => d.OrderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("PetCareBooking___fk_2");

            entity.HasOne(d => d.PetCareCategory).WithMany(p => p.PetCareBookings)
                .HasForeignKey(d => d.PetCareCategoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("PetCareBooking___fk");

            entity.HasOne(d => d.PetStore).WithMany(p => p.PetCareBookings)
                .HasForeignKey(d => d.PetStoreId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PetCareBo__PetSt__797309D9");

            entity.HasOne(d => d.User).WithMany(p => p.PetCareBookings)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PetCareBo__UserI__6FE99F9F");
        });

        modelBuilder.Entity<PetCareBookingDetail>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__PetCareB__3214EC07E4929218");

            entity.ToTable("PetCareBookingDetail");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.BookingDate).HasColumnType("datetime");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.TypeOfDisease)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.TypeTakeCare)
                .HasMaxLength(500)
                .IsUnicode(false);

            entity.HasOne(d => d.Booking).WithMany(p => p.PetCareBookingDetails)
                .HasForeignKey(d => d.BookingId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PetCareBo__Booki__7A672E12");

            entity.HasOne(d => d.Pet).WithMany(p => p.PetCareBookingDetails)
                .HasForeignKey(d => d.PetId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PetCareBo__PetId__7B5B524B");
        });

        modelBuilder.Entity<PetCareCategory>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__PetCareC__3214EC0757C705D7");

            entity.ToTable("PetCareCategory");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Attachment)
                .HasMaxLength(4000)
                .IsUnicode(false);
            entity.Property(e => e.Description).HasColumnType("text");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<PetStore>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__PetStore__3214EC07E0AD2DD6");

            entity.ToTable("PetStore");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Attachment)
                .HasMaxLength(1000)
                .IsUnicode(false);
            entity.Property(e => e.CreateAt).HasColumnType("datetime");
            entity.Property(e => e.Description)
                .HasMaxLength(5000)
                .IsUnicode(false);
            entity.Property(e => e.Email)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.Name)
                .HasMaxLength(1000)
                .IsUnicode(false);
            entity.Property(e => e.Phone)
                .HasMaxLength(12)
                .IsUnicode(false);
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.TypeStore)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.UpdateAt).HasColumnType("datetime");

            entity.HasOne(d => d.User).WithMany(p => p.PetStores)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PetStore__UserId__66603565");
        });

        modelBuilder.Entity<PetStoreProduct>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__PetStore__3214EC07C48C4F18");

            entity.ToTable("PetStoreProduct");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.CreateAt).HasColumnType("datetime");
            entity.Property(e => e.Name)
                .HasMaxLength(1000)
                .IsUnicode(false);
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.UpdateAt).HasColumnType("datetime");

            entity.HasOne(d => d.Category).WithMany(p => p.PetStoreProducts)
                .HasForeignKey(d => d.CategoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PetStoreP__Categ__68487DD7");

            entity.HasOne(d => d.PetStore).WithMany(p => p.PetStoreProducts)
                .HasForeignKey(d => d.PetStoreId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PetStoreP__PetSt__693CA210");
        });

        modelBuilder.Entity<PetStoreProductAttachment>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PetStoreProductAttachment_pk");

            entity.ToTable("PetStoreProductAttachment");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Attachment)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.CreateAt).HasColumnType("datetime");

            entity.HasOne(d => d.PetStoreProduct).WithMany(p => p.PetStoreProductAttachments)
                .HasForeignKey(d => d.PetStoreProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("PetStoreProductAttachment_PetStoreProduct_Id_fk");
        });

        modelBuilder.Entity<PetStoreProductItem>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__PetStore__3214EC07EE5D4A9C");

            entity.ToTable("PetStoreProductItem");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Name)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.Price).HasColumnType("decimal(15, 3)");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasOne(d => d.Product).WithMany(p => p.PetStoreProductItems)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PetStoreP__Produ__7C4F7684");
        });

        modelBuilder.Entity<PetStoreProductRating>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__PetStore__3214EC0752F752E7");

            entity.ToTable("PetStoreProductRating");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Comment)
                .HasMaxLength(1000)
                .IsUnicode(false);
            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.Rating).HasColumnType("decimal(3, 1)");

            entity.HasOne(d => d.ProductItem).WithMany(p => p.PetStoreProductRatings)
                .HasForeignKey(d => d.ProductItemId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("PetStoreProductRating___fk");

            entity.HasOne(d => d.User).WithMany(p => p.PetStoreProductRatings)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PetStoreR__UserI__7D439ABD");
        });

        modelBuilder.Entity<PetStoreRating>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ProductR__3214EC07D8E8D12B");

            entity.ToTable("PetStoreRating");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Comment)
                .HasMaxLength(1000)
                .IsUnicode(false);
            entity.Property(e => e.Rating).HasColumnType("decimal(3, 1)");

            entity.HasOne(d => d.PetStore).WithMany(p => p.PetStoreRatings)
                .HasForeignKey(d => d.PetStoreId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ProductRa__PetStore__00200768");

            entity.HasOne(d => d.User).WithMany(p => p.PetStoreRatings)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ProductRa__UserI__7F2BE32F");
        });

        modelBuilder.Entity<Post>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Post__3214EC0790F952F8");

            entity.ToTable("Post");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Content)
                .HasMaxLength(8000)
                .IsUnicode(false);
            entity.Property(e => e.CreateAt).HasColumnType("datetime");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.UpdateAt).HasColumnType("datetime");

            entity.HasOne(d => d.User).WithMany(p => p.Posts)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Post__UserId__72C60C4A");
        });

        modelBuilder.Entity<PostAttachment>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__PostAtta__3214EC07E8366C6C");

            entity.ToTable("PostAttachment");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Attachment).IsUnicode(false);
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasOne(d => d.Post).WithMany(p => p.PostAttachments)
                .HasForeignKey(d => d.PostId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PostAttac__PostI__6C190EBB");
        });

        modelBuilder.Entity<PostHashtag>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__PostHash__3214EC07C601230D");

            entity.ToTable("PostHashtag");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Hashtag)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasOne(d => d.Post).WithMany(p => p.PostHashtags)
                .HasForeignKey(d => d.PostId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PostHasht__PostI__6B24EA82");
        });

        modelBuilder.Entity<PostReaction>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__PostReac__3214EC078BDC084C");

            entity.ToTable("PostReaction");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Attachment)
                .HasMaxLength(1000)
                .IsUnicode(false);
            entity.Property(e => e.Content)
                .HasMaxLength(8000)
                .IsUnicode(false);
            entity.Property(e => e.CreateAt).HasColumnType("datetime");
            entity.Property(e => e.Type)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.TypeReact)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.UpdateAt).HasColumnType("datetime");

            entity.HasOne(d => d.Post).WithMany(p => p.PostReactions)
                .HasForeignKey(d => d.PostId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PostReact__PostI__71D1E811");

            entity.HasOne(d => d.User).WithMany(p => p.PostReactions)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PostReact__UserI__778AC167");
        });

        modelBuilder.Entity<PostStored>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__PostStor__3214EC07F81F461A");

            entity.ToTable("PostStored");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.CreateAt).HasColumnType("datetime");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasOne(d => d.Post).WithMany(p => p.PostStoreds)
                .HasForeignKey(d => d.PostId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PostStore__PostI__74AE54BC");

            entity.HasOne(d => d.User).WithMany(p => p.PostStoreds)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PostStore__UserI__73BA3083");
        });

        modelBuilder.Entity<Report>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Report__3214EC07ED1E5112");

            entity.ToTable("Report");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.CreateAt).HasColumnType("datetime");
            entity.Property(e => e.Reason)
                .HasMaxLength(1000)
                .IsUnicode(false);
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Type)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

            entity.HasOne(d => d.Comment).WithMany(p => p.Reports)
                .HasForeignKey(d => d.CommentId)
                .HasConstraintName("FK__Report__CommentI__6EF57B66");

            entity.HasOne(d => d.Post).WithMany(p => p.Reports)
                .HasForeignKey(d => d.PostId)
                .HasConstraintName("FK__Report__PostId__6E01572D");

            entity.HasOne(d => d.User).WithMany(p => p.Reports)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Report__UserId__6D0D32F4");
        });

        modelBuilder.Entity<Transaction>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Transact__3214EC0745DBBA46");

            entity.ToTable("Transaction");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.FinishedTransactionAt).HasColumnType("datetime");
            entity.Property(e => e.PaymentLinkId)
                .HasMaxLength(300)
                .IsUnicode(false);
            entity.Property(e => e.Status)
                .HasMaxLength(30)
                .IsUnicode(false);
            entity.Property(e => e.TransactionReference)
                .HasMaxLength(300)
                .IsUnicode(false);

            entity.HasOne(d => d.Order).WithMany(p => p.Transactions)
                .HasForeignKey(d => d.OrderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Transacti__Order__6477ECF3");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__User__3214EC07930BC8A3");

            entity.ToTable("User");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Avartar)
                .HasMaxLength(1000)
                .IsUnicode(false);
            entity.Property(e => e.CreateAt).HasColumnType("datetime");
            entity.Property(e => e.Email)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.Name)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.Phone)
                .HasMaxLength(1000)
                .IsUnicode(false);
            entity.Property(e => e.Role)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.UpdateAt).HasColumnType("datetime");
        });

        modelBuilder.Entity<UserAddress>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__UserAddr__3214EC07D0E525A5");

            entity.ToTable("UserAddress");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Address)
                .HasMaxLength(2000)
                .IsUnicode(false);
            entity.Property(e => e.CreateAt).HasColumnType("datetime");
            entity.Property(e => e.Name)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.Phone)
                .HasMaxLength(1000)
                .IsUnicode(false);
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.UpdateAt).HasColumnType("datetime");

            entity.HasOne(d => d.User).WithMany(p => p.UserAddresses)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__UserAddre__UserI__01142BA1");
        });

        modelBuilder.Entity<UserBankInformation>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__UserBank__3214EC07C3BD2C39");

            entity.ToTable("UserBankInformation");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.AccountHolderName)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.AccountNumber)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.BankBranchName).HasColumnType("text");
            entity.Property(e => e.BankName)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.CreateAt).HasColumnType("datetime");
            entity.Property(e => e.IdentificationNumber)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.UpdateAt).HasColumnType("datetime");
            entity.Property(e => e.UserName).HasColumnType("text");

            entity.HasOne(d => d.User).WithMany(p => p.UserBankInformations)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__UserBankI__UserI__628FA481");
        });

        modelBuilder.Entity<UserFollowing>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__UserFoll__3214EC07A541B9B8");

            entity.ToTable("UserFollowing");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasOne(d => d.Follower).WithMany(p => p.UserFollowingFollowers)
                .HasForeignKey(d => d.FollowerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__UserFollo__Follo__75A278F5");

            entity.HasOne(d => d.User).WithMany(p => p.UserFollowingUsers)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__UserFollo__UserI__76969D2E");
        });

        modelBuilder.Entity<UserPet>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__UserPet__3214EC076F20BF20");

            entity.ToTable("UserPet");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Age)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Attachment)
                .HasMaxLength(1000)
                .IsUnicode(false);
            entity.Property(e => e.Breed)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.CreateAt).HasColumnType("datetime");
            entity.Property(e => e.Gender)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Name)
                .HasMaxLength(450)
                .IsUnicode(false);
            entity.Property(e => e.Type)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.UpdateAt).HasColumnType("datetime");
            entity.Property(e => e.Weight).HasColumnType("decimal(4, 1)");

            entity.HasOne(d => d.User).WithMany(p => p.UserPets)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__UserPet__UserId__6A30C649");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
