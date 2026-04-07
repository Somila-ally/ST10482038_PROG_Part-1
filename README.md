# ST10482038_PROG_Part-1
CyberSecurityAwarenessBot
# Cybersecurity Awareness Bot (Somila)

A small console chatbot that provides basic cybersecurity tips, speaks responses on Windows, and includes a nicer console UI (ASCII art, colors, typing effect).

## Features
- Console UI with ASCII logo, colored sections and typing effect
- Keyword-based cybersecurity tips (passwords, phishing, browsing, privacy, general)
- Basic conversational QA ("How are you?", "What is your purpose?")
- Sentiment detection for worried users (gives empathetic prefix)
- Memory of last topic and user interest to continue conversations
- Text-to-speech (TTS) via `System.Speech` or SAPI when available
- Project split into `Program`, `Chatbot`, `User`, and `AudioPlayer` files
- CI workflow (`.github/workflows/dotnet.yml`) to build on push
- Helper `make_commits.ps1` to create multiple local commits for submission

## Requirements
- Windows (TTS uses Windows components)
- .NET SDK (tested with .NET 8 / compatible with `net10.0` target in this repo)
- PowerShell (for the commit script)

## Running locally
1. Open the solution in Visual Studio or use the terminal.
2. From project root (`ChatBot\`) run:

   `dotnet build`
   `dotnet run --project ChatBot`

3. Or open the project in Visual Studio and press F5.

## Speech / Audio notes
- The `AudioPlayer` probes for `System.Speech` first, falls back to SAPI, and finally uses a background `Task` to speak.
- Menu and option text are intentionally not spoken. The bot still speaks greetings and tips.
- If TTS does not work, ensure `System.Speech` is available or run the program on Windows with SAPI installed.

## Creating commits for submission
- Use the included `make_commits.ps1` script to create multiple local commits (12 by default):

  `pwsh .\make_commits.ps1`

- Configure `git` user info if needed:

  `git config user.name "Your Name"`
  `git config user.email "you@example.com"`

- Create a GitHub repo, add it as `origin`, and push to `main` to trigger CI:

  `git remote add origin https://github.com/youruser/yourrepo.git`
  `git branch -M main`
  `git push -u origin main`

## Continuous Integration
- The workflow `.github/workflows/dotnet.yml` restores and builds the solution on Windows.
- Adjust the `dotnet-version` in the workflow if you require a different SDK.

## Suggested commit breakdown (for your video)
1. Initial project setup
2. Added TTS/audio wrapper
3. Split code into separate classes
4. Added ASCII art and UI polish
5. Implemented tips database and keyword mapping
6. Added sentiment detection and memory
7. Input validation and menu flow fixes
8. Added CI workflow and commit script
(Additional commits were added by `make_commits.ps1`.)

## Video presentation checklist
- Use your own voice (no AI-generated voice).
- Show the program running and demonstrate features:
  - Greeting and TTS
  - Asking for name
  - Choosing menu options and asking questions
  - Demonstrate "another tip" flow and sentiment handling
- Explain code structure: `Program.cs`, `Chatbot.cs`, `User.cs`, `AudioPlayer.cs`.
- Explain logic (keyword mapping, memory, TTS fallback).

## Customization
- To disable all speech: comment out calls to `audio.SpeakAsync(...)` in `Program.cs`.
- To change typing speed, adjust the `TypeEffect` calls in `Program.cs` or `Chatbot.cs`.

## License
This project is provided as-is for educational purposes.
