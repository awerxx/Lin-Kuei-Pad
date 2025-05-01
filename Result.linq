<Query Kind="Program" />

void Main()
{

}

public class Result<T, TError>
{
	private T? _value;
	private TError? _error;

	public bool IsSuccess { get; }

	public T Value
	{
		get => IsSuccess
			? _value!
			: throw new InvalidOperationException("Result is not successful");
		private set => _value = value;
	}

	public TError Error
	{
		get => !IsSuccess
			? _error!
			: throw new InvalidOperationException("Result is successful");
		private set => _error = value;
	}

	private Result(bool isSuccess, T? value, TError? error) => (IsSuccess, _value, _error) = (isSuccess, value, error);

	public static Result<T, TError> Success(T value) => new(true, value, default);
	public static Result<T, TError> Failure(TError error) => new(false, default, error);
}

public static class ResultExtensions
{
	public static Result<T2, TError> Map<T1, T2, TError>(this Result<T1, TError> result, Func<T1, T2> map)
		=> result.IsSuccess
			? Result<T2, TError>.Success(map(result.Value))
			: Result<T2, TError>.Failure(result.Error);

	public static Result<T2, TError> Bind<T1, T2, TError>(
		this Result<T1, TError> result,
		Func<T1, Result<T2, TError>> bind)
		=> result.IsSuccess
			? bind(result.Value)
			: Result<T2, TError>.Failure(result.Error);

	public static Result<T, TNewError> MapError<T, TError, TNewError>(
		this Result<T, TError> result,
		Func<TError, TNewError> map)
		=> result.IsSuccess
			? Result<T, TNewError>.Success(result.Value)
			: Result<T, TNewError>.Failure(map(result.Error));

	public static TResult Match<T, TError, TResult>(
		this Result<T, TError> result,
		Func<T, TResult> mapValue,
		Func<TError, TResult> mapError)
		=> result.IsSuccess
			? mapValue(result.Value)
			: mapError(result.Error);
}