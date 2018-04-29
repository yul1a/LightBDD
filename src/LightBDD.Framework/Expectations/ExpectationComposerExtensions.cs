﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using LightBDD.Core.Formatting.Values;
using LightBDD.Framework.Expectations.Implementation;

namespace LightBDD.Framework.Expectations
{
    /// <summary>
    /// Extensions offering methods for defining expectations.
    /// </summary>
    public static class ExpectationComposerExtensions
    {
        /// <summary>
        /// Helper method creating simple expectation based on <paramref name="predicateFn"/> and <paramref name="descriptionFn"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="composer"></param>
        /// <param name="descriptionFn">Expectation description</param>
        /// <param name="predicateFn">Expectation predicate</param>
        public static Expectation<T> ComposeSimple<T>(this IExpectationComposer composer, Func<IValueFormattingService, string> descriptionFn, Func<T, bool> predicateFn)
        {
            return composer.Compose(new SimpleExpectation<T>(descriptionFn, predicateFn));
        }

        /// <summary>
        /// Creates expectation for values to be equal to <paramref name="expected"/> value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="composer"></param>
        /// <param name="expected">Expected value</param>
        public static Expectation<T> Equal<T>(this IExpectationComposer composer, T expected)
        {
            return composer.ComposeSimple<T>(
                formatter => $"equal '{formatter.FormatValue(expected)}'",
                x => Equals(x, expected));
        }

        /// <summary>
        /// Creates expectation for values to be equal one of values in <paramref name="expectedCollection"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="composer"></param>
        /// <param name="expectedCollection">Collection of expected values</param>
        public static Expectation<T> In<T>(this IExpectationComposer composer, params T[] expectedCollection)
        {
            return composer.ComposeSimple<T>(
                formatter => $"in '{formatter.FormatValue(expectedCollection)}'",
                expectedCollection.Contains);
        }

        /// <summary>
        /// Creates expectation for collections to contain value specified by <paramref name="value"/> parameter.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="composer"></param>
        /// <param name="value">Expected value</param>
        public static Expectation<IEnumerable<T>> Contains<T>(this IExpectationComposer composer, T value)
        {
            return composer.ComposeSimple<IEnumerable<T>>(
                formatter => $"contains '{formatter.FormatValue(value)}'",
                x => x != null && x.Contains(value));
        }

        /// <summary>
        /// Creates expectation for collections with at least one value fulfilling expectation specified by <paramref name="expectationBuilder"/> parameter.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="composer"></param>
        /// <param name="expectationBuilder">Expectation builder</param>
        public static Expectation<IEnumerable<T>> Any<T>(this IExpectationComposer composer, Func<IExpectationComposer, IExpectation<T>> expectationBuilder)
        {
            return composer.Compose(new AnyExpectation<T>(expectationBuilder.Invoke(Expect.To)));
        }

        /// <summary>
        /// Creates expectation for collections with all values fulfilling expectation specified by <paramref name="expectationBuilder"/> parameter.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="composer"></param>
        /// <param name="expectationBuilder">Expectation builder</param>
        public static Expectation<IEnumerable<T>> All<T>(this IExpectationComposer composer, Func<IExpectationComposer, IExpectation<T>> expectationBuilder)
        {
            return composer.Compose(new AllExpectation<T>(expectationBuilder.Invoke(Expect.To)));
        }

        /// <summary>
        /// Creates expectation for strings to match pattern specified by <paramref name="pattern"/> parameter.
        ///
        /// The <paramref name="pattern"/> may have special characters:
        /// <list type="bullet">
        /// <item><description>* - 0-more characters</description></item>
        /// <item><description>? - 1 character</description></item>
        /// <item><description># - 1 digit</description></item>
        /// </list>
        /// </summary>
        /// <param name="composer"></param>
        /// <param name="pattern">Expected pattern</param>
        public static Expectation<string> MatchWild(this IExpectationComposer composer, string pattern)
        {
            return MatchWild(composer, pattern, RegexOptions.None, $"matching '{pattern}'");
        }

