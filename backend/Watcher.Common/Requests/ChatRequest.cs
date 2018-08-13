﻿namespace Watcher.Common.Requests
{
    using System.Collections.Generic;

    using Watcher.Common.Dtos;
    using Watcher.Common.Enums;

    public class ChatRequest
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public ChatType Type { get; set; }
            
        public UserDto CreatedBy { get; set; }

        public OrganizationDto Organization { get; set; }

        public IList<MessageDto> Messages { get; set; }
    }
}
