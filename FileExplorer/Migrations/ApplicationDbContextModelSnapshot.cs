﻿// <auto-generated />
using System;
using FileExplorer;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace FileExplorer.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    partial class ApplicationDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "9.0.0-preview.4.24267.1");

            modelBuilder.Entity("FileExplorer.Models.Entities.File", b =>
                {
                    b.Property<int>("FileId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Contributor")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Coverage")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("TEXT");

                    b.Property<int>("CreatedByUserId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Creator")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("Date")
                        .HasColumnType("TEXT");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("FileName")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Format")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Identifier")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Language")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Publisher")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Relation")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Rights")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Source")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Subject")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("FileId");

                    b.HasIndex("CreatedByUserId");

                    b.ToTable("Files");
                });

            modelBuilder.Entity("FileExplorer.Models.Entities.FileModificationHistory", b =>
                {
                    b.Property<int>("FileModificationHistoryId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("FileId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("ModificationDetails")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("ModifiedAt")
                        .HasColumnType("TEXT");

                    b.Property<string>("ModifiedBy")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("FileModificationHistoryId");

                    b.HasIndex("FileId");

                    b.ToTable("FileModificationHistories");
                });

            modelBuilder.Entity("FileExplorer.Models.Entities.Notification", b =>
                {
                    b.Property<int>("NotificationId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("TEXT");

                    b.Property<string>("Message")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("UserId")
                        .HasColumnType("INTEGER");

                    b.HasKey("NotificationId");

                    b.HasIndex("UserId");

                    b.ToTable("Notifications");
                });

            modelBuilder.Entity("FileExplorer.Models.Entities.User", b =>
                {
                    b.Property<int>("UserId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("IpAddress")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<bool>("IsBlocked")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Role")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("UserId");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("FileExplorer.Models.Entities.UserFilePermission", b =>
                {
                    b.Property<int>("UserId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("FileId")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("CanDownload")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("CanRead")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("CanSendNotifications")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("CanUpload")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("CanWrite")
                        .HasColumnType("INTEGER");

                    b.HasKey("UserId", "FileId");

                    b.HasIndex("FileId");

                    b.ToTable("UserFilePermissions");
                });

            modelBuilder.Entity("FileExplorer.Models.Entities.File", b =>
                {
                    b.HasOne("FileExplorer.Models.Entities.User", "CreatedBy")
                        .WithMany("CreatedFiles")
                        .HasForeignKey("CreatedByUserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("CreatedBy");
                });

            modelBuilder.Entity("FileExplorer.Models.Entities.FileModificationHistory", b =>
                {
                    b.HasOne("FileExplorer.Models.Entities.File", "File")
                        .WithMany("ModificationHistory")
                        .HasForeignKey("FileId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("File");
                });

            modelBuilder.Entity("FileExplorer.Models.Entities.Notification", b =>
                {
                    b.HasOne("FileExplorer.Models.Entities.User", "User")
                        .WithMany("Notifications")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("FileExplorer.Models.Entities.UserFilePermission", b =>
                {
                    b.HasOne("FileExplorer.Models.Entities.File", "File")
                        .WithMany("UserFilePermissions")
                        .HasForeignKey("FileId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("FileExplorer.Models.Entities.User", "User")
                        .WithMany("UserFilePermissions")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("File");

                    b.Navigation("User");
                });

            modelBuilder.Entity("FileExplorer.Models.Entities.File", b =>
                {
                    b.Navigation("ModificationHistory");

                    b.Navigation("UserFilePermissions");
                });

            modelBuilder.Entity("FileExplorer.Models.Entities.User", b =>
                {
                    b.Navigation("CreatedFiles");

                    b.Navigation("Notifications");

                    b.Navigation("UserFilePermissions");
                });
#pragma warning restore 612, 618
        }
    }
}
