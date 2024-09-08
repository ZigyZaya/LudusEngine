using System.Reflection;
using System.Runtime.CompilerServices;
using LudusEngine;
using NLua;

namespace SandBox;

public class Main : Game
{
	public Main() : base(useLua: false)
	{
		SetGameAssembly(Assembly.GetExecutingAssembly());
	}
	
	protected override void Init()
	{
		Console.WriteLine("Init do C#");
	}

	protected override void Update(float delta)
	{
		//Console.WriteLine("Update do C#");
	}

	protected override void Draw()
	{
		//Console.WriteLine("Draw do C#");
	}
}
