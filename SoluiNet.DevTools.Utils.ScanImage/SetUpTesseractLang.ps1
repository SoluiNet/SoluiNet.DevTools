Add-Type -AssemblyName System.IO.Compression.FileSystem
function Unzip
{
    param([string]$zipfile, [string]$outpath)

    [System.IO.Compression.ZipFile]::ExtractToDirectory($zipfile, $outpath)
}

#$url = "https://github.com/tesseract-ocr/tessdata/archive/3.04.00.zip"
#$output = "$PSScriptRoot\tesseract-lang.zip"

$url = "https://github.com/tesseract-ocr/tessdata/raw/3.04.00/eng.traineddata"
$output = "$PSScriptRoot\tessdata\eng.traineddata"

$start_time = Get-Date

$wc = New-Object System.Net.WebClient
$wc.DownloadFile($url, $output)

Write-Output "Time taken: $((Get-Date).Subtract($start_time).Seconds) second(s)"

#Unzip "$PSScriptRoot\tesseract-lang.zip" "$PSScriptRoot\tesseract-lang-temp"

#$shell = New-Object -ComObject Shell.Application
#$path = Join-Path $output "tessdata-3.04.00"
#$outputPath = "$PSScriptRoot\tesseract-lang-temp"
#
#$shell.NameSpace($path).CopyHere($outputPath)

#[Reflection.Assembly]::LoadWithPartialName('System.IO.Compression.FileSystem') | Out-Null

#$folder = 'tessdata-3.04.00'
#$outputPath = "$PSScriptRoot\tesseract-lang-temp"

#[IO.Compression.ZipFile]::OpenRead($output).Entries | ? {
#  $_.FullName -like "$($folder -replace '\\','/')/*"
#} | % {
#  $file   = Join-Path $outputPath $_.FullName
#  $parent = Split-Path -Parent $file
#  if (-not (Test-Path -LiteralPath $parent)) {
#    New-Item -Path $parent -Type Directory | Out-Null
#  }
#  [IO.Compression.ZipFileExtensions]::ExtractToFile($_, $file, $true)
#}