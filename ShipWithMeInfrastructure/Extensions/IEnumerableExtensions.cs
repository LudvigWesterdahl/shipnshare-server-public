using System;
using System.Collections.Generic;
using System.Linq;

namespace ShipWithMeInfrastructure.Extensions
{
    internal static class IEnumerableExtensions
    {
        /// <summary>
        /// Filters the elements of an enumerable according to all the provided predicates.
        /// </summary>
        /// <typeparam name="T">the type of the enumerable</typeparam>
        /// <param name="enumerable">the enumerable</param>
        /// <param name="predicates">the predicates</param>
        /// <returns>the provided enumerable with all predicates applied</returns>
        internal static IEnumerable<T> WhereAll<T>(this IEnumerable<T> enumerable, IEnumerable<Predicate<T>> predicates)
        {
            if (predicates == null) {
                return enumerable;
            }

            foreach (var predicate in predicates) {
                enumerable = enumerable.Where(o => predicate.Invoke(o));
            }

            return enumerable;
        }
    }
}
