$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent $MyInvocation.MyCommand.Path
$out = Join-Path (Split-Path -Parent (Split-Path -Parent $root)) 'outputs\TrueStretch.exe'
$csc = 'C:\Windows\Microsoft.NET\Framework64\v4.0.30319\csc.exe'
if (-not (Test-Path -LiteralPath $csc)) { throw '64-bit .NET Framework derleyicisi bulunamadi.' }
& $csc /nologo /target:winexe /platform:x64 /optimize+ /win32manifest:"$root\app.manifest" /win32icon:"$root\assets\TrueStretch.ico" /reference:System.dll /reference:System.Drawing.dll /reference:System.Windows.Forms.dll /out:$out "$root\TrueStretch.cs"
if ($LASTEXITCODE -ne 0) { throw "Derleme basarisiz: $LASTEXITCODE" }
Get-Item -LiteralPath $out | Select-Object FullName, Length, LastWriteTime
