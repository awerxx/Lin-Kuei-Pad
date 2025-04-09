<Query Kind="Program">
  <Namespace>Xunit</Namespace>
</Query>

#load "xunit"

readonly struct Maybe<TValue> : IEquatable<Maybe<TValue>>
{
	private readonly object _value;

	public Maybe(TValue value)
		=> _value = value ?? throw new ArgumentNullException(nameof(value));

	private TValue Value
		=> (TValue)_value;

	private bool IsSome
		=> !IsNone;

	private bool IsNone
		=> _value is null;

	public TResult Map<TResult>(Func<TValue, TResult> ifSome, Func<TResult> ifNone)
		=> IsSome ? ifSome(Value) : ifNone();

	public Maybe<TResult> Map<TResult>(Func<TValue, TResult> ifSome)
		=> Map(value => ifSome(value), Maybe.None<TResult>);

	public Maybe<TResult> Map<TResult>(Func<TValue, Maybe<TResult>> ifSome)
		=> Map(ifSome, Maybe.None<TResult>);

	public void Do(Action<TValue> ifSome)
	{
		if (IsSome)
			ifSome(Value);
	}

	public void Do(Action<TValue> ifSome, Action ifNone)
	{
		if (IsSome)
			ifSome(Value);
		else
			ifNone();
	}

	public bool Equals(Maybe<TValue> other)
		=> Map(v => other.Map(ov => ov.Equals(v), () => false), () => other.IsNone);

	public override bool Equals(object obj)
		=> obj is Maybe<TValue> maybe && Equals(maybe);

	public override int GetHashCode()
		=> Map(v => v.GetHashCode(), () => 0);

	public static implicit operator Maybe<TValue>(TValue value)
		=> value is null ? Maybe.None<TValue>() : Maybe.Some(value);

	public static bool operator ==(Maybe<TValue> left, Maybe<TValue> right)
		=> left.Equals(right);

	public static bool operator !=(Maybe<TValue> left, Maybe<TValue> right)
		=> !left.Equals(right);
}

static class Maybe
{
	public static Maybe<TValue> Some<TValue>(TValue value)
		=> new Maybe<TValue>(value);

	public static Maybe<TValue> None<TValue>()
		=> default;

	public static Maybe<TValue> A<TValue>(TValue value)
		=> value;
}

void Main()
{
	[Fact]
	void Throws_when_constructed_with_null()
		=> Assert.Throws<ArgumentNullException>(() => new Maybe<object>(null));


	[Fact]
	void Is_some_when_there_is_a_value()
	{
		const int value = 123;

		var maybe1 = Maybe.A(value);
		Maybe<int> maybe2 = value;

		var expected = Maybe.Some(value);

		Assert.Equal(expected, maybe1);
		Assert.Equal(expected, maybe2);
	}

	[Fact]
	void Is_none_when_there_is_no_value()
	{
		var maybe1 = Maybe.A<string>(null);
		Maybe<string> maybe2 = null;

		var expected = Maybe.None<string>();

		Assert.Equal(expected, maybe1);
		Assert.Equal(expected, maybe2);
	}

	[Fact]
	void Maps_using_value_mapping_function_if_there_is_some_value()
	{
		var maybe = Maybe.Some(5);

		var mappedValue = maybe.Map(
			value => ++value,
			() => throw new Exception("This should not have been called!"));

		Assert.Equal(6, mappedValue);
	}

	[Fact]
	void Maps_using_none_mapping_function_if_there_is_no_value()
	{
		var maybe = Maybe.None<int>();

		const int mappedValueIfNone = 123;
		var mappedValue = maybe.Map(
			_ => throw new Exception("This should not have been called!"),
			() => mappedValueIfNone);

		Assert.Equal(mappedValueIfNone, mappedValue);
	}

	[Fact]
	void Maps_to_none_if_there_is_no_value_and_no_none_mapping_function_was_provided()
	{
		var maybe = Maybe.None<int>();

		var mapped = maybe.Map(v => v.ToString());

		Assert.Equal(Maybe.None<string>(), mapped);
	}

	[Fact]
	void Invokes_some_action_if_there_is_some_value()
	{
		var value = new object();
		var maybe = Maybe.Some(value);

		object invokedActionParameter = null;
		maybe.Do(v => invokedActionParameter = v);

		Assert.Equal(value, invokedActionParameter);
	}


	[Fact]
	void Invokes_none_action_if_there_was_no_value()
	{
		var maybe = Maybe.None<object>();

		var wasNoneActionInvoked = false;
		maybe.Do(v => throw new Exception("This should not have been called"), () => wasNoneActionInvoked = true);


		Assert.True(wasNoneActionInvoked);
	}

	[Fact]
	void Does_not_equal_non_maybe_types()
	{
		var maybe = Maybe.Some(123);
		const int nonMaybe = 123;

		Assert.False(maybe.Equals((object)nonMaybe));
	}

	[Fact]
	void Some_and_none_are_not_equal()
	{
		var some = Maybe.Some(123);
		var none = Maybe.None<int>();

		ShouldNotBeEqual(some, none);
		ShouldNotBeEqual(none, some);
	}

	[Fact]
	void Two_nones_are_always_equal()
	{
		var none1 = Maybe.None<string>();
		var none2 = Maybe.None<string>();

		ShouldBeEqual(none1, none2);
		ShouldBeEqual(none2, none1);
	}

	[Fact]
	void Two_somes_are_equal_only_if_their_values_are_equal()
	{
		var some1 = Maybe.Some(123);
		var some2 = Maybe.Some(123);
		var some3 = Maybe.Some(999);

		ShouldBeEqual(some1, some2);
		ShouldBeEqual(some2, some1);
		ShouldNotBeEqual(some1, some3);
		ShouldNotBeEqual(some3, some1);
		ShouldNotBeEqual(some2, some3);
		ShouldNotBeEqual(some3, some2);
	}

	[Fact]
	void None_has_a_hash_code_of_0()
	{
		var none = Maybe.None<string>();

		Assert.Equal(0, none.GetHashCode());
	}

	[Fact]
	void Some_has_the_underlying_value_hash_code()
	{
		const string value = "Goosfraba!";
		var some = Maybe.Some(value);

		Assert.Equal(value.GetHashCode(), some.GetHashCode());
	}


	void ShouldBeEqual<TValue>(Maybe<TValue> left, Maybe<TValue> right)
	{
		Assert.True(left.Equals(right));
		Assert.True(left == right);
		Assert.False(left != right);
	}


	void ShouldNotBeEqual<TValue>(Maybe<TValue> left, Maybe<TValue> right)
	{
		Assert.False(left.Equals(right));
		Assert.False(left == right);
		Assert.True(left != right);
	}
}


