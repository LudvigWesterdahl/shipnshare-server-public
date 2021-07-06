using System;
using System.Collections.Generic;
using ShipWithMeCore.SharedKernel;

namespace ShipWithMeCore.Entities
{
    /// <summary>
    /// A post created by a user.
    /// </summary>
    public sealed class PostEntity
    {
        /// <summary>
        /// Container for the base post information across all types of <see cref="PostEntity"/> types.
        /// </summary>
        public sealed class GeneralPostInfo
        {
            public UserEntity Owner { get; private set; }

            public DateTime CreatedAt { get; private set; }

            public TimeZoneInfo CreatedAtTimeZone { get; private set; }

            public Tuple<double, double> PickupLocation { get; private set; }

            public string OfferValueTitle { get; private set; }

            public string Description { get; private set; }

            public Uri StoreOrProductUri { get; private set; }

            public decimal ShippingCost { get; private set; }

            public string Currency { get; private set; }

            public bool Open { get; private set; }

            private readonly List<TagEntity> tags = new List<TagEntity>();

            public IEnumerable<TagEntity> Tags { get => tags; }

            public string ImagePath { get; private set; }

            /// <summary>
            /// Sets the owner of this post.
            /// </summary>
            /// <param name="owner">the owner</param>
            /// <returns>this instance for chaining</returns>
            public GeneralPostInfo SetOwner(UserEntity owner)
            {
                Validate.That(owner, nameof(owner)).IsNot(null);

                Owner = owner;
                return this;
            }

            /// <summary>
            /// Sets the date and time this post was created.
            /// </summary>
            /// <param name="createdAt">the date and time of creation</param>
            /// <returns>this instance for chaining</returns>
            public GeneralPostInfo SetCreatedAt(DateTime createdAt)
            {
                Validate.That(createdAt, nameof(createdAt)).IsNotGreaterThan(DateTime.UtcNow);

                CreatedAt = createdAt;
                return this;
            }

            /// <summary>
            /// Sets the timezone in which the post was created.
            /// </summary>
            /// <param name="createdAtTimeZone">the timezone of creation</param>
            /// <returns>this instance for chaining</returns>
            public GeneralPostInfo SetCreatedAtTimeZone(TimeZoneInfo createdAtTimeZone)
            {
                Validate.That(createdAtTimeZone, nameof(createdAtTimeZone)).IsNot(null);

                CreatedAtTimeZone = createdAtTimeZone;
                return this;
            }

            /// <summary>
            /// Sets the pickup location of the order in latitude, longitude.
            /// </summary>
            /// <param name="pickupLocation">latitude longitude pair</param>
            /// <returns>this instance for chaining</returns>
            public GeneralPostInfo SetPickupLocation(Tuple<double, double> pickupLocation)
            {
                Validate.That(pickupLocation, nameof(pickupLocation)).IsNot(null);
                Validate.That(pickupLocation.Item1, "latitude").IsNotGreaterThan(90D);
                Validate.That(pickupLocation.Item1, "latitude").IsNotLessThan(-90D);
                Validate.That(pickupLocation.Item2, "longitude").IsNotGreaterThan(180D);
                Validate.That(pickupLocation.Item2, "longitude").IsNotLessThan(-180D);

                PickupLocation = pickupLocation;
                return this;
            }

            /// <summary>
            /// Sets the title of the post containing the value proposition.
            /// </summary>
            /// <param name="offerValueTitle"></param>
            /// <returns>this instance for chaining</returns>
            public GeneralPostInfo SetOfferValueTitle(string offerValueTitle)
            {
                Validate.That(offerValueTitle, nameof(offerValueTitle)).IsNot(null);
                Validate.That(offerValueTitle.Length, nameof(offerValueTitle.Length)).IsGreaterThan(0);

                OfferValueTitle = offerValueTitle;
                return this;
            }

            /// <summary>
            /// Sets the description of the post.
            /// </summary>
            /// <param name="description">the description</param>
            /// <returns>this instance for chaining</returns>
            public GeneralPostInfo SetDescription(string description)
            {
                Validate.That(description, nameof(description)).IsNot(null);
                Validate.That(description.Length, nameof(description.Length)).IsGreaterThan(0);

                Description = description;
                return this;
            }

