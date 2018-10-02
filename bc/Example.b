namespace Example;

public class Program
{
	func Main(string args)
	{
		var int i = 0;
		let i = 1;
		var bool b = Test(i);
		var Test t = new Test();
		new Test(1);
		let t.i = 233.3;
		var Test t_Copy = t.Copy();
		inv CheatPrint(b);
		var int s = cast<int>'s';
		var int[,,,,,,,] arr = new int[0,0,0,0,0,0,0,0];
		for(var int f = 0; i < 100; let i = i + 1;)
		{
			if(i == 50) break;
			continue;
		}
		var Test wt = new Test(1);
		while(wt.i<100)
		{
			let wt = wt.copy(wt.i + 1);
		}
	}

	func Test(int i) : bool
	{
		if(i == 0) return true;
	}

	func CheatPrint(bool b)
	{
		inv Console.WriteLine(b);
		return;
	}
}

class Test : ITest
{
	var int i;
	var ITest ttt;

	func Test(int i)
	{
		let i = i;
	}

	func Test(int i, ITest t)
	{
		let i = i;
		let ttt = t;
	}

	func Copy() : Test
	{
		return new Test((2 * 4 - 8));
	}

	func copy(int i) : Test
	{
		return new Test(i + (2 * 4 - 8));
	}

	func copy(ITest it) : Test
	{
		return new Test(i + (2 * 4 - 8));
	}
}

interface ITest
{
	func GetTest();
}

struct int{}
struct char{}
struct float{}
struct bool{}
struct void{}
class string{}

/*
444
*/
//dfdfdfd