namespace LudusEngine.Graphics;
using OpenTK.Graphics.OpenGL;

public class Rectangle : Shape
{
	private float _width;
	private float _height;
	private uint[] _indices =
	{
		0, 1, 3,
		1, 2, 3,
	};
	
	public Rectangle(float x, float y, float width, float height, float[] color)
	{
		X = x;
		Y = y;
		_width = width;
		_height = height;
		ShapeColor = color;
		Indices = _indices;
	}
	
	
	
}