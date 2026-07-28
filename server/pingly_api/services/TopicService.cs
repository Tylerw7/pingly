using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using pingly_api.Data;
using pingly_api.DTOs;
using pingly_api.Models;

namespace pingly_api.services
{
    public class TopicService
    {
        private readonly AppDbContext _context;

        public TopicService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<TopicResponseDto> CreateAsync(CreateTopicRequestDto request)
        {
            var topicName = GenerateTopicName(request.Name);

            var topic = new Topic
            {
                Id = Guid.NewGuid(),
                Name = topicName,
                Description = request.Description,
                IsPublic = request.IsPublic,
                CreatedAt = DateTime.UtcNow
            };

                _context.Topics.Add(topic);
                await _context.SaveChangesAsync();

                return MapResponse(topic);
        }


        public async Task<Topic?> GetAsync(string name)
        {
            return await _context.Topics
                .FirstOrDefaultAsync(x => x.Name == name);
        }

        public async Task<List<TopicResponseDto>> GetAllAsync()
        {
            return await _context.Topics
                .Select(x => MapResponse(x))
                .ToListAsync();
        }

        public async Task<bool> DeleteAsync(string name)
        {
            var topic = await GetAsync(name);

            if (topic == null) return false;

            _context.Topics.Remove(topic);
            await _context.SaveChangesAsync();

            return true;
        }



        private static string GenerateTopicName(string input)
        {
            var clean = input.ToLower().Replace(" ", "-");

            var suffix = Guid.NewGuid().ToString().Substring(0, 8);

            return $"{clean}-{suffix}";
        }


        private static TopicResponseDto MapResponse(Topic topic)
        {
            return new TopicResponseDto
            {
                Id = topic.Id,

                Name = topic.Name,

                Description = topic.Description,

                IsPublic = topic.IsPublic,

                CreatedAt = topic.CreatedAt,


                PublishUrl =
                    $"https://api.pingly.app/t/{topic.Name}"
            };
        }





    }
}