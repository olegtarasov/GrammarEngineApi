param([String]$buildName, [String]$commit, [String]$result, [String]$directory, [String]$branch, [String]$user, [String]$buildUrl)

$loc = Get-Location

Set-Location -Path $directory
$commitMsg = git log --format=%B -n 1 $commit | Out-String | foreach {$_.Trim()}

Set-Location -Path $loc

If ($result -eq "Succeeded")
{
    $state = "succeeded!"
    $emoji = ":tada:"
}
Else
{
    $state = "FAILED!"
    $emoji = ":scream:"
}

$params = "{`"text`": `"$emoji Build $buildName for commit '$commitMsg' on branch $branch by $user $state Build URL: $buildUrl`", `"username`": `"Build $buildName $state`"}"
Invoke-WebRequest -Uri https://mattermost.westeurope.cloudapp.azure.com/hooks/jtuxomee1tbw884krdpekm89ay -Method POST -Body $params -ContentType "text/plain; charset=utf-8"