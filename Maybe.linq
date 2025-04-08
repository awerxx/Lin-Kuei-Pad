<Query Kind="Program">
  <Namespace>Xunit</Namespace>
</Query>

#load "xunit"

void Main()
{
	[Fact]
	void WHEN_constructing_with_null_THEN_throws_ArgumentNullException() => Assert.Throws<ArgumentNullException>(() => new Maybe<object>(null));
}

readonly struct Maybe<TValue>// : IEquatable<Maybe<TValue>>
{
	private readonly object _value;
	
	public Maybe(TValue value) => _value = value?? throw new ArgumentNullException(nameof(value));
	
	private TValue Value => (TValue)_value;
	private bool IsSome => !IsNone;
	private bool IsNone => _value is null;
	
	public TResult Map<TResult>(Func<TValue, TResult> ifSome, Func<TResult> ifNone) => IsSome ? ifSome(Value) : ifNone();
	public Maybe<TResult> Map<TResult>(Func<TValue, TResult> ifSome) => Map(value => ifSome(value), Maybe.None<TResult>);
	
	public Maybe<TResult> Map<TResult>(Func<TValue, Maybe<TResult>> ifSome) => Map(ifSome, Maybe.None<TResult>);
	

	public static implicit operator Maybe<TValue>(TValue value) => value is null ? Maybe.None<TValue>() : Maybe.Some(value);
	public static bool operator ==(Maybe<TValue> left, Maybe<TValue> right) => left.Equals(right);
	public static bool operator !=(Maybe<TValue> left, Maybe<TValue> right) => !left.Equals(right);

}

static class Maybe
{
	public static Maybe<TValue> Some<TValue>(TValue value) => new Maybe<TValue>(value);
	public static Maybe<TValue> None<TValue>() => default;
	public static Maybe<TValue> A<TValue>(TValue value) => value;
}
