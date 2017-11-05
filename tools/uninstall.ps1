param($installPath, $toolsPath, $package, $project)

# $value: - Copy always, 2 - Copy if newer
Function RemoveFile([string] $fileName)
{
      $file = $project.ProjectItems.Item($fileName)
      $file.Remove()
}

RemoveFile "libdesr.dll"
RemoveFile "solarix_grammar_engine.dll"
RemoveFile "sqlite.dll"
