﻿using System;
using Microsoft.EntityFrameworkCore;
using Domain;

namespace Persistence
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions options) : base(options)
        {
            
        }

        public DbSet<Value> Values { get; set; }

        protected override void OnModelCreating(ModelBuilder model){
            model.Entity<Value>()
                .HasData(
                    new Value {ID = 1, Name = "Name 1"},
                    new Value {ID = 2, Name = "Name 2"},
                    new Value {ID = 3, Name = "Name 3"}
                );
        }
    }
}
