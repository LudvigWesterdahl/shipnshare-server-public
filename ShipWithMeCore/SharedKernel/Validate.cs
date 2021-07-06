using System;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;

namespace ShipWithMeCore.SharedKernel
{
    /// <summary>
    /// Helper class to validate arguments.
    /// <para>
    ///     TODO: Simplify when CallerArgumentExpression is added.
    /// </para>
    /// </summary>
    public static class Validate
    {
        /// <summary>
        /// Returns a <see cref="Requirement{T}"/> type that can validate the provided value based
        /// on the available criterias.
        /// </summary>
        /// <typeparam name="T">the type of the value</typeparam>
        /// <param name="val">the value</param>
        /// <param name="name">the name of the value such as nameof(x)</param>
        /// <param name="memberName">the member name</param>
        /// <param name="lineNumber">the line number</param>
        /// <param name="filePath">the file path</param>
        /// <returns>new <see cref="Requirement{T}"/> instance for validation</returns>
        public static Requirement<T> That<T>(
            T val,
            string name,
            [CallerMemberName] string memberName = "",
            [CallerLineNumber] int lineNumber = 0,
            [CallerFilePath] string filePath = "")
        {
            return new Requirement<T>(val, name, memberName, lineNumber, filePath);
        }

        /// <summary>
        /// Requirements to validate variables where all methods throws exceptions if
        /// the chosen requirement is not satisfied.
        /// </summary>
        /// <typeparam name="T">the type to validate</typeparam>
        public sealed class Requirement<T>
        {
            private readonly T value;

            private readonly string name;

            private readonly string memberName;

            private readonly int lineNumber;

            private readonly string filePath;

            private string ValueName => $"{name}({NullableString(value)})";

            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="value">the value to check</param>
            /// <param name="name">the name of the value</param>
            /// <param name="memberName">the member name</param>
            /// <param name="lineNumber">the line number</param>
            /// <param name="filePath">the file path</param>
            internal Requirement(T value, string name, string memberName, int lineNumber, string filePath)
            {
                this.value = value;
                this.name = name;
                this.memberName = memberName;
                this.lineNumber = lineNumber;
                this.filePath = filePath;
            }

            /// <summary>
            /// Returns the string representation of value or "null" if it is null.
            /// </summary>
            /// <param name="value">the value</param>
            /// <returns>string representation or "null"</returns>
            private static string NullableString(object value)
            {
                if (value == null) {
                    return "null";
                }

                return value.ToString();
            }

            /// <summary>
            /// Throws a new <see cref="ArgumentException"/> based on the member name, line number, file path
            /// and the provided argument expression (message).
            /// </summary>
            /// <param name="argumentExpression">the argument expression</param>
            private void ThrowArgumentException(string argumentExpression)
            {
                throw new ArgumentException(
                    $"{memberName}({Path.GetFileName(filePath)}:{lineNumber}) - {argumentExpression}");
            }

            /// <summary>
            /// Calls <see cref="ThrowArgumentException(string)"/> by creating the argument expression
            /// from this value, op and other.
            /// </summary>
            /// <param name="op">the operator between the value and other</param>
            /// <param name="other">the value that was compared with</param>
            private void Throw(string op, object other)
            {
                ThrowArgumentException(ValueName + $" {op} {NullableString(other)}");
            }

            /// <summary>
            /// Requires that this value equals other.
            /// <para>
            ///     Note that null is considered to be equal to null.
            /// </para>
            /// </summary>
            /// <param name="other">the value to compare with</param>
            /// <returns>this instance for chaining</returns>
            public Requirement<T> Is(T other)
            {
                if (value == null && other == null) {
                    return this;
                }

                if (value == null || other == null || !value.Equals(other)) {
                    Throw("==", other);
                }

                return this;
            }

            /// <summary>
            /// Requires that this value equals any of the provided values.
            /// <para>
            ///     Note that null is considered to be equal to null.
            /// </para>
            /// </summary>
            /// <param name="other">the value to compare with</param>
            /// <returns>this instance for chaining</returns>
            public Requirement<T> IsAny(params T[] others)
            {
                if (value == null && others == null) {
                    return this;
                }

                if (others.Length == 0 || !others.Contains(value)) {
                    Throw("in", others);
                }

                return this;
            }

            /// <summary>
            /// Requires that this value does not equal other.
            /// <para>
            ///     Note that null is considered to be equal to null.
            /// </para>
            /// </summary>
            /// <param name="other">the value to compare with</param>
            /// <returns>this instance for chaining</returns>
            public Requirement<T> IsNot(T other)
            {
                if (value == null && other == null) {
                    Throw("!=", other);
                }

                if (value == null || other == null) {
                    return this;
                }

                if (value.Equals(other)) {
                    Throw("!=", other);
                }

                return this;
            }

            /// <summary>
            /// Requires that this value is less than other.
            /// <para>
            ///     Note that null is not less than or greater than any value.
            /// </para>
            /// </summary>
            /// <param name="other">the value to compare with</param>
            /// <returns>this instance for chaining</returns>
            public Requirement<T> IsLessThan(IComparable<T> other)
            {
                if (value == null || other == null || other.CompareTo(value) <= 0) {
                    Throw("<", other);
                }

                return this;
            }

            /// <summary>
            /// Requires that this value is not less than other (greater than or equals to).
            /// <para>
            ///     Note that null is not less than or greater than any value.
            /// </para>
            /// </summary>
            /// <param name="other">the value to compare with</param>
            /// <returns>this instance for chaining</returns>
            public Requirement<T> IsNotLessThan(IComparable<T> other)
            {
                if (value == null || other == null || !(other.CompareTo(value) <= 0)) {
                    Throw(">=", other);
                }

                return this;
            }

            /// <summary>
            /// Requires that this value is greater than other.
            /// <para>
            ///     Note that null is not less than or greater than any value.
            /// </para>
            /// </summary>
            /// <param name="other">the value to compare with</param>
            /// <returns>this instance for chaining</returns>
            public Requirement<T> IsGreaterThan(IComparable<T> other)
            {
                if (value == null || other == null || other.CompareTo(value) >= 0) {
                    Throw(">", other);
                }

                return this;
            }

            /// <summary>
            /// Requires that this value is not greater than other (less than or equals to).
            /// <para>
            ///     Note that null is not less than or greater than any value.
            /// </para>
            /// </summary>
            /// <param name="other">the value to compare with</param>
            /// <returns>this instance for chaining</returns>
            public Requirement<T> IsNotGreaterThan(IComparable<T> other)
            {
                if (value == null || other == null || !(other.CompareTo(value) >= 0)) {
                    Throw("<=", other);
                }

                return this;
            }
        }
    }
}
