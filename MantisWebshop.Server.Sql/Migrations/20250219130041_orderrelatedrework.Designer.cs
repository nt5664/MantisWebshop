﻿// <auto-generated />
using System;
using MantisWebshop.Server.Sql.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace MantisWebshop.Server.Sql.Migrations
{
    [DbContext(typeof(MantisWebshopDbContext))]
    [Migration("20250219130041_orderrelatedrework")]
    partial class orderrelatedrework
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.2")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("MantisWebshop.Server.Sql.Models.CartItem", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("ProductId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("Quantity")
                        .HasColumnType("int");

                    b.Property<Guid?>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("ProductId");

                    b.HasIndex("UserId");

                    b.ToTable("cartItems", (string)null);
                });

            modelBuilder.Entity("MantisWebshop.Server.Sql.Models.Order", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid?>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("order", (string)null);
                });

            modelBuilder.Entity("MantisWebshop.Server.Sql.Models.Product", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid?>("CreatorId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ImageUrl")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<float>("Price")
                        .HasColumnType("real");

                    b.Property<int>("Version")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("CreatorId");

                    b.ToTable("products", (string)null);
                });

            modelBuilder.Entity("MantisWebshop.Server.Sql.Models.ProductSnapshot", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid?>("OrderId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<float>("Price")
                        .HasColumnType("real");

                    b.Property<Guid>("ProductId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("SnapshotVersion")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("OrderId");

                    b.ToTable("productSnapshots", (string)null);
                });

            modelBuilder.Entity("MantisWebshop.Server.Sql.Models.User", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("users", (string)null);
                });

            modelBuilder.Entity("MantisWebshop.Server.Sql.Models.CartItem", b =>
                {
                    b.HasOne("MantisWebshop.Server.Sql.Models.Product", "Product")
                        .WithMany()
                        .HasForeignKey("ProductId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("MantisWebshop.Server.Sql.Models.User", "User")
                        .WithMany("CartItems")
                        .HasForeignKey("UserId");

                    b.Navigation("Product");

                    b.Navigation("User");
                });

            modelBuilder.Entity("MantisWebshop.Server.Sql.Models.Order", b =>
                {
                    b.HasOne("MantisWebshop.Server.Sql.Models.User", "User")
                        .WithMany("Orders")
                        .HasForeignKey("UserId");

                    b.Navigation("User");
                });

            modelBuilder.Entity("MantisWebshop.Server.Sql.Models.Product", b =>
                {
                    b.HasOne("MantisWebshop.Server.Sql.Models.User", "Creator")
                        .WithMany("Products")
                        .HasForeignKey("CreatorId");

                    b.Navigation("Creator");
                });

            modelBuilder.Entity("MantisWebshop.Server.Sql.Models.ProductSnapshot", b =>
                {
                    b.HasOne("MantisWebshop.Server.Sql.Models.Order", "Order")
                        .WithMany("ProductSnapshots")
                        .HasForeignKey("OrderId");

                    b.Navigation("Order");
                });

            modelBuilder.Entity("MantisWebshop.Server.Sql.Models.Order", b =>
                {
                    b.Navigation("ProductSnapshots");
                });

            modelBuilder.Entity("MantisWebshop.Server.Sql.Models.User", b =>
                {
                    b.Navigation("CartItems");

                    b.Navigation("Orders");

                    b.Navigation("Products");
                });
#pragma warning restore 612, 618
        }
    }
}
