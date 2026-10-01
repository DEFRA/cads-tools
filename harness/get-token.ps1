<#
.SYNOPSIS
Gets tokens from the local OIDC mock using the authorization code flow.

.PARAMETER App
Which UI client to sign in as: 'mis' (default) or 'admin'.

.PARAMETER Scopes
Optional. Overrides the default scopes for the chosen app (space separated).

.EXAMPLE
./get-token.ps1
./get-token.ps1 -App admin
./get-token.ps1 -App admin -Scopes "openid profile email db.admin.execute"
#>
param(
    [ValidateSet("mis", "admin")]
    [string]$App = "mis",

    [string]$Scopes
)

# Client details must match oidc/config/clients.yml
$Apps = @{
    mis = @{
        ClientId     = "local-cads-mis"
        ClientSecret = "local-mock-secret"
        Scopes       = "openid profile email offline_access reports.read"
        TestUser     = "mip-viewer-user"
    }
    admin = @{
        ClientId     = "local-cads-admin-frontend"
        ClientSecret = "local-mock-secret"
        Scopes       = "openid profile email offline_access db.admin.execute admin.s3.manager admin.queue.manager"
        TestUser     = "cads-admin-user"
    }
}

$Selected     = $Apps[$App]
$ClientId     = $Selected.ClientId
$ClientSecret = $Selected.ClientSecret
if (-not $Scopes) { $Scopes = $Selected.Scopes }

$AuthUrl     = "http://localhost:5557/connect/authorize"
$TokenUrl    = "http://localhost:5557/connect/token"
$RedirectUri = "http://localhost:7777/callback"

Write-Host "App: $App (client: $ClientId)"
Write-Host "Sign in as: $($Selected.TestUser) / password"

# Encode scopes
$EncodedScopes = [System.Net.WebUtility]::UrlEncode($Scopes)

# Build URL safely
$Url = $AuthUrl + "?client_id=" + $ClientId + `
       "&response_type=code" + `
       "&redirect_uri=" + $RedirectUri + `
       "&scope=" + $EncodedScopes

Write-Host "Opening browser for login..."

try {
    $Uri = [System.Uri]$Url
    Start-Process $Uri.AbsoluteUri
}
catch {
    Write-Host "Failed to open browser automatically. Please open this URL manually:"
    Write-Host $Url
}

Write-Host "Starting temporary callback server on port 7777..."

# Step 1 — Start a tiny callback server to capture the ?code=
$Listener = New-Object System.Net.HttpListener
$Listener.Prefixes.Add("http://*:7777/")
$Listener.Start()

$Context = $Listener.GetContext()
$Request = $Context.Request
$Response = $Context.Response

$Code = $Request.QueryString["code"]
$AuthError = $Request.QueryString["error"]

# Respond to browser
$ResponseString = "<html><body>You may close this window.</body></html>"
$Buffer = [System.Text.Encoding]::UTF8.GetBytes($ResponseString)
$Response.ContentLength64 = $Buffer.Length
$Response.OutputStream.Write($Buffer, 0, $Buffer.Length)
$Response.OutputStream.Close()
$Listener.Stop()

if (-not $Code) {
    Write-Error "No authorization code received. Error from server: $AuthError"
    exit 1
}

Write-Host "Received authorization code: $Code"
Write-Host "Exchanging code for tokens..."

# Step 2 — Exchange the code for tokens
$Body = @{
    grant_type    = "authorization_code"
    client_id     = $ClientId
    client_secret = $ClientSecret
    code          = $Code
    redirect_uri  = $RedirectUri
}

$Tokens = Invoke-RestMethod -Method Post -Uri $TokenUrl -Body $Body

Write-Host "Tokens received:"
$Tokens | ConvertTo-Json -Depth 5
