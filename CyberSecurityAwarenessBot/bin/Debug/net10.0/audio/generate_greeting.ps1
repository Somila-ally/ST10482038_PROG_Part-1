# This script generates a simple WAV greeting using built-in .NET capabilities when run from PowerShell.
# It will create 'greeting.wav' in the same folder as the script.
# The script is intentionally simple — it requires PowerShell 7+ (pwsh) or Windows PowerShell with .NET available.

param()

try {
    $out = Join-Path -Path (Split-Path -Parent $MyInvocation.MyCommand.Definition) -ChildPath 'greeting.wav'

    Write-Output "Generating greeting WAV at: $out"

    # Text to speak
    $texts = @(
        'Welcome to the Cybersecurity Awareness ChatBot',
        'Hello. Welcome to the Cybersecurity Awareness ChatBot.'
    )

    # Use System.Speech if available (Windows PowerShell), otherwise fall back to SAPI via COM.
    $speechSucceeded = $false

    try {
        Add-Type -AssemblyName System.Speech -ErrorAction Stop
        $synth = New-Object System.Speech.Synthesis.SpeechSynthesizer
        $synth.SetOutputToWaveFile($out)
        foreach ($t in $texts) { $synth.Speak($t) }
        $synth.Dispose()
        $speechSucceeded = $true
    }
    catch {
        Write-Output "System.Speech not available: $_"
    }

    if (-not $speechSucceeded) {
        # Try COM SAPI (works on most Windows systems)
        try {
            $sapi = New-Object -ComObject SAPI.SpVoice
            $stream = New-Object -ComObject SAPI.SpFileStream
            $format = [Microsoft.DirectX.AudioVideoPlayback.WaveFormat]? $null

            # Open stream for write
            $stream.Open($out, 3, $false)
            $sapi.AudioOutputStream = $stream
            foreach ($t in $texts) { $sapi.Speak($t) }
            $stream.Close()
            $speechSucceeded = $true
        }
        catch {
            Write-Output "COM SAPI speak failed: $_"
        }
    }

    if ($speechSucceeded) { Write-Output "greeting.wav created." } else { Write-Output "Failed to generate greeting.wav. Please provide one manually." }
}
catch {
    Write-Output "Generator failed: $_"
}
