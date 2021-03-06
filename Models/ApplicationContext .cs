﻿using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AlbumManagement.Models
{
    public class ApplicationContext :DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<FileModel> Pictures { get; set; }
        public DbSet<Album> AlbumList { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<User>(entity => {
                entity.HasIndex(e => e.Login).IsUnique();
            });
        }

        public ApplicationContext(DbContextOptions<ApplicationContext> options)
                   : base(options)
        {
            Database.EnsureCreated();
        }
    }
}
