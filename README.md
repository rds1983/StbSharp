### About
StbSharp is C# port of the famous C framework: [https://github.com/nothings/stb](https://github.com/nothings/stb)
It is important to note, that this project is **port**, not **wrapper**. Original C code had been ported to C#. Therefore StbSharp doesnt require any native binaries.
The porting hasn't been done by hand, but using [Sichem](https://github.com/rds1983/Sichem), which is the C to C# code converter utility.
Following libraries had been ported so far: stb_image.h, stb_image_write.h, stb_image_resize.h, stb_dxt.h and stb_vorbis.c.
Following libraries porting is planned: stb_textedit.h, stb_truetype.h and stb_rectpack.

### Documentation
StbSharp has same API as STB. Therefore the STB documentation is valid for StbSharp as well.
However some wrapper classes and helper functions had been added.

The [wiki](https://github.com/rds1983/StbSharp/wiki) contains instructions about referencing StbSharp in a project and the code samples.

### License
Public Domain