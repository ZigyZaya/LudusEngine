using System.Drawing;
using LudusEngine.Graphics.Shaders;
using OpenTK.Graphics.OpenGL;

namespace LudusEngine.Graphics;

public class Shape
{
	protected float X;
	protected float Y;
	
	protected uint[] Indices;
	private readonly float[] _vertices = {
		0.5f,  0.5f, 0.0f,  // top right
		0.5f, -0.5f, 0.0f,  // bottom right
		-0.5f, -0.5f, 0.0f,  // bottom left
		-0.5f,  0.5f, 0.0f   // top left
	};

	
	protected Shader ShapeShader;
	protected float[] ShapeColor;
	
	private int _vertexBufferObject;
	private int _vertexArrayObject;
	private int _elementBufferObject;

	public void InitializeBuffers()
	{
		_vertexBufferObject = GL.GenBuffer();
		_vertexArrayObject = GL.GenVertexArray();
		_elementBufferObject = GL.GenBuffer();
		
		GL.BindVertexArray(_vertexArrayObject);
		
		GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject);
		GL.BufferData(BufferTarget.ArrayBuffer, _vertices.Length * sizeof(float), _vertices, BufferUsageHint.StaticDraw);
		
		GL.BindBuffer(BufferTarget.ElementArrayBuffer, _elementBufferObject);
		GL.BufferData(BufferTarget.ElementArrayBuffer, Indices.Length * sizeof(uint), Indices, BufferUsageHint.StaticDraw);
		
		GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
		GL.EnableVertexAttribArray(0);
		
		// shader
		ShapeShader = new Shader("LudusEngine.Graphics.Shaders.shader.vert", "LudusEngine.Graphics.Shaders.shader.frag");
		
	}

	public void Draw()
	{
		ShapeShader.Use(ShapeColor);
		GL.BindVertexArray(_vertexArrayObject);
		GL.DrawElements(PrimitiveType.Triangles, Indices.Length, DrawElementsType.UnsignedInt, 0);
	}
}