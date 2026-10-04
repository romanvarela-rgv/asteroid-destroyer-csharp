# Genera los assets de UI siguiendo el estilo de los botones originales
# (marco gris pixel art en grilla de 16px, texto blanco grueso, titulos naranja con brillo blanco).
#
# Uso (desde la raiz del repo):   powershell -ExecutionPolicy Bypass -File tools\generate-ui-assets.ps1
#
# Para agregar un boton nuevo, sumalo a $buttonGroups de abajo y volve a correrlo.
# El dibujo de botones, titulos y fondo esta en tools\UiGen.cs.

$ErrorActionPreference = 'Stop'

$root     = Split-Path -Parent $PSScriptRoot

# El generador (UiGen.cs) usa la misma fuente bitmap que el juego (src/PixelFont.cs)
Add-Type -Path (Join-Path $PSScriptRoot "UiGen.cs"), (Join-Path $root "src\PixelFont.cs") -ReferencedAssemblies System.Drawing

$textures = Join-Path $root 'assets\EngineGDI\textures'
New-Item -ItemType Directory -Force (Join-Path $textures 'UI') | Out-Null

# Cada grupo son los botones que aparecen juntos en una pantalla:
# comparten tamaño de letra (el que le entra al texto mas largo del grupo)
$buttonGroups = @(
    # Menu de pausa
    [ordered]@{
        'ResumeButton.png'  = 'Resume'
        'RestartButton.png' = 'Restart'
        'MenuButton.png'    = 'Menu'
    },
    # Menu de opciones
    [ordered]@{
        'SoundOnButton.png'  = 'Sound: On'
        'SoundOffButton.png' = 'Sound: Off'
    }
)

foreach ($buttons in $buttonGroups) {
    $scale = ($buttons.Values | ForEach-Object { [UiGen]::FitScale($_) } | Measure-Object -Minimum).Minimum
    foreach ($file in $buttons.Keys) {
        [UiGen]::Button($buttons[$file], $scale, (Join-Path $textures "Buttons\$file"))
        Write-Host "Buttons\$file"
    }
}

$titles = [ordered]@{
    'PausedTitle.png'    = 'PAUSED'
    'OptionsTitle.png'   = 'OPTIONS'
    'Wave1Title.png'     = 'WAVE 1'
    'Wave2Title.png'     = 'WAVE 2'
    'Wave3Title.png'     = 'WAVE 3'
    'NewRecordTitle.png' = 'NEW RECORD!'
}
foreach ($file in $titles.Keys) {
    [UiGen]::Title($titles[$file], 10, (Join-Path $textures "UI\$file"))
    Write-Host "UI\$file"
}

[UiGen]::Selector(5, (Join-Path $textures 'UI\Selector.png'));                            Write-Host 'UI\Selector.png'

# Frames de la explosion (los de 16px de Animations) agrandados x8 sin suavizar
New-Item -ItemType Directory -Force (Join-Path $textures 'Explosion') | Out-Null
foreach ($i in 1..8) {
    [UiGen]::Upscale((Join-Path $textures "Animations\$i.png"), 8, (Join-Path $textures "Explosion\$i.png"))
    Write-Host "Explosion\$i.png"
}

[UiGen]::Background(1024, 544, 4, 7, (Join-Path $textures 'Scenes\GameBackground.png'));  Write-Host 'Scenes\GameBackground.png'
