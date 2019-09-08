﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Persistence.Migrations
{
    [DbContext(typeof(DataContext))]
    [Migration("20190904200718_AddValues")]
    partial class AddValues
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.2.6-servicing-10079");

            modelBuilder.Entity("Domain.Value", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name");

                    b.HasKey("ID");

                    b.ToTable("Values");

                    b.HasData(
                        new
                        {
                            ID = 1,
                            Name = "Name 1"
                        },
                        new
                        {
                            ID = 2,
                            Name = "Name 2"
                        },
                        new
                        {
                            ID = 3,
                            Name = "Name 3"
                        });
                });
#pragma warning restore 612, 618
        }
    }
}
