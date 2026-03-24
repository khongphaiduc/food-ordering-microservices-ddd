using Microsoft.EntityFrameworkCore;
using tracking_service.Tracking.Application.Interface;
using tracking_service.Tracking.Infastructrure.Models;
using tracking_service.Tracking.Domain.Enums;

namespace tracking_service.Tracking.Infastructrure.ImplementServices
{
    public class GetBehaviourOfUser : IGetBehaviourOfUser
    {
        private readonly FoodProductsDbContext _db;

        public GetBehaviourOfUser(FoodProductsDbContext foodProductsDbContext)
        {
            _db = foodProductsDbContext;
        }

        public async Task<BehaviourUser> Execute(Guid IdUser)
        {

            var latestSessionId = await _db.UserSessions
                .Where(s => s.UserId == IdUser)
                .OrderByDescending(s => s.StartedAt)
                .Select(s => s.Id)
                .FirstOrDefaultAsync();

            if (latestSessionId == Guid.Empty) return new BehaviourUser();

            var behaviorData = await _db.TrackingEvents
                .Where(e => e.SessionId == latestSessionId)
                .GroupBy(e => new { e.EventType, e.ProductId })
                .Select(g => new DataOfUsers
                {

                    eventType = g.Key.EventType ?? "None",
                    IdProduct = g.Key.ProductId ?? Guid.Empty,
                    CountTimes = g.Count()
                })
                .ToListAsync();


            return new BehaviourUser
            {
                IdUser = IdUser,
                BehaviourUsers = behaviorData,

            };
        }
    }
}