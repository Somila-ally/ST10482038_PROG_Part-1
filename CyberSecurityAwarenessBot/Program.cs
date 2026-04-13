using System;
using System.IO;
using System.Media;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace CyberSecurityAwarenessBot
{
    internal static class Program
    {
        // Audio features removed — start with a text welcome

        private static void Main()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.Clear();

            // Show immediate welcome as soon as the screen displays
            TypeWrite("Hello Welcome to the Cybersecurity Awareness ChatBot", ConsoleColor.Green);
            // Speak the welcome immediately using system speech (SAPI) if available
            try
            {
                AudioPlayer.SpeakText("Hello Welcome to the Cybersecurity Awareness ChatBot");
            }
            catch
            {
                // ignore SAPI failures
            }
            // Play greeting WAV synchronously if present
            try
            {
                AudioPlayer.EnsureGreetingAudioExists();
                AudioPlayer.PlayGreetingIfExists();
            }
            catch
            {
                // ignore audio failures
            }

            // ASCII art logo
            var logo = @"  ____ _        _                 ____        _   
 / ___| | _ __ (_) ___  _ __ ___ / ___|  __ _| |_ 
| |   | || '_ \| |/ _ \| '_ ` _ \\___ \ / _` | __|
| |___| || |_) | | (_) | | | | | |___) | (_| | |_ 
 \____|_|| .__/|_|\___/|_| |_| |_|____/ \__,_|\__|
         |_|                                      
    AEGIS CYBER SENTINEL - Cybersecurity Awareness Assistant
";

            // Use consistent green color for main UI elements
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(logo);
            Console.ResetColor();

            DrawSeparator();

            // Big welcome banner before asking name
            var bigWelcome = @" __        __   _                            _          _   _                  _   _                \
 \ \      / /__| | ___ ___  _ __ ___   ___  | |_ ___   | |_| |__   ___  _ __  | \ | | _____      __ \
  \ \ /\ / / _ \ |/ __/ _ \| '_ ` _ \ / _ \ | __/ _ \  | __| '_ \ / _ \| '_ \ |  \| |/ _ \ \ /\ / / \
   \ V  V /  __/ | (_| (_) | | | | | |  __/ | || (_) | | |_| | | | (_) | | | || |\  |  __/\ V  V /  \
    \_/\_/ \___|_|\___\___/|_| |_| |_|\___|  \__\___/   \__|_| |_|\___/|_| |_|\_| \_/\___| \_/\_/   ";

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(bigWelcome);
            Console.ResetColor();
            Console.WriteLine();

            // Audio handled earlier so the UI appears immediately

            var user = new User();
            while (string.IsNullOrWhiteSpace(user.Name))
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write("What is your name? ");
                Console.ResetColor();
                var name = Console.ReadLine()?.Trim();
                if (string.IsNullOrWhiteSpace(name))
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("Please enter a name (cannot be empty).\n");
                    Console.ResetColor();
                    continue;
                }

                user.Name = name;
                // Immediate personalized welcome so the user sees confirmation right away
                TypeWrite($"Nice to meet you, {user.Name}!", ConsoleColor.Green, 5);
                // If SAPI is available, speak a short personalized greeting
                Task.Run(() =>
                {
                    try
                    {
                        AudioPlayer.SpeakText($"Hello {user.Name}. Welcome to the Cybersecurity Awareness Bot.");
                    }
                    catch
                    {
                        // ignore speech failures
                    }
                });
            }

            // Clear and reprint logo so menu is visible
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(logo);
            Console.ResetColor();
            DrawSeparator();

            // Initialize chatbot and show welcome + menu after name is entered
            var bot = new Chatbot();
            TypeWrite($"WELCOME TO AEGIS CYBER SENTINEL, {user.Name}!", ConsoleColor.Green);
            Console.WriteLine();
            RenderBoxedMenu(bot);

            TypeWrite($"Hello, {user.Name}! I am Aegis, your Cyber Sentinel. Ask me about cybersecurity, type a menu number (1-25), or type 'menu' to see the list again. Type 'exit' to quit.\n\n");

            while (true)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write($"{user.Name}> ");
                Console.ResetColor();
                var input = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(input))
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("(Please type a question, a menu number, 'menu', or 'exit')");
                    Console.ResetColor();
                    continue;
                }

                input = input.Trim();
                if (input.Equals("exit", StringComparison.OrdinalIgnoreCase))
                {
                    TypeWrite("Goodbye! Stay safe online.", ConsoleColor.Green);
                    break;
                }

                if (input.Equals("menu", StringComparison.OrdinalIgnoreCase))
                {
                    RenderBoxedMenu(bot);
                    continue;
                }

                var response = bot.GetResponse(input, user);
                TypeWrite(response);
                Console.WriteLine();
            }
        }
        // audio lookup removed

        // Removed PowerShell-based TTS generation. Playback will only occur if a local 'greeting.wav' exists.

        // Audio playback / generation logic has been moved to AudioPlayer.cs to keep Program.cs focused on
        // application flow and UI. Calls in Main above invoke AudioPlayer.EnsureGreetingAudioExists and
        // AudioPlayer.PlayGreetingIfExists.

        private static void RenderBoxedMenu(Chatbot bot)
        {
            var topics = bot.GetTopics();
            const int cols = 2;
            var total = topics.Length;
            var rows = (total + cols - 1) / cols;

            // Build grid
            var grid = new string[rows, cols];
            for (var i = 0; i < total; i++)
            {
                var r = i % rows;
                var c = i / rows;
                grid[r, c] = $"{i + 1}. {topics[i]}";
            }

            // Compute column widths
            var colWidths = new int[cols];
            for (var c = 0; c < cols; c++)
            {
                var max = 0;
                for (var r = 0; r < rows; r++)
                {
                    var val = grid[r, c];
                    if (string.IsNullOrEmpty(val)) continue;
                    if (val.Length > max) max = val.Length;
                }
                colWidths[c] = Math.Max(20, max);
            }

            var innerWidth = colWidths[0] + colWidths[1] + 7; // spacing and separators
            var boxWidth = Math.Min(Math.Max(innerWidth, 40), Math.Max(40, Console.WindowWidth - 2));

            // Top border
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine("+" + new string('-', boxWidth) + "+");

            // Header
            var header = " TOPICS MENU ";
            var padding = Math.Max(0, boxWidth - header.Length);
            var leftPad = padding / 2;
            var rightPad = padding - leftPad;
            Console.Write('|');
            Console.Write(new string(' ', leftPad));
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write(header);
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write(new string(' ', rightPad));
            Console.WriteLine('|');

            // Separator under header
            Console.WriteLine("+" + new string('-', boxWidth) + "+");

            // Rows
            for (var r = 0; r < rows; r++)
            {
                // Left column
                var left = grid[r, 0] ?? string.Empty;
                var right = grid[r, 1] ?? string.Empty;

                var leftNumEnd = left.IndexOf('.', StringComparison.Ordinal);
                var rightNumEnd = right.IndexOf('.', StringComparison.Ordinal);

                // Build padded columns
                var leftPadded = left.PadRight(colWidths[0] + 3);
                var rightPadded = right.PadRight(colWidths[1] + 3);

                Console.Write('|');
                // Left number
                if (leftNumEnd > 0)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write(left.Substring(0, leftNumEnd + 1));
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write(left.Substring(leftNumEnd + 1).PadRight(colWidths[0] - (leftNumEnd + 1) + 3));
                }
                else
                {
                    Console.Write(leftPadded);
                }

                // Spacer between columns
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.Write("|");

                // Right number
                if (rightNumEnd > 0)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write(right.Substring(0, rightNumEnd + 1));
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write(right.Substring(rightNumEnd + 1).PadRight(colWidths[1] - (rightNumEnd + 1) + 3));
                }
                else
                {
                    Console.Write(rightPadded);
                }

                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.WriteLine('|');
            }

            // Bottom border
            Console.WriteLine("+" + new string('-', boxWidth) + "+");
            Console.ResetColor();
            Console.WriteLine();
        }

        private static void DrawSeparator()
        {
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine(new string('-', Console.WindowWidth > 80 ? 80 : Math.Max(1, Console.WindowWidth - 1)));
            Console.ResetColor();
        }

        internal static void TypeWrite(string text, int delayMs = 0)
        {
            foreach (var ch in text)
            {
                Console.Write(ch);
                if (delayMs > 0) Thread.Sleep(delayMs);
            }
            Console.WriteLine();
        }

        internal static void TypeWrite(string text, ConsoleColor color, int delayMs = 0)
        {
            var prev = Console.ForegroundColor;
            Console.ForegroundColor = color;
            foreach (var ch in text)
            {
                Console.Write(ch);
                if (delayMs > 0) Thread.Sleep(delayMs);
            }
            Console.WriteLine();
            Console.ForegroundColor = prev;
        }

        private static void WriteDebug(string message)
        {
            try
            {
                var prev = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.WriteLine(message);
                Console.ForegroundColor = prev;
            }
            catch
            {
                // ignore
            }
        }
    }
}
