### About
StbSharp is C# image decoding/encoding library. 
It is port of famous C library: [https://github.com/nothings/stb](https://github.com/nothings/stb)  
Right now, it can load images in JPG, PNG, BMP, TGA, PSD and GIF formats. And write in BMP, TGA, PNG and HDR formats.

### Adding Reference to StbSharp
StbSharp.dll is PCL and therefore could be used on the broad range of platforms.
The easiest way of adding it to the project is through NuGet:
`Install-Package StbSharp`

Alternative way is to download the latest release:
https://github.com/rds1983/StbSharp/releases

Then add StbSharp.dll reference manually.

### Loading Image
StbSharp has same API as STB. Therefore the STB documentation is valid for StbSharp as well.
However some wrapper classes and functions had been added.
I.e. 'ImageReader' class wraps the call to 'stbi_load_from_callbacks' method.
It could be used following way:
```c#
ImageReader loader = new ImageReader();
using (Stream stream = File.Open(path, FileMode.Open)) 
{
	Image image = loader.Read(ms, Stb.STBI_rgb_alpha);
}
```

Or 'LoadFromMemory' method wraps 'stbi_load_from_memory':
```c# 
byte[] buffer = File.ReadAllBytes(path);
int x, y, comp;
Image image = Stb.LoadFromMemory(buffer, Stb.STBI_rgb_alpha);
```

Both code samples will try to load an image (JPG/PNG/BMP/TGA/PSD/GIF) located at 'path'. It'll throw Exception on failure.

If you are writing MonoGame application and would like to convert that data to the Texture2D. It could be done following way:
```c#
Texture2D texture = new Texture2D(GraphicsDevice, image.Width, image.Height, false, SurfaceFormat.Color);
texture.SetData(image.Data);
```

Or if you are writing WinForms app and would like StbSharp resulting bytes to be converted to the Bitmap. The sample code is:
```c#
byte[] data = image.Data;
// Convert rgba to bgra
for (int i = 0; i < x*y; ++i)
{
	byte r = data[i*4];
	byte g = data[i*4 + 1];
	byte b = data[i*4 + 2];
	byte a = data[i*4 + 3];


	data[i*4] = b;
	data[i*4 + 1] = g;
	data[i*4 + 2] = r;
	data[i*4 + 3] = a;
}

// Create Bitmap
Bitmap bmp = new Bitmap(_loadedImage.Width, _loadedImage.Height, PixelFormat.Format32bppArgb);
BitmapData bmpData = bmp.LockBits(new Rectangle(0, 0, _loadedImage.Width, _loadedImage.Height), ImageLockMode.WriteOnly,
	bmp.PixelFormat);

Marshal.Copy(data, 0, bmpData.Scan0, bmpData.Stride*bmp.Height);
bmp.UnlockBits(bmpData);
```

### Saving Image
StbSharp can write images in BMP, TGA, PNG and HDR formats.
Sample code for writing an image represented as RGBA byte array `data`:
```c#
using (Stream stream = File.OpenWrite(path))
{
	ImageWriter writer = new ImageWriter();
	Image image = new Image
	{
		Data = data,
		Width = width,
		Height = height,
		Comp = 4
	};
	writer.Write(image, Stb.ImageWriterType.Png, stream);
}
```

### License
Public Domain