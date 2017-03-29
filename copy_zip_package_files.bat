rem delete existing
rmdir "ZipPackage" /Q /S

rem Create required folders
mkdir "ZipPackage"

set "CONFIGURATION=Release"

rem Copy output files
copy "StbSharp\bin\%CONFIGURATION%\StbSharp.dll" ZipPackage /Y
copy "StbSharp\bin\%CONFIGURATION%\StbSharp.pdb" ZipPackage /Y
copy "StbSharp\bin\%CONFIGURATION%\Sichem.Framework.dll" ZipPackage /Y
copy "StbSharp\bin\%CONFIGURATION%\Sichem.Framework.pdb" ZipPackage /Y