            /// <summary>
            /// Sets the URL as a uri to the product/store that the post is meant for.
            /// </summary>
            /// <param name="storeOrProductUri"></param>
            /// <returns>this instance for chaining</returns>
            public GeneralPostInfo SetStoreOrProductUri(Uri storeOrProductUri)
            {
                Validate.That(storeOrProductUri, nameof(storeOrProductUri)).IsNot(null);
                Validate.That(storeOrProductUri.IsAbsoluteUri, nameof(storeOrProductUri.IsAbsoluteUri)).Is(true);
                Validate.That(storeOrProductUri.Scheme, nameof(storeOrProductUri.Scheme))
                    .IsAny(Uri.UriSchemeHttp, Uri.UriSchemeHttps);

                StoreOrProductUri = storeOrProductUri;
                return this;
            }

            /// <summary>
            /// Sets the cost of shipping that will be shared.
            /// </summary>
            /// <param name="shippingCost">the cost of shipping</param>
            /// <returns>this instance for chaining</returns>
            public GeneralPostInfo SetShippingCost(decimal shippingCost)
            {
                Validate.That(shippingCost, nameof(shippingCost)).IsNotLessThan((decimal)0);

                ShippingCost = shippingCost;
                return this;
            }

            /// <summary>
            /// Sets the currency that the post is designed for.
            /// </summary>
            /// <param name="currency">the currency</param>
            /// <returns>this instance for chaining</returns>
            public GeneralPostInfo SetCurrency(string currency)
            {
                Validate.That(currency, nameof(currency)).IsNot(null);
                Validate.That(currency.Length, nameof(currency.Length)).IsGreaterThan(0);

                Currency = currency;
                return this;
            }

            /// <summary>
            /// Sets whether or not this post is open.
            /// </summary>
            /// <param name="open">if the post is open or not</param>
            /// <returns>this instance for chaining</returns>
            public GeneralPostInfo SetOpen(bool open)
            {
                Open = open;

                return this;
            }

            /// <summary>
            /// Sets optional tags to this post.
            /// </summary>
            /// <param name="tags">the tags</param>
            /// <returns>this instance for chaining</returns>
            public GeneralPostInfo SetTags(IEnumerable<TagEntity> tags)
            {
                this.tags.Clear();

                if (tags != null)
                {
                    this.tags.AddRange(tags);
                }

                return this;
            }

            /// <summary>
            /// Adds optional tags to this post.
            /// </summary>
            /// <param name="tags">the tags</param>
            /// <returns>this instance for chaining</returns>
            public GeneralPostInfo WithTags(params TagEntity[] tags)
            {
                if (tags != null)
                {
                    foreach (var tag in tags)
                    {
                        this.tags.Add(tag);
                    }
                }

                return this;
            }

            /// <summary>
            /// Adds optional tags to this post.
            /// </summary>
            /// <param name="tags">the tags</param>
            /// <returns>this instance for chaining</returns>
            public GeneralPostInfo WithTags(IEnumerable<TagEntity> tags)
            {
                if (tags != null)
                {
                    foreach (var tag in tags)
                    {
                        this.tags.Add(tag);
                    }
                }

                return this;
            }

            /// <summary>
            /// Sets an optional path to the post image.
            /// </summary>
            /// <param name="imageUri">the path to the image</param>
            /// <returns>this instance for chaining</returns>
            public GeneralPostInfo SetImagePath(string imagePath)
            {
                ImagePath = imagePath;
                return this;
            }
        }

        /// <summary>
        /// The id of this post.
        /// </summary>
        public string Id { get; }

        public UserEntity Owner { get; }

        public DateTime CreatedAt { get; }

        public TimeZoneInfo CreatedAtTimeZone { get; }

        public Tuple<double, double> PickupLocation { get; }

        public string OfferValueTitle { get; }

        public string Description { get; }

        public Uri StoreOrProductUri { get; }

        public decimal ShippingCost { get; }

        public string Currency { get; }

        public bool Open { get; set; }

        public IEnumerable<TagEntity> Tags { get; }

        public string ImagePath { get; set; }

