using auth_services.AuthService.Application.DTOS;
using auth_services.AuthService.Application.Interfaces;
using auth_services.AuthService.Infrastructure.DbContextAuth;
using auth_services.AuthService.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;

namespace auth_services.AuthService.Infrastructure.Repository
{
    public class OutBoxMessage : IOutBoxMessage
    {
        private readonly FoodAuthContext _db;

        public OutBoxMessage(FoodAuthContext foodAuthContext)
        {
            _db = foodAuthContext;
        }

        public async Task CreateNewMessage(OutBoxMessageInternalDTO message)
        {

            var newMessage = new OutBoxMessageTable
            {
                Id = message.Id,
                TypeMessage = message.TypeMessage,
                PayLoad = message.Payload,
                IsProccessed = message.IsProccessed,
                CreateAt = message.CreateAt

            };

            await _db.OutBoxMessageTables.AddAsync(newMessage);
        }

        public async Task<List<OutBoxMessageInternalDTO>> GetMessage()
        {
            var listMessage = await _db.OutBoxMessageTables.Where(s => s.IsProccessed == false)
                .Select(s => new OutBoxMessageInternalDTO(s.Id, s.TypeMessage, s.PayLoad, s.IsProccessed, s.CreateAt))
                .ToListAsync();
            return listMessage;
        }

        public async Task MartAsProccessed(Guid IdMessage)
        {
            var message = await _db.OutBoxMessageTables.FirstOrDefaultAsync(t => t.Id == IdMessage);
            if (message != null)
            {
                message.IsProccessed = true;
                await _db.SaveChangesAsync();
            }
        }
    }
}
