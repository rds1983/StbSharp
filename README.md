### About
StbSharp is C# port of famous C framework: [https://github.com/nothings/stb](https://github.com/nothings/stb)

### Usage
StbSharp.dll is PCL and therefore could be used on the broad range of platforms.
The easiest way of adding it to the project is through NuGet:
`Install-Package StbSharp`

Alternative way is to download the latest release:
https://github.com/rds1983/StbSharp/releases

Then add StbSharp.dll reference manually.

StbSharp has same API as STB. Therefore the STB documentation is valid for StbSharp as well.
However some helper functions had been added.
Therefore image loading code looks like following:
```c# 
var buffer = File.ReadAllBytes(path);
int x, y, comp;
var data = Image.stbi_load_from_memory(buffer, out x, out y, out comp, Image.STBI_rgb_alpha);
```

This code will try to load image (jpg/png/pic/bmp/tga) located at path. It'll throw Exception on failure.
If the loading is succesful then it'll return byte array containing image in the 32-bit rgba representation.

If you are writing MonoGame application and would like to convert that data to the Texture2D. It could be done following way:
```c#
var colors = new Color[x*y];
for (var i = 0; i < colors.Length; ++i)
{
	colors[i].R = data[i*4];
	colors[i].G = data[i*4 + 1];
	colors[i].B = data[i*4 + 2];
	colors[i].A = data[i*4 + 3];
}

texLoadedBySTB = new Texture2D(GraphicsDevice, x, y, false, SurfaceFormat.Color);
texLoadedBySTB.SetData(colors);
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

### License
[https://opensource.org/licenses/MIT] (https://opensource.org/licenses/MIT)