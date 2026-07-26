using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http.HttpResults;


namespace pingly_api.Endpoints
{
    public static class TopicEndpoints
    {
        private static readonly Regex TopicNameRegex = new(
            @"^[a-zA-Z0-9_-]{1,64}$", RegexOptions.Compiled
        );

        private const int MaxBodyBytes = 8 * 1024;

        public static void MapTopicEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/topics");

            group.MapGet("/health", HealthCheck);
        }

        public static IResult HealthCheck()
        {
            return Results.Ok(new
            {
                status = "healthy api."
            });
        }
    }
}