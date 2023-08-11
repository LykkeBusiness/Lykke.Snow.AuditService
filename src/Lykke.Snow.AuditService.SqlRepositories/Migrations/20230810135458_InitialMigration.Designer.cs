﻿// Copyright (c) 2023 Lykke Corp.
// See the LICENSE file in the project root for more information.
// <auto-generated />

using System;
using Lykke.Snow.AuditService.SqlRepositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Lykke.Snow.AuditService.SqlRepositories.Migrations
{
    [DbContext(typeof(AuditDbContext))]
    [Migration("20230810135458_InitialMigration")]
    partial class InitialMigration
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasDefaultSchema("audit")
                .HasAnnotation("ProductVersion", "6.0.6")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("Lykke.Snow.AuditService.SqlRepositories.Entities.AuditEventEntity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("AuditEventTypeDetails")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("nvarchar(200)");

                    b.Property<string>("CorrelationId")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("DataDiff")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("DataReference")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("nvarchar(200)");

                    b.Property<string>("DataType")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<DateTime>("Timestamp")
                        .HasColumnType("datetime2");

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("UserName")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Events", "audit");
                });

            modelBuilder.Entity("Lykke.Snow.AuditService.SqlRepositories.Entities.AuditObjectStateEntity", b =>
                {
                    b.Property<int>("Oid")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Oid"), 1L, 1);

                    b.Property<string>("DataReference")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("DataType")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<DateTime>("LastModified")
                        .HasColumnType("datetime2");

                    b.Property<string>("StateInJson")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Oid");

                    b.HasIndex("DataType", "DataReference")
                        .IsUnique();

                    b.ToTable("AuditObjectStates", "audit");
                });
#pragma warning restore 612, 618
        }
    }
}
