<Query Kind="Program">
  <Namespace>Xunit</Namespace>
  <Namespace>System.Collections.ObjectModel</Namespace>
  <AutoDumpHeading>true</AutoDumpHeading>
</Query>

#load "xunit"

public static class EnumerableExtensions
{
	public static IEnumerable<T> WhereNotNull<T>(this IEnumerable<T>? source)
		=> source?.Where(value => value is not null) ?? [];

	public static IEnumerable<string> WhereNotEmpty(this IEnumerable<string>? source)
		=> source?.Where(value => !string.IsNullOrWhiteSpace(value)) ?? [];

	public static IReadOnlyCollection<T> ReadOnly<T>(this IEnumerable<T>? source)
		=> source?.ToList().AsReadOnly() ?? new ReadOnlyCollection<T>([]);

	public static bool IsEmpty<T>(this T[]? source) => source is null || source.Length is 0;

	public static bool IsEmpty<T>(this List<T>? source) => source is null || source.Count is 0;

	public static bool IsEmpty<T>(this IReadOnlyCollection<T>? source) => source is null || source.Count is 0;

	public static string JoinWithComma(this IEnumerable<string>? elements)
		=> elements is null
			? string.Empty
			: string.Join(",", values: elements.Select(e => e.Trim()));

	public static IEnumerable<TSource> ExceptMatchingBy<TSource, TExcluded, TKey>(
		this IEnumerable<TSource> source,
		IEnumerable<TExcluded> excluded,
		Func<TSource, TKey> sourceKeySelector,
		Func<TExcluded, TKey> excludedKeySelector,
		IEqualityComparer<TKey> keyEqualityComparer)
	{
		var excludedKeys = new HashSet<TKey>(excluded.Select(excludedKeySelector), keyEqualityComparer);

		foreach (var s in source)
		{
			if (!excludedKeys.Contains(sourceKeySelector(s)))
				yield return s;
		}
	}

	public static IEnumerable<TSource> ExceptMatchingBy<TSource, TExcluded, TKey>(
		this IEnumerable<TSource> source,
		IEnumerable<TExcluded> excluded,
		Func<TSource, TKey> sourceKeySelector,
		Func<TExcluded, TKey> excludedKeySelector)
		=> ExceptMatchingBy(source, excluded, sourceKeySelector, excludedKeySelector, EqualityComparer<TKey>.Default);

	public static IEnumerable<TSource> IntersectMatchingBy<TSource, TExcluded, TKey>(
		this IEnumerable<TSource> source,
		IEnumerable<TExcluded> second,
		Func<TSource, TKey> sourceKeySelector,
		Func<TExcluded, TKey> secondKeySelector,
		IEqualityComparer<TKey> keyEqualityComparer)
	{
		var secondKeys = new HashSet<TKey>(second.Select(secondKeySelector), keyEqualityComparer);

		foreach (var s in source)
		{
			if (secondKeys.Contains(sourceKeySelector(s)))
				yield return s;
		}
	}

	public static IEnumerable<TSource> IntersectMatchingBy<TSource, TExcluded, TKey>(
		this IEnumerable<TSource> source,
		IEnumerable<TExcluded> second,
		Func<TSource, TKey> sourceKeySelector,
		Func<TExcluded, TKey> secondKeySelector)
		=> IntersectMatchingBy(source, second, sourceKeySelector, secondKeySelector, EqualityComparer<TKey>.Default);
}

#region private::Tests

[Fact] void Test_Xunit() => Assert.True(1 + 1 == 2);

#endregion