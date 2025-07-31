<Query Kind="Statements" />

int n = 2;
int m = 5;

Func<int> lambda = () => Utils.Prepare(n, m);

Utils.Puzzle(0, lambda).Dump("Prints: 1");

n = 3;	
Utils.Puzzle(0, lambda).Dump("Prints: 3");

Utils.Puzzle(5, 6).Dump("Prints: 5");
Utils.Puzzle(5, Utils.Prepare(n, m));

class Whatever
{
	private int _n;
	private int _m;
	
	public Whatever(int n, int m) => (_n, _m) = (n, m);
	
	public int Invoke() => Utils.Prepare(1, 2);
}

static class Utils
{
	public static int Puzzle(int a, Whatever b)
	{
		if (a > 0) return a;
		return b.Invoke();
	}

	public static int Puzzle(int a, int b)
	{
		return Puzzle(a, () => b);
	}

	public static int Puzzle(int a, Func<int> b)
	{
		if (a > 0) return a;
		return b.Invoke();
	}

	public static int Prepare(int n, int m)
	{
		int sum = 0;
		for (int i = 0; i < n; i++)
		{
			sum += i % m;
		}
		
		return sum;
	}
}