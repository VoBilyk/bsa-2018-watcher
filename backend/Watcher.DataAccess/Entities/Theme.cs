﻿using Watcher.Common.Interfaces.Entities;

namespace Watcher.DataAccess.Entities
{
    using System.ComponentModel.DataAnnotations;

    public class Theme : Entity<int>, ISoftDeletable
    {
        public override int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public string FontFamily { get; set; }

        public string BackgroundColor { get; set; }

        [Required]
        public bool IsDeleted { get; set; }

        public void OnDelete()
        {
            IsDeleted = true;
        }
    }
}
