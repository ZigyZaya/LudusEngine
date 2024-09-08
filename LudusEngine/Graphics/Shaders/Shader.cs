using System.Drawing;
using System.Reflection;
using OpenTK.Graphics.OpenGL;

namespace LudusEngine.Graphics.Shaders;

public class Shader : IDisposable
{
	public int Handle;
	
	private bool _disposed;

	public Shader(string vertexShaderResource, string fragmentShaderResource)
	{
		string vertexSource = ReadEmbeddedResource(vertexShaderResource);
		string fragmentSource = ReadEmbeddedResource(fragmentShaderResource);
		
		int vertexShader = GL.CreateShader(ShaderType.VertexShader);
		GL.ShaderSource(vertexShader, vertexSource);
		
		int fragmentShader = GL.CreateShader(ShaderType.FragmentShader);
		GL.ShaderSource(fragmentShader, fragmentSource);
		
		// compile the shaders
		GL.CompileShader(vertexShader);
		{
			GL.GetShader(vertexShader, ShaderParameter.CompileStatus, out int success);
			if (success == 0)
			{
				string infoLog = GL.GetShaderInfoLog(vertexShader);
				Console.WriteLine(infoLog);
			}
		}
		
		GL.CompileShader(fragmentShader);
		{
			GL.GetShader(fragmentShader, ShaderParameter.CompileStatus, out int success);
			if (success == 0)
			{
				string infoLog = GL.GetShaderInfoLog(fragmentShader);
				Console.WriteLine(infoLog);
			}
		}
		
		// program
		Handle = GL.CreateProgram();
		
		GL.AttachShader(Handle, vertexShader);
		GL.AttachShader(Handle, fragmentShader);
		
		GL.LinkProgram(Handle);
		{
			GL.GetProgram(Handle, GetProgramParameterName.LinkStatus, out int success);
			if (success == 0)
			{
				string infoLog = GL.GetProgramInfoLog(Handle);
				Console.WriteLine(infoLog);
			}
		}
		
		GL.DetachShader(Handle, vertexShader);
		GL.DetachShader(Handle, fragmentShader);
		GL.DeleteShader(vertexShader);
		GL.DeleteShader(fragmentShader);
	}

	public void Use(float[] color)
	{
		GL.UseProgram(Handle);
		int colorLocation = GL.GetUniformLocation(Handle, "uColor");
		GL.Uniform3(colorLocation, color[0], color[1], color[2]);
	}

	public virtual void Dispose(bool disposing)
	{
		if (!_disposed)
		{
			GL.DeleteProgram(Handle);
			_disposed = true;
		}
	}

	~Shader()
	{
		if (!_disposed)
		{
			Console.WriteLine("Maybe u forgot to call Dispose()?");
		}
	}

	public void Dispose()
	{
		Dispose(true);
		GC.SuppressFinalize(this);
	}
	
	private string ReadEmbeddedResource(string resourceName)
	{
		var assembly = Assembly.GetExecutingAssembly();
		using (Stream stream = assembly.GetManifestResourceStream(resourceName))
		{
			if (stream == null)
			{
				throw new FileNotFoundException($"Resource {resourceName} not found.");
			}

			using (StreamReader reader = new StreamReader(stream))
			{
				return reader.ReadToEnd();
			}
		}
	}
}