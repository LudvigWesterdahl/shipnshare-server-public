using System;
using System.Collections.Generic;

namespace ShipWithMeWeb.Responses
{
    public sealed class UserReviewsResponse
    {
        public double AverageRating { get; set; }

        public int NumberOfReviews { get; set; }

        public int NumberOfGoodReviews { get; set; }

        public int NumberOfGoodButReviews { get; set; }

        public int NumberOfBadButReviews { get; set; }

        public int NumberOfBadReviews { get; set; }

        public IList<UserReviewResponse> UserReviews { get; set; }
    }
}