        /// <summary>
        /// Returns the uri to the favicon of the website.
        /// </summary>
        public Uri FaviconUri { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="id">the id</param>
        /// <param name="generalPostInfo">the generalPostInfo</param>
        private PostEntity(string id, GeneralPostInfo generalPostInfo)
        {
            Id = id;
            Owner = generalPostInfo.Owner;
            CreatedAt = generalPostInfo.CreatedAt;
            CreatedAtTimeZone = generalPostInfo.CreatedAtTimeZone;
            PickupLocation = generalPostInfo.PickupLocation;
            OfferValueTitle = generalPostInfo.OfferValueTitle;
            Description = generalPostInfo.Description;
            StoreOrProductUri = generalPostInfo.StoreOrProductUri;
            ShippingCost = generalPostInfo.ShippingCost;
            Currency = generalPostInfo.Currency;
            Open = generalPostInfo.Open;
            Tags = generalPostInfo.Tags;
            ImagePath = generalPostInfo.ImagePath;

            FaviconUri = new Uri(StoreOrProductUri.Scheme + "://" + StoreOrProductUri.DnsSafeHost + "/favicon.ico");
        }

        public static PostEntity Create(string id, GeneralPostInfo generalPostInfo)
        {
            Validate.That(generalPostInfo.Owner, nameof(generalPostInfo.Owner)).IsNot(null);

            Validate.That(generalPostInfo.CreatedAt, nameof(generalPostInfo.CreatedAt))
                .IsNotGreaterThan(DateTime.UtcNow);

            Validate.That(generalPostInfo.CreatedAtTimeZone, nameof(generalPostInfo.CreatedAtTimeZone)).IsNot(null);

            Validate.That(generalPostInfo.PickupLocation, nameof(generalPostInfo.PickupLocation)).IsNot(null);
            Validate.That(generalPostInfo.PickupLocation.Item1, "latitude").IsNotGreaterThan(90D);
            Validate.That(generalPostInfo.PickupLocation.Item1, "latitude").IsNotLessThan(-90D);
            Validate.That(generalPostInfo.PickupLocation.Item2, "longitude").IsNotGreaterThan(180D);
            Validate.That(generalPostInfo.PickupLocation.Item2, "longitude").IsNotLessThan(-180D);

            Validate.That(generalPostInfo.OfferValueTitle, nameof(generalPostInfo.OfferValueTitle)).IsNot(null);
            Validate.That(generalPostInfo.OfferValueTitle.Length, nameof(generalPostInfo.OfferValueTitle.Length))
                .IsGreaterThan(0);

            Validate.That(generalPostInfo.Description, nameof(generalPostInfo.Description)).IsNot(null);
            Validate.That(generalPostInfo.Description.Length, nameof(generalPostInfo.Description.Length))
                .IsGreaterThan(0);

            Validate.That(generalPostInfo.StoreOrProductUri, nameof(generalPostInfo.StoreOrProductUri)).IsNot(null);
            Validate.That(
                generalPostInfo.StoreOrProductUri.IsAbsoluteUri,
                nameof(generalPostInfo.StoreOrProductUri.IsAbsoluteUri))
                .Is(true);
            Validate.That(generalPostInfo.StoreOrProductUri.Scheme, nameof(generalPostInfo.StoreOrProductUri.Scheme))
                .IsAny(Uri.UriSchemeHttp, Uri.UriSchemeHttps);

            Validate.That(generalPostInfo.ShippingCost, nameof(generalPostInfo.ShippingCost)).IsNotLessThan((decimal)0);

            Validate.That(generalPostInfo.Currency, nameof(generalPostInfo.Currency)).IsNot(null);
            Validate.That(generalPostInfo.Currency.Length, nameof(generalPostInfo.Currency.Length)).IsGreaterThan(0);

            Validate.That(generalPostInfo.Tags, nameof(generalPostInfo.Tags)).IsNot(null);

            return new PostEntity(id, generalPostInfo);
        }

        internal GeneralPostInfo ToGeneralPostInfo()
        {
            return new GeneralPostInfo()
                .SetOwner(Owner)
                .SetCreatedAt(CreatedAt)
                .SetCreatedAtTimeZone(CreatedAtTimeZone)
                .SetPickupLocation(PickupLocation)
                .SetOfferValueTitle(OfferValueTitle)
                .SetDescription(Description)
                .SetStoreOrProductUri(StoreOrProductUri)
                .SetShippingCost(ShippingCost)
                .SetCurrency(Currency)
                .SetTags(Tags)
                .SetImagePath(ImagePath);
        }
    }
}
