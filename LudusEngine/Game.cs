
using System.Drawing;
using OpenTK.Graphics.OpenGL;
using OpenTK.Windowing.Desktop;
using System.Reflection;
using LudusEngine.Graphics;
using NLua;
using NLua.Exceptions;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using Rectangle = LudusEngine.Graphics.Rectangle;


namespace LudusEngine
{
	public abstract class Game
	{
		// Run
        
		private float _deltaTime;
        
		// lua
		private bool _useLua;
		private Lua _lua;
		private LuaFunction? _luaUpdate;
		private LuaFunction? _luaDraw;
		private LuaFunction? _luaInit;
		
		// App
		private Assembly _gameAssembly;
        
		// Window
		private GameWindow _window;
		
		// Instances
		private Shape[] _shapeInstances = [new Rectangle(10, 10, 10, 10, new []{0.9f, 0.3f, 0.9f})];
		

		protected Game(bool useLua = false)
		{
			_useLua = useLua;

		}

		~Game()
		{
		}

		public void Run(string title, Vector2i screen, int fps)
		{
			// configure lua scripts and run the Init() functions
			if (_useLua)
			{
				ConfigureLua();
				
				try
				{
					_luaInit.Call(_lua);
				}
				catch (Exception e)
				{
					// todo: implement own exceptions to the engine
					throw new LuaException("An error occurred when loading Init() from Lua: " + e);
				}
			}
			else
			{
				Init();
			}
			
			
			// configure window
			var nativeWindowSettings = new NativeWindowSettings()
			{
				ClientSize = screen,
				Title = title,
				APIVersion = new Version(3, 3)
			};

			_window = new GameWindow(new GameWindowSettings
			{ 
				UpdateFrequency = fps,
				
			}, nativeWindowSettings);

			_window.Load += OnLoad;
			_window.RenderFrame += OnRenderFrame;
			_window.UpdateFrame += OnUpdateFrame;
			_window.FramebufferResize += OnFramebufferResize;

			_window.Run();
		}
        
        // window
        private void OnLoad()
        {
	        GL.ClearColor(Color.DarkSlateGray);
	        foreach (Shape shape in _shapeInstances)
	        {
		        shape.InitializeBuffers();
	        }
        }
		
		private void OnUpdateFrame(FrameEventArgs args)
		{
			_deltaTime = (float)args.Time;  // delta from openTK
			
			if (_useLua)
				_luaUpdate?.Call(_deltaTime);
			else
				Update(_deltaTime);

		}

		private void OnRenderFrame(FrameEventArgs args)
		{
			GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

			// if it is using lua, call draw func
			if (_useLua)
				_luaDraw?.Call();
			else
				Draw();

			foreach (Shape shape in _shapeInstances)
			{
				shape.Draw();
			}
			
			_window.SwapBuffers();
			GL.Flush();
		}

		private void OnFramebufferResize(FramebufferResizeEventArgs args)
		{
			GL.Viewport(0, 0, args.Width, args.Height);
		}

        // base functions
		protected abstract void Init();
		protected abstract void Update(float delta);
		protected abstract void Draw();

		private string GetLuaScript()
		{
			if (_gameAssembly == null)
				throw new InvalidOperationException("Game assembly not set.");

			// find the main.lua file inside the Lua folder
			var resourceName = _gameAssembly.GetManifestResourceNames()
				.FirstOrDefault(name => name.EndsWith("main.lua"));

			if (resourceName == null)
				throw new FileNotFoundException("Lua sript not found.");

			using (var stream = _gameAssembly.GetManifestResourceStream(resourceName))
			{
				if (stream == null)
					throw new FileNotFoundException($"Resource not found: {resourceName}");

				using (var reader = new StreamReader(stream))
				{
					return reader.ReadToEnd();
				}
			}
		}

		private void ConfigureLua()
		{
			_lua = new Lua();
			_lua.LoadCLRPackage();

			var mainha = new Teste();
			_lua["teste"] = mainha;
			
			// load main.lua
			var script = GetLuaScript();
			_lua.DoString(script);

			// get main.lua functions
			_luaUpdate = _lua.GetFunction("Update");
			_luaDraw = _lua.GetFunction("Draw");
			_luaInit = _lua.GetFunction("Init");
			

		}

		protected void SetGameAssembly(Assembly assembly)
		{
			_gameAssembly = assembly;
		}
	}

	public class Teste
	{
		public void Piranha()
		{
			Console.WriteLine("Piranha");
		}
	}
}