param([String]$definition, [String]$result, [String]$deployUrl)

If ($result -eq "Succeeded")
{
    $state = "succeeded!"
    $emoji = ":rocket:"
}
Else
{
    $state = "FAILED!"
    $emoji = ":construction:"
}

$params = "{`"text`": `"$emoji Deployment '$definition' $state. Deployment URL: $deployUrl`", `"username`": `"Deployment $state`"}"
Invoke-WebRequest -Uri https://mattermost.westeurope.cloudapp.azure.com/hooks/jtuxomee1tbw884krdpekm89ay -Method POST -Body $params -ContentType "text/plain; charset=utf-8"