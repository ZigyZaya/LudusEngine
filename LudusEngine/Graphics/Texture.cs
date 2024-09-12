using OpenTK.Graphics.OpenGL;
using StbImageSharp;

namespace LudusEngine.Graphics;

public class Texture
{
	private int Handle;

	public Texture(string filepath)
	{
		Handle = GL.GenTexture();	
		StbImage.stbi_set_flip_vertically_on_load(1);

		ImageResult texture = ImageResult.FromStream(File.OpenRead(filepath), ColorComponents.RedGreenBlueAlpha);
		
		GL.TexImage2D(TextureTarget.Texture2D, 
			0, 
			PixelInternalFormat.Rgba, 
			texture.Width, 
			texture.Height, 
			0, 
			PixelFormat.Rgba, 
			PixelType.UnsignedByte, 
			texture.Data
			);
		
		
	}
}