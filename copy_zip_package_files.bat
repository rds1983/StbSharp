rem delete existing
rmdir "ZipPackage" /Q /S

rem Create required folders
mkdir "ZipPackage"
mkdir "ZipPackage\netstandard1.1"

set "CONFIGURATION=Release"

rem Copy output files
copy "StbSharp\bin\%CONFIGURATION%\StbSharp.dll" ZipPackage /Y
copy "StbSharp\bin\%CONFIGURATION%\netstandard1.1\publish\StbSharp.dll" ZipPackage\netstandard1.1 /Y