        /// <summary>
        /// Creates expectation for strings to match pattern specified by <paramref name="pattern"/> parameter, where character case is ignored.
        ///
        /// The <paramref name="pattern"/> may have special characters:
        /// <list type="bullet">
        /// <item><description>* - 0-more characters</description></item>
        /// <item><description>? - 1 character</description></item>
        /// <item><description># - 1 digit</description></item>
        /// </list>
        /// </summary>
        /// <param name="composer"></param>
        /// <param name="pattern">Expected pattern</param>
        public static Expectation<string> MatchWildIgnoreCase(this IExpectationComposer composer, string pattern)
        {
            return MatchWild(composer, pattern, RegexOptions.IgnoreCase, $"matching any case '{pattern}'");
        }

        private static Expectation<string> MatchWild(IExpectationComposer composer, string pattern, RegexOptions options, string format)
        {
            var regexPattern = "^" + Regex.Escape(pattern).Replace("\\?", ".").Replace("\\*", ".*").Replace("\\#", "\\d") + "$";
            var regex = new Regex(regexPattern, options);
            return composer.ComposeSimple<string>(formatter => format, value => value != null && regex.IsMatch(value));
        }

        /// <summary>
        /// Creates expectation for strings to match regex pattern specified by <paramref name="pattern"/> parameter.
        /// </summary>
        /// <param name="composer"></param>
        /// <param name="pattern">Expected pattern</param>
        public static Expectation<string> MatchRegex(this IExpectationComposer composer, string pattern)
        {
            return MatchRegex(composer, pattern, RegexOptions.None, $"matching regex '{pattern}'");
        }

        /// <summary>
        /// Creates expectation for strings to match regex pattern specified by <paramref name="pattern"/> parameter, where character case is ignored.
        /// </summary>
        /// <param name="composer"></param>
        /// <param name="pattern">Expected pattern</param>
        public static Expectation<string> MatchRegexIgnoreCase(this IExpectationComposer composer, string pattern)
        {
            return MatchRegex(composer, pattern, RegexOptions.IgnoreCase, $"matching regex any case '{pattern}'");
        }

        private static Expectation<string> MatchRegex(IExpectationComposer composer, string pattern, RegexOptions options, string format)
        {
            var regex = new Regex(pattern, options);
            return composer.ComposeSimple(formatter => format, (string value) => value != null && regex.IsMatch(value));
        }

        /// <summary>
        /// Creates expectation for values to be null.
        /// </summary>
        /// <param name="builder"></param>
        public static Expectation<object> Null(this IExpectationComposer builder)
        {
            return builder.ComposeSimple<object>(formatter => "null", x => x == null);
        }

        /// <summary>
        /// Creates expectation for comparable types to be between values specified by <paramref name="a"/> and <paramref name="b"/> parameters, where parameter values are not included.
        /// The <paramref name="a"/> parameter value may be greater or lower than value of <paramref name="b"/> - both scenarios are supported.
        /// None of the provided parameters can be null.
        /// </summary>
        /// <param name="composer"></param>
        /// <param name="a">Parameter A.</param>
        /// <param name="b">Parameter B.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="a"/> or <paramref name="b"/> is null.</exception>
        public static Expectation<T> Between<T>(this IExpectationComposer composer, T a, T b) where T : IComparable<T>
        {
            if (a == null) throw new ArgumentNullException(nameof(a));
            if (b == null) throw new ArgumentNullException(nameof(b));
            return composer.ComposeSimple<T>(formatter => $"between '{formatter.FormatValue(a)}' and '{formatter.FormatValue(b)}'", x => x != null && Math.Abs(x.CompareTo(a) + x.CompareTo(b)) < 2);
        }

        /// <summary>
        /// Creates expectation for comparable types to be greater than value specified by <paramref name="value"/>, where the value cannot be null.
        /// </summary>
        /// <param name="composer"></param>
        /// <param name="value">Value</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="value"/> parameter value is null.</exception>
        public static Expectation<T> GreaterThan<T>(this IExpectationComposer composer, T value) where T : IComparable<T>
        {
            if (value == null) throw new ArgumentNullException(nameof(value));
            return composer.ComposeSimple<T>(formatter => $"greater than '{formatter.FormatValue(value)}'", x => x != null && x.CompareTo(value) > 0);
        }

