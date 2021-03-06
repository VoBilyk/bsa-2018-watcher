﻿namespace Watcher.Core.Services
{
    using AutoMapper;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Watcher.Common.Dtos;
    using Watcher.Common.Enums;
    using Watcher.Common.Requests;
    using Watcher.Core.Interfaces;
    using Watcher.DataAccess.Entities;
    using Watcher.DataAccess.Interfaces;

    public class NotificationSettingsService : INotificationSettingsService
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;
        public NotificationSettingsService(IUnitOfWork uow, IMapper mapper)
        {
            _uow = uow;
            _mapper = mapper;
        }

        public async Task<IEnumerable<NotificationSettingDto>> GetEntitysByUserIdAsync(string userId)
        {
            var entity = await _uow.NotificationSettingsRepository.GetRangeAsync(1, int.MaxValue, x => x.UserId == userId && x.Type != NotificationType.Chat);

            if (entity == null)
            {
                return null;
            }

            var dto = _mapper.Map<IEnumerable<NotificationSetting>, IEnumerable<NotificationSettingDto>>(entity);

            return dto;
        }

        public async Task<bool> UpdateEntitiesAsync(IEnumerable<NotificationSettingUpdateRequest> requests)
        {
            var entities = _mapper.Map<IEnumerable<NotificationSettingUpdateRequest>, IEnumerable<NotificationSetting>>(requests);

            foreach (var notificationSetting in entities)
            {
                await _uow.NotificationSettingsRepository.UpdateAsync(notificationSetting);
            }

            // In returns updated entity, you could do smth with it or just leave as it is
            var result = await _uow.SaveAsync();

            return result;
        }

        public async Task<NotificationSetting> CreateEntityAsync(NotificationSettingRequest request, string userId)
        {

            var entity = new NotificationSetting()
            {
                UserId = userId,
                IsDisable = request.IsDisable,
                IsEmailable = request.IsEmailable,
                IsMute = request.IsMute,
                Type = request.Type,
            };

            await _uow.NotificationSettingsRepository.CreateAsync(entity);
            await _uow.SaveAsync();

            return entity;
        }

        public async Task<bool> UpdateEntityByIdAsync(NotificationSettingRequest request, int id)
        {
            var entity = _mapper.Map<NotificationSettingRequest, NotificationSetting>(request);
            entity.Id = id;

            // In returns updated entity, you could do smth with it or just leave as it is
            var updated = await _uow.NotificationSettingsRepository.UpdateAsync(entity);
            var result = await _uow.SaveAsync();

            return result;
        }
    }
}
