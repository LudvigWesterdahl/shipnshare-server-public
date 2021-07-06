using System;
using System.Collections.Generic;
using System.Linq;
using ShipWithMeCore.SharedKernel;

namespace ShipWithMeCore.Entities
{
    public sealed class ChatEntity
    {
        /// <summary>
        /// The id of the chat.
        /// </summary>
        public string Id { get; }

        /// <summary>
        /// The date and time the chat was created.
        /// </summary>
        public DateTime CreatedAt { get; }

        /// <summary>
        /// The post this chat is about.
        /// </summary>
        public PostEntity Post { get; }

        /// <summary>
        /// The participants in the chat and whether or not they have left.
        /// </summary>
        public IDictionary<UserEntity, bool> Participants { get; }

        /// <summary>
        /// The messages in this chat.
        /// </summary>
        public IList<ChatMessageEntity> Messages { get; }

        /// <summary>
        /// If this chat is closed or not.
        /// </summary>
        public bool Closed { get; }

        public ChatEntity(
            string id,
            DateTime createdAt,
            PostEntity post,
            IDictionary<UserEntity, bool> participants,
            IEnumerable<ChatMessageEntity> messages)
        {
            Validate.That(id, nameof(id)).IsNot(null);
            Id = id;

            Validate.That(createdAt, nameof(createdAt)).IsNotGreaterThan(DateTime.UtcNow);
            CreatedAt = createdAt;

            Validate.That(post, nameof(post)).IsNot(null);
            Post = post;

            Validate.That(participants, nameof(participants)).IsNot(null);
            var participantsUserIds = participants.Select(kv => kv.Key.Id).ToHashSet();
            Validate.That(participantsUserIds.Count, "unique user Ids").Is(participantsUserIds.Count);
            Participants = new Dictionary<UserEntity, bool>(participants);

            Validate.That(messages, nameof(messages)).IsNot(null);
            foreach (var message in messages)
            {
                if (!participantsUserIds.Contains(message.User.Id))
                {
                    throw new ArgumentException(
                        $"User {message.User} is not a participant in the chat and cannot send messages.");
                }
            }
            Messages = new List<ChatMessageEntity>(messages);

            Closed = participants.Select(kv => kv.Value).Where(active => active).Count() < 2;
        }
    }
}
