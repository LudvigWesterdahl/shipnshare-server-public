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
    internal sealed class ChatRepository : IChatRepository
    {
        private readonly ILogger<IChatRepository> logger;
        private readonly MainDbContext mainDbContext;

        internal ChatRepository(ILogger<IChatRepository> logger, MainDbContext mainDbContext)
        {
            Validate.That(logger, nameof(logger)).IsNot(null);
            this.logger = logger;
            Validate.That(mainDbContext, nameof(mainDbContext)).IsNot(null);
            this.mainDbContext = mainDbContext;
        }

        private ChatEntity GetChatEntity(Expression<Func<Chat, bool>> filter)
        {
            var chat = mainDbContext.Chats
                .Where(filter)
                .Include(c => c.ChatUsers)
                    .ThenInclude(cu => cu.User)
                .Include(c => c.Post)
                    .ThenInclude(p => p.Owner)
                .Include(c => c.Post)
                    .ThenInclude(p => p.Tags)
                .Include(c => c.ChatMessages)
                    .ThenInclude(cm => cm.FromUser)
                .First();

            return chat.ToChatEntity();
        }

        public async Task<ChatEntity> AddMessage(string chatId, DateTime createdAt, string message, long userId)
        {
            var chat = mainDbContext.Chats
                .Where(c => c.Id == chatId)
                .Include(c => c.ChatMessages)
                .First();

            var newChatMessage = new ChatMessage
            {
                Id = RepositoryUtils.NewGuidString(),
                CreatedAt = createdAt,
                ChatId = chatId,
                FromUserId = userId,
                Message = message
            };

            chat.ChatMessages.Add(newChatMessage);

            await mainDbContext.SaveChangesAsync();

            return GetChatEntity(c => c.Id == chat.Id);
        }

        public async Task<ChatEntity> AddOrUpdateUser(string chatId, long userId, bool active)
        {
            var chat = mainDbContext.Chats
                .Where(c => c.Id == chatId)
                .Include(c => c.ChatMessages)
                .First();

            var chatUser = mainDbContext.ChatUsers
                .Where(cu => cu.ChatId == chatId && cu.UserId == userId)
                .FirstOrDefault();

            if (chatUser == null)
            {
                var newChatUser = new ChatUser
                {
                    UserId = userId,
                    ChatId = chatId,
                    Active = active
                };
                chat.ChatUsers.Add(newChatUser);
            }
            else
            {
                chatUser.Active = active;
            }

            await mainDbContext.SaveChangesAsync();

            return GetChatEntity(c => c.Id == chat.Id);
        }

        public async Task<ChatEntity> Create(DateTime createdAt, string postId, IDictionary<long, bool> participants)
        {
            var chat = new Chat
            {
                Id = RepositoryUtils.NewGuidString(),
                PostId = postId,
                CreatedAt = createdAt
            };

            var chatUsers = new List<ChatUser>();
            foreach (var kv in participants)
            {
                var chatUser = new ChatUser
                {
                    UserId = kv.Key,
                    ChatId = chat.Id,
                    Active = kv.Value
                };
                chatUsers.Add(chatUser);
            }

            chat.ChatUsers = chatUsers;

            await mainDbContext.Chats.AddAsync(chat);
            await mainDbContext.SaveChangesAsync();

            return GetChatEntity(c => c.Id == chat.Id);
        }

        public Task<ChatEntity> GetById(string chatId)
        {
            return Task.Run(() =>
            {
                return GetChatEntity(c => c.Id == chatId);
            });
        }

        public Task<IEnumerable<ChatEntity>> GetByUserId(long userId)
        {
            return Task.Run(() =>
            {
                var chats = mainDbContext.ChatUsers
                    .Where(cu => cu.UserId == userId)
                    .ToList()
                    .Select(cu => cu.ChatId)
                    .Select(chatId => GetChatEntity(c => c.Id == chatId))
                    .AsEnumerable();

                return chats;
            });
        }
    }
}
