﻿namespace Watcher.Common.Dtos
{
    using Watcher.Common.Interfaces.Entities;

    public class RoleDto : IEntity<int>
    {
        public int Id { get; set; }
        
        public string Name { get; set; }
    }
}