        /// <summary>
        /// Creates expectation for comparable types to be less than value specified by <paramref name="value"/>, where the value cannot be null.
        /// </summary>
        /// <param name="composer"></param>
        /// <param name="value">Value</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="value"/> parameter value is null.</exception>
        public static Expectation<T> LessThan<T>(this IExpectationComposer composer, T value) where T : IComparable<T>
        {
            if (value == null) throw new ArgumentNullException(nameof(value));
            return composer.ComposeSimple<T>(formatter => $"less than '{formatter.FormatValue(value)}'", x => x != null && x.CompareTo(value) < 0);
        }

        /// <summary>
        /// Creates expectation for comparable types to be greater or equal value specified by <paramref name="value"/>, where the value cannot be null.
        /// </summary>
        /// <param name="composer"></param>
        /// <param name="value">Value</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="value"/> parameter value is null.</exception>
        public static Expectation<T> GreaterOrEqual<T>(this IExpectationComposer composer, T value) where T : IComparable<T>
        {
            if (value == null) throw new ArgumentNullException(nameof(value));
            return composer.ComposeSimple<T>(formatter => $"greater or equal '{formatter.FormatValue(value)}'", x => x != null && x.CompareTo(value) >= 0);
        }

        /// <summary>
        /// Creates expectation for comparable types to be less or equal value specified by <paramref name="value"/>, where the value cannot be null.
        /// </summary>
        /// <param name="composer"></param>
        /// <param name="value">Value</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="value"/> parameter value is null.</exception>
        public static Expectation<T> LessOrEqual<T>(this IExpectationComposer composer, T value) where T : IComparable<T>
        {
            if (value == null) throw new ArgumentNullException(nameof(value));
            return composer.ComposeSimple<T>(formatter => $"less or equal '{formatter.FormatValue(value)}'", x => x != null && x.CompareTo(value) <= 0);
        }

        /// <summary>
        /// Creates expectation for values to fulfill all expectations specified by <paramref name="expectationBuilder"/> parameter.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="composer"></param>
        /// <param name="expectationBuilder">Expectation builder</param>
        public static Expectation<T> AllTrue<T>(this IExpectationComposer composer, params Func<IExpectationComposer, IExpectation<T>>[] expectationBuilder)
        {
            return composer.Compose(new AndExpectation<T>(expectationBuilder.Select(x => x.Invoke(Expect.To)).ToArray()));
        }

        /// <summary>
        /// Creates expectation for values to fulfill any expectation specified by <paramref name="expectationBuilder"/> parameter.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="composer"></param>
        /// <param name="expectationBuilder">Expectation builder</param>
        public static Expectation<T> AnyTrue<T>(this IExpectationComposer composer, params Func<IExpectationComposer, IExpectation<T>>[] expectationBuilder)
        {
            return composer.Compose(new OrExpectation<T>(expectationBuilder.Select(x => x.Invoke(Expect.To)).ToArray()));
        }

        /// <summary>
        /// Combines the existing expectation with one specified by <paramref name="andExpectation"/> parameter where both have to be fulfilled by values.
        /// </summary>
        public static Expectation<T> And<T>(this Expectation<T> expectation, Func<IExpectationComposer, Expectation<T>> andExpectation)
        {
            return new AndExpectation<T>(expectation, andExpectation(Expect.To));
        }

        /// <summary>
        /// Combines the existing expectation with one specified by <paramref name="orExpectation"/> parameter where at least one has to be fulfilled by values.
        /// </summary>
        public static Expectation<T> Or<T>(this Expectation<T> expectation, Func<IExpectationComposer, Expectation<T>> orExpectation)
        {
            return new OrExpectation<T>(expectation, orExpectation(Expect.To));
        }
    }
}