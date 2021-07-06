using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace ShipWithMeInfrastructure.Repositories
{
    public static class RepositoryUtils
    {
        public static string NewGuidString()
        {
            return Guid.NewGuid().ToString("N");
        }

        public static async Task<string> NewGuidString<T>(Func<string, ValueTask<T>> findAsync) where T : class
        {

            var id = NewGuidString();

            while (await findAsync(id) != null)
            {
                id = NewGuidString();
            }

            return id;
        }

        public static async Task<IEnumerable<string>> NewGuidStrings<T>(int num, Func<string, ValueTask<T>> findAsync)
            where T : class
        {
            var ids = new List<string>();

            while(ids.Count < num)
            {
                var id = await NewGuidString(findAsync);
                if (!ids.Contains(id))
                {
                    ids.Add(id);
                }
            }

            return ids;
        }

    }
}
