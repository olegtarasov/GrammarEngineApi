param($installPath, $toolsPath, $package, $project)

# $value: - Copy always, 2 - Copy if newer
Function AddFile([string] $fileName)
{
      Write-Host "Adding $fileName"
      $file = $project.ProjectItems.AddFromFile($fileName)
      $copyToOutput = $file.Properties.Item("CopyToOutputDirectory")
      $copyToOutput.Value = 2
}

AddFile "$($installPath)\lib\libdesr.dll"
AddFile "$($installPath)\lib\solarix_grammar_engine.dll"
AddFile "$($installPath)\lib\sqlite.dll"

$project.Save()