using System;

namespace CyberSecurityAwarenessBot
{
    internal class Chatbot
    {
        private static readonly string[] Topics = new[]
        {
            "What is Cybersecurity",
            "Phishing Awareness",
            "Strong Passwords",
            "Multi-Factor Authentication",
            "Safe Browsing",
            "Email Safety",
            "Social Engineering",
            "Malware Types",
            "Ransomware",
            "Software Updates",
            "Secure Wi-Fi",
            "Public Hotspot Risks",
            "Backing Up Data",
            "Two-Step Verification",
            "Recognizing Scams",
            "Secure Mobile Practices",
            "Privacy Settings",
            "Safe Downloads",
            "Identifying Fake Websites",
            "Secure File Sharing",
            "Using VPNs Safely",
            "Device Encryption",
            "Reporting Incidents",
            "IoT Device Security",
            "Protecting Children Online"
        };

        public string[] GetTopics() => Topics;

        public string GetMenu()
        {
            var menu = "";
            for (var i = 0; i < Topics.Length; i++)
            {
                menu += $"{i + 1}. {Topics[i]}\n";
            }
            menu += "\nType the number (1-25) to learn more about a topic, or ask a question.\n";
            return menu;
        }

        public string GetResponse(string input, User user)
        {
            if (string.IsNullOrWhiteSpace(input)) return "I didn't catch that. Could you rephrase?";

            var trimmed = input.Trim();

            // Handle numeric menu selection
            if (int.TryParse(trimmed, out var n))
            {
                if (n >= 1 && n <= Topics.Length)
                {
                    return GetTopicDetail(n - 1);
                }
            }

            var lower = trimmed.ToLowerInvariant();

            if (lower.Contains("how are you") || lower.Contains("how r you") || lower.Contains("how are u"))
                return "I'm a bot, but I'm functioning as expected! Thanks for asking.";

            if (lower.Contains("purpose") || lower.Contains("what do you do") || lower.Contains("what is your purpose"))
                return "I help raise cybersecurity awareness by answering basic questions and guiding safe online habits.";

            if (lower.Contains("phishing") || lower.Contains("phish"))
                return "Phishing is a social engineering attack where attackers trick you into giving sensitive info. Never click unknown links or provide credentials via email.";

            if (lower.Contains("password"))
                return "Use a strong, unique password for each account and enable multi-factor authentication whenever possible.";

            if (lower.Contains("malware") || lower.Contains("virus"))
                return "Malware is malicious software. Keep your OS and apps updated, avoid unknown downloads, and use reputable antivirus tools.";

            if (lower.Contains("what is cyber security") || lower.Contains("what is cybersecurity"))
                return "Cybersecurity is the practice of protecting systems, networks, and programs from digital attacks.";

            if (lower.EndsWith("?") || lower.Contains("how") || lower.Contains("what") || lower.Contains("why") || lower.Contains("when"))
                return "That's a good question. For more detailed guidance, try asking about phishing, passwords, or malware, or choose a topic from the menu.";

            return "I'm not sure about that. Try asking about 'phishing', 'passwords', 'malware', or type a menu number (1-25).";
        }

        private static string GetTopicDetail(int index)
        {
            return index switch
            {
                0 => "Cybersecurity: the practice of protecting systems, networks, and data from digital attacks. It includes policies, tools, and best practices.",
                1 => "Phishing Awareness: be cautious of unexpected emails, check sender addresses, hover links before clicking, and never provide credentials to untrusted sites.",
                2 => "Strong Passwords: use long, unique passwords or a passphrase. Consider a password manager to store and generate strong passwords.",
                3 => "Multi-Factor Authentication: adds an extra verification step (SMS, app, hardware key) to reduce risk if a password is compromised.",
                4 => "Safe Browsing: keep your browser updated, avoid suspicious sites, and be careful with browser extensions and downloads.",
                5 => "Email Safety: don't open attachments from unknown senders, verify links, and be wary of urgent requests for information.",
                6 => "Social Engineering: attackers manipulate people into revealing information. Verify identities and be skeptical of unsolicited requests.",
                7 => "Malware Types: viruses, trojans, worms, spyware — all can harm devices. Keep software updated and avoid untrusted downloads.",
                8 => "Ransomware: malware that encrypts files and demands payment. Regular backups and system updates help mitigate risk.",
                9 => "Software Updates: install updates promptly to patch security vulnerabilities that attackers can exploit.",
                10 => "Secure Wi-Fi: change default router passwords, use WPA3/WPA2, and avoid using weak encryption or open networks for sensitive tasks.",
                11 => "Public Hotspot Risks: public Wi-Fi can be monitored. Avoid logging into sensitive accounts and use a VPN when on public hotspots.",
                12 => "Backing Up Data: regular backups protect against data loss from hardware failure or ransomware. Keep offline or offsite copies.",
                13 => "Two-Step Verification: similar to MFA; use authentication apps or hardware keys for stronger security than SMS alone.",
                14 => "Recognizing Scams: check URLs, spelling errors, and too-good-to-be-true offers. When in doubt, contact the organization directly.",
                15 => "Secure Mobile Practices: keep mobile OS updated, install apps from trusted stores, and review app permissions.",
                16 => "Privacy Settings: review social media and app privacy settings to limit data sharing and location access.",
                17 => "Safe Downloads: only download from official sources and verify digital signatures when available.",
                18 => "Identifying Fake Websites: check HTTPS, domain spelling, and certificates. Look for trust indicators and avoid entering credentials on suspicious sites.",
                19 => "Secure File Sharing: use trusted services, set expiration links, and limit permissions when sharing sensitive files.",
                20 => "Using VPNs Safely: VPNs encrypt traffic on untrusted networks. Choose reputable providers and avoid free VPNs with unclear policies.",
                21 => "Device Encryption: encrypt disks to protect data if a device is lost or stolen. Use built-in OS encryption tools when possible.",
                22 => "Reporting Incidents: report suspicious emails or security incidents to your IT team or relevant authorities quickly.",
                23 => "IoT Device Security: change defaults, keep firmware updated, and isolate IoT devices on separate networks when possible.",
                24 => "Protecting Children Online: use parental controls, educate children about privacy, and monitor online interactions appropriately.",
                _ => "Topic not found."
            };
        }
    }
}
