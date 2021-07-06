using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ShipWithMeCore.Entities;
using ShipWithMeCore.Repositories;
using ShipWithMeCore.SharedKernel;
using ShipWithMeInfrastructure.Models;

namespace ShipWithMeInfrastructure.Repositories
{
    internal sealed class ChatRequestRepository : IChatRequestRepository
    {
        private readonly ILogger<IChatRequestRepository> logger;
        private readonly MainDbContext mainDbContext;

        internal ChatRequestRepository(ILogger<IChatRequestRepository> logger, MainDbContext mainDbContext)
        {
            Validate.That(logger, nameof(logger)).IsNot(null);
            this.logger = logger;
            Validate.That(mainDbContext, nameof(mainDbContext)).IsNot(null);
            this.mainDbContext = mainDbContext;
        }

        private IEnumerable<ChatRequestEntity> GetChatRequestEntity(Expression<Func<ChatRequest, bool>> filter)
        {
            var chatRequests = mainDbContext.ChatRequests
                .Where(filter)
                .Include(cr => cr.FromUser)
                .Include(cr => cr.ToUser)
                .Include(cr => cr.Chat)
                    .ThenInclude(c => c.ChatUsers)
                        .ThenInclude(cu => cu.User)
                .Include(cr => cr.Chat)
                    .ThenInclude(c => c.Post)
                        .ThenInclude(p => p.Owner)
                .Include(cr => cr.Chat)
                    .ThenInclude(c => c.Post)
                        .ThenInclude(p => p.Tags)
                .Include(cr => cr.Chat)
                    .ThenInclude(c => c.ChatMessages)
                        .ThenInclude(cm => cm.FromUser)
                .AsEnumerable()
                .Select(cr => cr.ToChatRequestEntity());

            return chatRequests;
        }

        private ChatRequestEntity GetChatRequestEntity(string chatRequestId)
        {
            return GetChatRequestEntity(cr => cr.Id == chatRequestId).First();
        }

        public async Task<ChatRequestEntity> Create(
            DateTime dateTime, long fromUserId, long toUserId, string chatId, bool accepted)
        {

            var chatRequest = new ChatRequest
            {
                Id = RepositoryUtils.NewGuidString(),
                CreatedAt = dateTime,
                FromUserId = fromUserId,
                ToUserId = toUserId,
                ChatId = chatId,
                Accepted = accepted
            };

            await mainDbContext.ChatRequests.AddAsync(chatRequest);
            await mainDbContext.SaveChangesAsync();

            return GetChatRequestEntity(cr => cr.Id == chatRequest.Id).First();
        }

        public Task<IEnumerable<ChatRequestEntity>> GetByChatId(string chatId)
        {
            return Task.Run(() =>
            {
                var chatRequests = mainDbContext.ChatRequests
                    .Where(cr => cr.ChatId == chatId)
                    .ToList()
                    .Select(cr => cr.Id)
                    .Select(chatRequestId => GetChatRequestEntity(chatRequestId))
                    .AsEnumerable();

                var chatRequests2 = GetChatRequestEntity(cr => cr.ChatId == chatId);

                return chatRequests2;
            });
        }

        public Task<ChatRequestEntity> GetById(string chatRequestId)
        {
            return Task.Run(() =>
            {
                return GetChatRequestEntity(chatRequestId);
            });
        }

        public Task<IEnumerable<ChatRequestEntity>> GetByUserId(long userId)
        {
            return Task.Run(() =>
            {
                
                var chatRequests = mainDbContext.ChatRequests
                    .Where(cr => cr.FromUserId == userId || cr.ToUserId == userId)
                    .ToList()
                    .Select(cr => cr.Id)
                    .Select(chatRequestId => GetChatRequestEntity(chatRequestId))
                    .AsEnumerable();
                

                var chatRequests2 = GetChatRequestEntity(cr => cr.FromUserId == userId || cr.ToUserId == userId);

                return chatRequests2;
            });
        }

        public async Task<bool> Update(ChatRequestEntity chatRequestEntity)
        {
            var chatRequest = mainDbContext.ChatRequests
                .Where(cr => cr.Id == chatRequestEntity.Id)
                .First();

            chatRequest.FromUserId = chatRequestEntity.FromUser.Id;
            chatRequest.ToUserId = chatRequestEntity.ToUser.Id;
            chatRequest.ChatId = chatRequestEntity.Chat.Id;
            chatRequest.Accepted = chatRequestEntity.Accepted;

            await mainDbContext.SaveChangesAsync();

            return true;
        }
    }
}
