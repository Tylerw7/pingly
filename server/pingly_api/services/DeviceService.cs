using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using pingly_api.Data;
using pingly_api.DTOs;
using pingly_api.DTOs.Devices;
using pingly_api.Models;

namespace pingly_api.services
{
    public class DeviceService
    {
        private readonly AppDbContext _context;

        public DeviceService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<DeviceResponseDto> RegisterAsync(RegisterDeviceRequestDto request)
        {
            var device = await _context.Devices
                .FirstOrDefaultAsync(x => x.ApnsToken == request.ApnsToken);

            if (device != null)
            {
                device.LastSeenAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                return Map(device);
            }

            device = new Device
            {
                Id = Guid.NewGuid(),
                ApnsToken = request.ApnsToken,
                Platform = "ios",
                CreatedAt = DateTime.UtcNow,
                LastSeenAt = DateTime.UtcNow,
                IsActive = true
            };

            _context.Devices.Add(device);

            await _context.SaveChangesAsync();

            return Map(device);
        }

        public async Task<bool> UpdateTokenAsync(
            Guid deviceId,
            UpdateDeviceRequestDto request)
        {
            var device = await _context.Devices.FindAsync(deviceId);

            if (device == null)
                return false;

            device.ApnsToken = request.ApnsToken;
            device.LastSeenAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> SubscribeAsync(
            Guid deviceId,
            string topicName)
        {
            var device = await _context.Devices.FindAsync(deviceId);

            if (device == null)
                return false;

            var topic = await _context.Topics
                .FirstOrDefaultAsync(x => x.Name == topicName);

            if (topic == null)
                return false;

            var exists = await _context.DeviceSubscriptions.AnyAsync(x =>
                x.DeviceId == deviceId &&
                x.TopicId == topic.Id);

            if (exists)
                return true;

            _context.DeviceSubscriptions.Add(
                new DeviceSubscription
                {
                    Id = Guid.NewGuid(),
                    DeviceId = deviceId,
                    TopicId = topic.Id,
                    CreatedAt = DateTime.UtcNow
                });

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> UnsubscribeAsync(
            Guid deviceId,
            string topicName)
        {
            var topic = await _context.Topics
                .FirstOrDefaultAsync(x => x.Name == topicName);

            if (topic == null)
                return false;

            var subscription = await _context.DeviceSubscriptions
                .FirstOrDefaultAsync(x =>
                    x.DeviceId == deviceId &&
                    x.TopicId == topic.Id);

            if (subscription == null)
                return false;

            _context.DeviceSubscriptions.Remove(subscription);

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<List<string>> GetSubscriptionsAsync(Guid deviceId)
        {
            return await _context.DeviceSubscriptions
                .Where(x => x.DeviceId == deviceId)
                .Select(x => x.Topic.Name)
                .ToListAsync();
        }

        private static DeviceResponseDto Map(Device device)
        {
            return new DeviceResponseDto
            {
                Id = device.Id,
                CreatedAt = device.CreatedAt,
                IsActive = device.IsActive
            };
        }
    }
}