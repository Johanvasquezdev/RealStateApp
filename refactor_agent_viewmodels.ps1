$namespaces = @{
    "RealEstateApp.Core.Application.ViewModels.Propiedad" = "RealEstateApp.Core.Application.ViewModels.AgentProperties"
    "RealEstateApp.Core.Application.ViewModels.Oferta" = "RealEstateApp.Core.Application.ViewModels.AgentOffers"
    "RealEstateApp.Core.Application.ViewModels.Chat" = "RealEstateApp.Core.Application.ViewModels.AgentChats"
    "RealEstateApp.Core.Application.ViewModels.Perfil" = "RealEstateApp.Core.Application.ViewModels.AgentProfile"
    "RealEstateApp.Core.Application.ViewModels.Cuenta" = "RealEstateApp.Core.Application.ViewModels.Accounts"
    "RealEstateApp.Core.Application.ViewModels.Agente" = "RealEstateApp.Core.Application.ViewModels.Agents"
}

$classes = @{
    "PropiedadViewModel" = "AgentPropertyViewModel"
    "PropiedadSaveViewModel" = "AgentPropertySaveViewModel"
    "OfertaDetalleViewModel" = "AgentOfferDetailViewModel"
    "OfertaGestionViewModel" = "AgentOfferManagementViewModel"
    "OfertaResumenViewModel" = "AgentOfferSummaryViewModel"
    "ConversacionDetalleViewModel" = "AgentChatDetailViewModel"
    "ConversacionResumenViewModel" = "AgentChatSummaryViewModel"
    "EnviarMensajeViewModel" = "AgentSendMessageViewModel"
    "PerfilViewModel" = "AgentProfileViewModel"
    "PerfilSaveViewModel" = "AgentProfileSaveViewModel"
}

$utf8 = New-Object System.Text.UTF8Encoding $true

Get-ChildItem -Path . -Recurse -Include *.cs,*.cshtml | ForEach-Object {
    $content = [System.IO.File]::ReadAllText($_.FullName, $utf8)
    $original = $content
    
    foreach ($key in $namespaces.Keys) {
        $content = $content.Replace($key, $namespaces[$key])
    }
    
    foreach ($key in $classes.Keys) {
        $content = [System.Text.RegularExpressions.Regex]::Replace($content, "\b$key\b", $classes[$key])
    }
    
    if ($original -cne $content) {
        [System.IO.File]::WriteAllText($_.FullName, $content, $utf8)
        Write-Host "Updated $($_.Name)"
    }
}
