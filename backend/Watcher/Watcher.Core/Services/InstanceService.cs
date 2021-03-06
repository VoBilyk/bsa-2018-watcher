﻿using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ServiceBus.Shared.Messages;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Watcher.Common.Dtos;
using Watcher.Common.Requests;
using Watcher.Core.Interfaces;
using Watcher.DataAccess.Entities;
using Watcher.DataAccess.Interfaces;

namespace Watcher.Core.Services
{
    public class InstanceService : IInstanceService
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;
        private readonly IQueueProvider _serviceBus;

        public InstanceService(IUnitOfWork uow, IMapper mapper, IQueueProvider serviceBus)
        {
            _uow = uow;
            _mapper = mapper;
            _serviceBus = serviceBus;
        }

        public async Task<IEnumerable<InstanceDto>> GetAllEntitiesAsync()
        {
            var entities = await _uow.InstanceRepository.GetRangeAsync(count: await _uow.InstanceRepository.CountAsync(o => o.Id >= 0),
                include: x => x
                .Include(o => o.Organization)
                .Include(o => o.Dashboards));

            if (entities == null)
            {
                return null;
            }

            var dtos = _mapper.Map<List<Instance>, List<InstanceDto>>(entities);

            return dtos;
        }

        public async Task<InstanceDto> GetEntityByIdAsync(int id)
        {
            var entity = await _uow.InstanceRepository.GetFirstOrDefaultAsync(predicate: s => s.Id == id,
                include: x => x.Include(o => o.Dashboards));

            if (entity == null)
            {
                return null;
            }

            var dto = _mapper.Map<Instance, InstanceDto>(entity);

            return dto;
        }

        public async Task<IEnumerable<InstanceDto>> GetEntitiesByOrganizationIdAsync(int id)
        {
            var entities = await _uow.InstanceRepository.GetRangeAsync(
                filter: i => i.OrganizationId == id);

            if (entities == null)
            {
                return null;
            }

            var dtos = _mapper.Map<List<Instance>, List<InstanceDto>>(entities);

            return dtos;
        }

        public async Task<InstanceDto> CreateEntityAsync(InstanceRequest request)
        {
            var entity = _mapper.Map<InstanceRequest, Instance>(request);
            entity.GuidId = Guid.NewGuid();

            entity = await _uow.InstanceRepository.CreateAsync(entity);

            var result = await _uow.SaveAsync();
            if (!result)
            {
                return null;
            }

            if (entity == null)
            {
                return null;
            }

            InstanceSettingsMessage message = new InstanceSettingsMessage()
            {
                InstanceId = entity.GuidId,
                IsActive = entity.IsActive,
                AggregationForHour = entity.AggregationForHour,
                AggregationForDay = entity.AggregationForDay,
                AggregationForWeek = entity.AggregationForWeek,
                AggregationForMonth = entity.AggregationForMonth,
                CpuMaxPercent = entity.CpuMaxPercent,
                RamMaxPercent = entity.CpuMaxPercent,
                DiskMaxPercent = entity.DiskMaxPercent
            };
            await _serviceBus.SendInstanceSettingsAsync(message);

            var dto = _mapper.Map<Instance, InstanceDto>(entity);

            return dto;
        }

        public async Task<bool> UpdateEntityByIdAsync(InstanceRequest request, int id)
        {
            var entity = _mapper.Map<InstanceRequest, Instance>(request);
            entity.Id = id;

            // In returns updated entity, you could do smth with it or just leave as it is
            var updated = await _uow.InstanceRepository.UpdateAsync(entity);
            var result = await _uow.SaveAsync();

            InstanceSettingsMessage message = new InstanceSettingsMessage()
            {
                InstanceId = updated.GuidId,
                IsActive = updated.IsActive,
                AggregationForHour = updated.AggregationForHour,
                AggregationForDay = updated.AggregationForDay,
                AggregationForWeek = updated.AggregationForWeek,
                AggregationForMonth = updated.AggregationForMonth,
                CpuMaxPercent = updated.CpuMaxPercent,
                RamMaxPercent = updated.CpuMaxPercent,
                DiskMaxPercent = updated.DiskMaxPercent
            };
            await _serviceBus.SendInstanceSettingsAsync(message);

            return result;
        }

        public async Task<bool> DeleteEntityByIdAsync(int id)
        {
            await _uow.InstanceRepository.DeleteAsync(id,
                instances =>
                instances.Include(i => i.Dashboards)
                            .ThenInclude(d => d.Charts));

            var result = await _uow.SaveAsync();

            return result;
        }
    }
}
