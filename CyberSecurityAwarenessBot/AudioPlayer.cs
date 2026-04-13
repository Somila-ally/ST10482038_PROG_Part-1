using System;
using System.Diagnostics;
using System.IO;
using System.Media;
using System.Threading;

namespace CyberSecurityAwarenessBot
{
    internal static class AudioPlayer
    {
        public static void EnsureGreetingAudioExists()
        {
            try
            {
                var baseDir = AppDomain.CurrentDomain.BaseDirectory;
                var cwd = Environment.CurrentDirectory;

                // Where we prefer to put an audio greeting for playback
                var preferredAudioDir = Path.Combine(baseDir, "audio");
                Directory.CreateDirectory(preferredAudioDir);
                var target = Path.Combine(preferredAudioDir, "greeting.wav");
                // If a greeting already exists, leave it alone (user-provided or previously generated)
                if (File.Exists(target))
                {
                    return;
                }

                // First try to generate a WAV using local SAPI (COM) with a male voice when available
                try
                {
                    var sapiType = Type.GetTypeFromProgID("SAPI.SpVoice");
                    var streamType = Type.GetTypeFromProgID("SAPI.SpFileStream");
                    if (sapiType != null && streamType != null)
                    {
                        try
                        {
                            dynamic sapi = Activator.CreateInstance(sapiType);
                            dynamic stream = Activator.CreateInstance(streamType);
                            // 3 = SSFMCreateForWrite
                            stream.Open(target, 3, false);

                            // Try to select a male voice if available
                            try
                            {
                                dynamic voices = sapi.GetVoices();
                                for (int i = 0; i < voices.Count; i++)
                                {
                                    dynamic v = voices.Item(i);
                                    try
                                    {
                                        var gender = v.GetAttribute("Gender") as string;
                                        if (!string.IsNullOrEmpty(gender) && gender.ToLowerInvariant().Contains("male"))
                                        {
                                            sapi.Voice = v;
                                            break;
                                        }
                                    }
                                    catch
                                    {
                                        // ignore
                                    }
                                }
                            }
                            catch
                            {
                                // ignore
                            }

                            sapi.AudioOutputStream = stream;
                            sapi.Speak("Hello. Welcome to the Cybersecurity Awareness ChatBot");
                            stream.Close();
                            return;
                        }
                        catch
                        {
                            // ignore
                        }
                    }
                }
                catch
                {
                    // ignore and fall back to script
                }

                // Look for a generator script in several candidate source locations (project audio, baseDir audio, cwd audio)
                var candidateScripts = new[]
                {
                    Path.Combine(preferredAudioDir, "generate_greeting.ps1"),
                    Path.Combine(baseDir, "generate_greeting.ps1"),
                    Path.Combine(baseDir, "audio", "generate_greeting.ps1"),
                    Path.Combine(cwd, "audio", "generate_greeting.ps1"),
                    Path.Combine(cwd, "generate_greeting.ps1"),
                    Path.Combine(Directory.GetParent(baseDir)?.FullName ?? baseDir, "audio", "generate_greeting.ps1")
                };

                string scriptPath = null;
                foreach (var s in candidateScripts)
                {
                    if (string.IsNullOrWhiteSpace(s)) continue;
                    if (File.Exists(s))
                    {
                        scriptPath = s;
                        break;
                    }
                }

                if (scriptPath == null)
                {
                    return;
                }

                // Run the script where it lives so it produces the greeting.wav next to the script (PlayGreetingIfExists checks cwd/audio and parent locations)
                var scriptDir = Path.GetDirectoryName(scriptPath) ?? preferredAudioDir;

                var tried = false;
                foreach (var shell in new[] { "pwsh", "powershell" })
                {
                    try
                    {
                        var psi = new ProcessStartInfo(shell, $"-NoProfile -ExecutionPolicy Bypass -File \"{scriptPath}\"")
                        {
                            CreateNoWindow = true,
                            UseShellExecute = false,
                            RedirectStandardOutput = true,
                            RedirectStandardError = true,
                            WorkingDirectory = scriptDir
                        };

                        using var proc = Process.Start(psi);
                        if (proc == null) continue;
                        proc.WaitForExit(15000);
                        tried = true;
                        break;
                    }
                    catch
                    {
                        // try next shell
                    }
                }

                if (!tried)
                {
                    // Could not run script - leave silently
                }
            }
            catch
            {
                // ignore
            }
        }

        public static void PlayGreetingIfExists()
        {
            try
            {
                var filename = "greeting.wav";
                var baseDir = AppDomain.CurrentDomain.BaseDirectory;
                var cwd = Environment.CurrentDirectory;

                var candidates = new[]
                {
                    Path.Combine(baseDir, filename),
                    Path.Combine(baseDir, "audio", filename),
                    Path.Combine(cwd, filename),
                    Path.Combine(cwd, "audio", filename),
                    Path.Combine(Directory.GetParent(baseDir)?.FullName ?? baseDir, filename)
                };

                foreach (var p in candidates)
                {
                    if (string.IsNullOrWhiteSpace(p)) continue;
                    if (!File.Exists(p)) continue;

                        try
                        {
                            using var fs = new FileStream(p, FileMode.Open, FileAccess.Read, FileShare.Read);
                            using var player = new SoundPlayer(fs);
                            player.Load();
                            player.PlaySync();
                            return;
                        }
                    catch
                    {
                        // try next
                    }
                }
            }
            catch
            {
                // ignore
            }
        }

        public static void SpeakText(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return;
            try
            {
                var sapiType = Type.GetTypeFromProgID("SAPI.SpVoice");
                if (sapiType == null) return;
                dynamic sapi = Activator.CreateInstance(sapiType);
                sapi.Speak(text);
            }
            catch
            {
                // ignore if SAPI not available
            }
        }
    }
}
