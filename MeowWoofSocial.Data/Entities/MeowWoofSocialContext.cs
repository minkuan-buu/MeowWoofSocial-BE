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

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<Notification> Notifications { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<OrderDetail> OrderDetails { get; set; }

    public virtual DbSet<PetCareBooking> PetCareBookings { get; set; }

    public virtual DbSet<PetCareBookingDetail> PetCareBookingDetails { get; set; }

    public virtual DbSet<PetStore> PetStores { get; set; }

    public virtual DbSet<PetStoreProduct> PetStoreProducts { get; set; }

    public virtual DbSet<PetStoreProductItem> PetStoreProductItems { get; set; }

    public virtual DbSet<PetStoreRating> PetStoreRatings { get; set; }

    public virtual DbSet<Post> Posts { get; set; }

    public virtual DbSet<PostAttachment> PostAttachments { get; set; }

    public virtual DbSet<PostHashtag> PostHashtags { get; set; }

    public virtual DbSet<PostReaction> PostReactions { get; set; }

    public virtual DbSet<PostStored> PostStoreds { get; set; }

    public virtual DbSet<ProductRating> ProductRatings { get; set; }

    public virtual DbSet<Report> Reports { get; set; }

    public virtual DbSet<Transaction> Transactions { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserBankInformation> UserBankInformations { get; set; }

    public virtual DbSet<UserFollowing> UserFollowings { get; set; }

    public virtual DbSet<UserPet> UserPets { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Category__3214EC070E7256E9");

            entity.ToTable("Category");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Description)
                .HasMaxLength(511)
                .IsUnicode(false);
            entity.Property(e => e.Name).HasMaxLength(50);
            entity.Property(e => e.Status)
                .HasMaxLength(30)
                .IsUnicode(false);

            entity.HasOne(d => d.ParentCategory).WithMany(p => p.InverseParentCategory)
                .HasForeignKey(d => d.ParentCategoryId)
                .HasConstraintName("FK__Category__Parent__5FB337D6");
        });

        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Notifica__3214EC07E73C7E16");

            entity.ToTable("Notification");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Content)
                .HasMaxLength(1000)
                .IsUnicode(false);
            entity.Property(e => e.UpdateAt).HasColumnType("datetime");

            entity.HasOne(d => d.Post).WithMany(p => p.Notifications)
                .HasForeignKey(d => d.PostId)
                .HasConstraintName("FK__Notificat__PostI__6EF57B66");

            entity.HasOne(d => d.User).WithMany(p => p.Notifications)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Notificat__UserI__76969D2E");
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Order__3214EC07CEFEDDBC");

            entity.ToTable("Order");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.CreateAt).HasColumnType("datetime");
            entity.Property(e => e.Price).HasColumnType("decimal(15, 3)");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.UpdateAt).HasColumnType("datetime");

            entity.HasOne(d => d.User).WithMany(p => p.Orders)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Order__UserId__6383C8BA");
        });

        modelBuilder.Entity<OrderDetail>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__OrderDet__3214EC0756D07623");

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
                .HasConstraintName("FK__OrderDeta__Order__619B8048");

            entity.HasOne(d => d.ProductItem).WithMany(p => p.OrderDetails)
                .HasForeignKey(d => d.ProductItemId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__OrderDeta__Produ__656C112C");
        });

        modelBuilder.Entity<PetCareBooking>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__PetCareB__3214EC07D990A40E");

            entity.ToTable("PetCareBooking");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.CreateAt).HasColumnType("datetime");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasOne(d => d.PetStore).WithMany(p => p.PetCareBookings)
                .HasForeignKey(d => d.PetStoreId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PetCareBo__PetSt__778AC167");

            entity.HasOne(d => d.User).WithMany(p => p.PetCareBookings)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PetCareBo__UserI__6E01572D");
        });

        modelBuilder.Entity<PetCareBookingDetail>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__PetCareB__3214EC07556308FF");

            entity.ToTable("PetCareBookingDetail");

            entity.Property(e => e.Id).ValueGeneratedNever();
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
                .HasConstraintName("FK__PetCareBo__Booki__787EE5A0");

            entity.HasOne(d => d.Pet).WithMany(p => p.PetCareBookingDetails)
                .HasForeignKey(d => d.PetId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PetCareBo__PetId__797309D9");
        });

        modelBuilder.Entity<PetStore>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__PetStore__3214EC078150FCD2");

            entity.ToTable("PetStore");

            entity.Property(e => e.Id).ValueGeneratedNever();
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
            entity.Property(e => e.UpdateAt).HasColumnType("datetime");

            entity.HasOne(d => d.User).WithMany(p => p.PetStores)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PetStore__UserId__6477ECF3");
        });

        modelBuilder.Entity<PetStoreProduct>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__PetStore__3214EC07C3C97809");

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
                .HasConstraintName("FK__PetStoreP__Categ__66603565");

            entity.HasOne(d => d.PetStore).WithMany(p => p.PetStoreProducts)
                .HasForeignKey(d => d.PetStoreId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PetStoreP__PetSt__6754599E");
        });

        modelBuilder.Entity<PetStoreProductItem>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__PetStore__3214EC0786447503");

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
                .HasConstraintName("FK__PetStoreP__Produ__7A672E12");
        });

        modelBuilder.Entity<PetStoreRating>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__PetStore__3214EC07B273D7A9");

            entity.ToTable("PetStoreRating");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Comment)
                .HasMaxLength(1000)
                .IsUnicode(false);
            entity.Property(e => e.Rating).HasColumnType("decimal(3, 1)");

            entity.HasOne(d => d.PetStore).WithMany(p => p.PetStoreRatings)
                .HasForeignKey(d => d.PetStoreId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PetStoreR__PetSt__7C4F7684");

            entity.HasOne(d => d.User).WithMany(p => p.PetStoreRatings)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PetStoreR__UserI__7B5B524B");
        });

        modelBuilder.Entity<Post>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Post__3214EC0723BED7AC");

            entity.ToTable("Post");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Address)
                .HasMaxLength(1000)
                .IsUnicode(false);
            entity.Property(e => e.Attachment).IsUnicode(false);
            entity.Property(e => e.Content)
                .HasMaxLength(8000)
                .IsUnicode(false);
            entity.Property(e => e.CreateAt).HasColumnType("datetime");
            entity.Property(e => e.Hashtag)
                .HasMaxLength(1000)
                .IsUnicode(false);
            entity.Property(e => e.Phone)
                .HasMaxLength(12)
                .IsUnicode(false);
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Type)
                .HasMaxLength(1000)
                .IsUnicode(false);
            entity.Property(e => e.UpdateAt).HasColumnType("datetime");

            entity.HasOne(d => d.User).WithMany(p => p.Posts)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Post__UserId__70DDC3D8");
        });

        modelBuilder.Entity<PostAttachment>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__PostAtta__3214EC077C02AAF3");

            entity.ToTable("PostAttachment");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Attachment).IsUnicode(false);

            entity.HasOne(d => d.Post).WithMany(p => p.PostAttachments)
                .HasForeignKey(d => d.PostId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PostAttac__PostI__6A30C649");
        });

        modelBuilder.Entity<PostHashtag>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__PostHash__3214EC077AA51F30");

            entity.ToTable("PostHashtag");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Hashtag)
                .HasMaxLength(150)
                .IsUnicode(false);

            entity.HasOne(d => d.Post).WithMany(p => p.PostHashtags)
                .HasForeignKey(d => d.PostId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PostHasht__PostI__693CA210");
        });

        modelBuilder.Entity<PostReaction>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__PostReac__3214EC073F666F9C");

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
                .HasConstraintName("FK__PostReact__PostI__6FE99F9F");

            entity.HasOne(d => d.User).WithMany(p => p.PostReactions)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PostReact__UserI__75A278F5");
        });

        modelBuilder.Entity<PostStored>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__PostStor__3214EC07A6E32973");

            entity.ToTable("PostStored");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.CreateAt).HasColumnType("datetime");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasOne(d => d.Post).WithMany(p => p.PostStoreds)
                .HasForeignKey(d => d.PostId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PostStore__PostI__72C60C4A");

            entity.HasOne(d => d.User).WithMany(p => p.PostStoreds)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PostStore__UserI__71D1E811");
        });

        modelBuilder.Entity<ProductRating>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ProductR__3214EC0735BCC4D7");

            entity.ToTable("ProductRating");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Comment)
                .HasMaxLength(1000)
                .IsUnicode(false);
            entity.Property(e => e.Rating).HasColumnType("decimal(3, 1)");

            entity.HasOne(d => d.Product).WithMany(p => p.ProductRatings)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ProductRa__Produ__7E37BEF6");

            entity.HasOne(d => d.User).WithMany(p => p.ProductRatings)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ProductRa__UserI__7D439ABD");
        });

        modelBuilder.Entity<Report>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Report__3214EC07FB896BC3");

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
                .HasConstraintName("FK__Report__CommentI__6D0D32F4");

            entity.HasOne(d => d.Post).WithMany(p => p.Reports)
                .HasForeignKey(d => d.PostId)
                .HasConstraintName("FK__Report__PostId__6C190EBB");

            entity.HasOne(d => d.User).WithMany(p => p.Reports)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Report__UserId__6B24EA82");
        });

        modelBuilder.Entity<Transaction>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Transact__3214EC0777AA0F5B");

            entity.ToTable("Transaction");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.CreateAt).HasColumnType("datetime");
            entity.Property(e => e.PaymentMethod)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Status)
                .HasMaxLength(30)
                .IsUnicode(false);
            entity.Property(e => e.UpdateAt).HasColumnType("datetime");

            entity.HasOne(d => d.Order).WithMany(p => p.Transactions)
                .HasForeignKey(d => d.OrderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Transacti__Order__628FA481");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__User__3214EC07BC4FB036");

            entity.ToTable("User");

            entity.Property(e => e.Id).ValueGeneratedNever();
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

        modelBuilder.Entity<UserBankInformation>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__UserBank__3214EC07827E7219");

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
                .HasConstraintName("FK__UserBankI__UserI__60A75C0F");
        });

        modelBuilder.Entity<UserFollowing>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__UserFoll__3214EC07AD6757D1");

            entity.ToTable("UserFollowing");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasOne(d => d.Follower).WithMany(p => p.UserFollowingFollowers)
                .HasForeignKey(d => d.FollowerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__UserFollo__Follo__73BA3083");

            entity.HasOne(d => d.User).WithMany(p => p.UserFollowingUsers)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__UserFollo__UserI__74AE54BC");
        });

        modelBuilder.Entity<UserPet>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__UserPet__3214EC079DA4BA2D");

            entity.ToTable("UserPet");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Age)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Breed)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Gender)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.Name)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.Type)
                .HasMaxLength(30)
                .IsUnicode(false);
            entity.Property(e => e.Weight).HasColumnType("decimal(18, 0)");

            entity.HasOne(d => d.User).WithMany(p => p.UserPets)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__UserPet__UserId__68487DD7");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
