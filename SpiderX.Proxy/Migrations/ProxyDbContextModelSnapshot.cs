﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SpiderX.Proxy;

namespace SpiderX.Proxy.Migrations
{
    [DbContext(typeof(ProxyDbContext))]
    partial class ProxyDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.1.3-rtm-32065")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("SpiderX.Proxy.SpiderProxyEntity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("AnonymityDegree");

                    b.Property<int>("Category");

                    b.Property<string>("Host")
                        .HasColumnType("VARCHAR(32)");

                    b.Property<string>("Location");

                    b.Property<int>("Port");

                    b.Property<int>("ResponseMilliseconds");

                    b.HasKey("Id");

                    b.HasIndex("Host", "Port")
                        .IsUnique()
                        .HasFilter("[Host] IS NOT NULL");

                    b.ToTable("ProxyEntities");
                });
#pragma warning restore 612, 618
        }
    }
}