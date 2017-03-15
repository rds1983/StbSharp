### About
StbSharp is C# image loading library. It is port of famous C library: [https://github.com/nothings/stb/blob/master/stb_image.h](https://github.com/nothings/stb/blob/master/stb_image.h)
Right now, it can load images in JPG, PNG, BMP, TGA, PSD, PIC and GIF formats.

### Adding Reference to StbSharp
StbSharp.dll is PCL and therefore could be used on the broad range of platforms.
The easiest way of adding it to the project is through NuGet:
`Install-Package StbSharp`

Alternative way is to download the latest release:
https://github.com/rds1983/StbSharp/releases

Then add StbSharp.dll reference manually.

### Loading Image
StbSharp has same API as STB. Therefore the STB documentation is valid for StbSharp as well.
However some helper functions had been added.
Therefore image loading code looks like following:
```c# 
var buffer = File.ReadAllBytes(path);
int x, y, comp;
var data = Stb.stbi_load_from_memory(buffer, out x, out y, out comp, Image.STBI_rgb_alpha);
```

This code will try to load image (JPG/PNG/BMP/TGA/PSD/PIC/GIF) located at `path`. It'll throw Exception on failure.
If the loading is succesful then it'll return byte array containing image in the 32-bit RGBA representation.

If you are writing MonoGame application and would like to convert that data to the Texture2D. It could be done following way:
```c#
var texture = new Texture2D(GraphicsDevice, x, y, false, SurfaceFormat.Color);
texture.SetData(data);
```

Or if you are writing WinForms app and would like StbSharp resulting bytes to be converted to the Bitmap. The sample code is:
```c#
// Convert rgba to bgra
for (var i = 0; i < x*y; ++i)
{
	var r = data[i*4];
	var g = data[i*4 + 1];
	var b = data[i*4 + 2];
	var a = data[i*4 + 3];


	data[i*4] = b;
	data[i*4 + 1] = g;
	data[i*4 + 2] = r;
	data[i*4 + 3] = a;
}

// Create Bitmap
var pixelFormat = PixelFormat.Format32bppArgb;
var bmp = new Bitmap(x, y, pixelFormat);
var bmpData = bmp.LockBits(new Rectangle(0, 0, x, y), ImageLockMode.WriteOnly, bmp.PixelFormat);

Marshal.Copy(data, 0, bmpData.Scan0, bmpData.Stride*bmp.Height);
bmp.UnlockBits(bmpData);
```

### Saving Image
StbSharp can save images in rgba format.
Sample code:
```c#
using (var stream = File.OpenWrite(fileName))
{
	Stb.stbi_write_to(data, width, height, 4, Stb.ImageWriterType.Png, stream);
}
```

### License
[https://opensource.org/licenses/MIT](https://opensource.org/licenses/MIT)