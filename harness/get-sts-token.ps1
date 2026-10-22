<#
.SYNOPSIS
Gets a token from the local STS mock using the client credentials flow, and prints the decoded JWT.

.PARAMETER App
Required. Which service is calling: 'cads-bridge' or 'cads-data-service'.

.PARAMETER Audience
Required (unless -Scopes is given). Which service the token is for: 'cads-bridge' or 'cads-data-service'.
The scope is derived from it: sts.<audience>.

.PARAMETER Scopes
Optional. Overrides the scope that is derived from the audience.

.EXAMPLE
./get-sts-token.ps1 -App cads-bridge -Audience cads-data-service
./get-sts-token.ps1 -App cads-data-service -Audience cads-bridge
./get-sts-token.ps1 -App cads-bridge -Scopes "sts.cads-data-service"
#>
param(
    [ValidateSet("cads-bridge", "cads-data-service")]
    [string]$App,

    [ValidateSet("cads-bridge", "cads-data-service")]
    [string]$Audience,

    [string]$Scopes
)

# Client details must match sts-mock/config/clients.yml
$Apps = @{
    "cads-bridge" = @{
        ClientId     = "local-sts-cads-bridge"
        ClientSecret = "local-mock-secret"
    }
    "cads-data-service" = @{
        ClientId     = "local-sts-cads-data-service"
        ClientSecret = "local-mock-secret"
    }
}

if (-not $App -or (-not $Audience -and -not $Scopes)) {
    Write-Host "Usage: ./get-sts-token.ps1 -App <cads-bridge|cads-data-service> -Audience <cads-bridge|cads-data-service> [-Scopes <scope>]" -ForegroundColor Red
    Write-Host ""
    Read-Host "Press Enter to close"
    exit 1
}

$Selected     = $Apps[$App]
$ClientId     = $Selected.ClientId
$ClientSecret = $Selected.ClientSecret
if (-not $Scopes) { $Scopes = "sts.$Audience" }

$TokenUrl = "http://localhost:5567/connect/token"

function ConvertFrom-JwtPart([string]$Part) {
    $Padded = $Part.Replace('-', '+').Replace('_', '/')
    while (($Padded.Length -band 3) -ne 0) { $Padded += '=' }
    [System.Text.Encoding]::UTF8.GetString([Convert]::FromBase64String($Padded))
}

try {
    Write-Host "App: $App (client: $ClientId)"
    Write-Host "Requesting token for scope: $Scopes"

    $Body = @{
        grant_type    = "client_credentials"
        client_id     = $ClientId
        client_secret = $ClientSecret
        scope         = $Scopes
    }

    $Tokens = Invoke-RestMethod -Method Post -Uri $TokenUrl -Body $Body
    $Parts  = $Tokens.access_token.Split('.')

    Write-Host ""
    Write-Host "Header:"
    ConvertFrom-JwtPart $Parts[0] | ConvertFrom-Json | ConvertTo-Json -Depth 10

    Write-Host ""
    Write-Host "Payload:"
    ConvertFrom-JwtPart $Parts[1] | ConvertFrom-Json | ConvertTo-Json -Depth 10

    Write-Host ""
    Write-Host "Token:"
    $Tokens.access_token
}
catch {
    Write-Host ""
    Write-Host "Failed to get a token. Is the harness running (./harness/run-harness.sh up)?" -ForegroundColor Red
    Write-Host $_.Exception.Message -ForegroundColor Red
}

Write-Host ""
Read-Host "Press Enter to close"
