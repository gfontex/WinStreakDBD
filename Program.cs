using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace WinStreakTracker
{
    // Classe para armazenar os dados de cada personagem
    public class StreakData
    {
        public int CurrentStreak { get; set; }
        public int HighestStreak { get; set; }
    }

    class Program
    {
        private static Dictionary<string, StreakData> _killerStreaks;

        private static string SaveFilePath
        {
            get
            {
                string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                string appFolderPath = Path.Combine(appDataPath, "WinStreakTracker");
                Directory.CreateDirectory(appFolderPath);
                return Path.Combine(appFolderPath, "streaks.dat");
            }
        }

        static void Main(string[] args)
        {
            _killerStreaks = LoadStreaks();

            while (true)
            {
                ShowMainMenu();
                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        SelectCharacterAndPlay();
                        break;
                    case "2":
                        ShowAllStreaksScreen();
                        break;
                    case "3":
                        ResetStreaksMenu();
                        break;
                    case "4":
                        return; // Sai do programa
                    default:
                        ShowMessage("Opção inválida. Pressione qualquer tecla para tentar novamente.", ConsoleColor.Red, true);
                        break;
                }
            }
        }

        private static void ShowMainMenu()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("╔══════════════════════════════════════╗");
            Console.WriteLine("║   CONTADOR DE SEQUÊNCIA DE VITÓRIAS  ║");
            Console.WriteLine("╚══════════════════════════════════════╝");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("\nby: Biel.");
            Console.ResetColor();

            Console.WriteLine("\nMENU PRINCIPAL:");
            Console.WriteLine("  1. Iniciar / Continuar Winstreak");
            Console.WriteLine("  2. Exibir Todas as Winstreaks");
            Console.WriteLine("  3. Zerar Winstreaks");
            Console.WriteLine("  4. Sair");
            Console.Write("\nEscolha uma opção: ");
        }

        private static void SelectCharacterAndPlay()
        {
            Console.Clear();
            Console.Write("Com qual personagem você vai jogar hoje? (ou 'voltar') > ");
            string playerChoice = Console.ReadLine()?.Trim();

            if (playerChoice.Equals("voltar", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            var matchedKey = _killerStreaks.Keys.FirstOrDefault(k => k.Equals(playerChoice, StringComparison.OrdinalIgnoreCase));

            if (string.IsNullOrEmpty(matchedKey))
            {
                ShowMessage("Personagem inválido!", ConsoleColor.Red, true);
            }
            else
            {
                TrackWins(matchedKey);
            }
        }

        private static void ShowAllStreaksScreen()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("--- TODAS AS WINSTREAKS ---\n");
            Console.ResetColor();

            var sortedStreaks = _killerStreaks.OrderByDescending(s => s.Value.HighestStreak)
                                              .ThenByDescending(s => s.Value.CurrentStreak);

            foreach (var streakEntry in sortedStreaks)
            {
                string characterName = streakEntry.Key;
                StreakData data = streakEntry.Value;
                string output = $"- {characterName,-18} | Atual: {data.CurrentStreak,-3} (Recorde: {data.HighestStreak})";

                if (data.CurrentStreak > 0)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"{output}");
                    Console.ResetColor();
                }
                else
                {
                    Console.WriteLine(output);
                }
            }
            ShowMessage("\nPressione qualquer tecla para voltar ao menu.", ConsoleColor.Gray, false);
            Console.ReadKey();
        }

        private static void ResetStreaksMenu()
        {
            while (true)
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("--- ZERAR WINSTREAKS ---");
                Console.ResetColor();
                Console.WriteLine("\n  1. Zerar um personagem específico");
                Console.WriteLine("  2. ZERAR TODOS OS DADOS (AÇÃO IRREVERSÍVEL)");
                Console.WriteLine("  3. Voltar ao menu principal");
                Console.Write("\nEscolha uma opção: ");

                string choice = Console.ReadLine();
                switch (choice)
                {
                    case "1":
                        Console.Clear();
                        Console.Write("Qual personagem você deseja zerar? > ");
                        string charToReset = Console.ReadLine()?.Trim();
                        var matchedKey = _killerStreaks.Keys.FirstOrDefault(k => k.Equals(charToReset, StringComparison.OrdinalIgnoreCase));
                        if (string.IsNullOrEmpty(matchedKey))
                        {
                            ShowMessage("Personagem não encontrado!", ConsoleColor.Red, true);
                        }
                        else
                        {
                            Console.Write($"Tem certeza que deseja zerar os dados de {matchedKey}? (s/n) > ");
                            if (Console.ReadLine()?.Trim().ToLower() == "s")
                            {
                                _killerStreaks[matchedKey] = new StreakData { CurrentStreak = 0, HighestStreak = 0 };
                                SaveStreaks();
                                ShowMessage("Dados do personagem zerados com sucesso!", ConsoleColor.Green, true);
                            }
                            else
                            {
                                ShowMessage("Operação cancelada.", ConsoleColor.Yellow, true);
                            }
                        }
                        break;
                    case "2":
                        Console.Clear();
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine("\nATENÇÃO: Esta ação apagará TODOS os seus recordes e sequências.");
                        Console.Write("Digite 'CONFIRMAR' para continuar > ");
                        Console.ResetColor();
                        if (Console.ReadLine()?.Trim() == "CONFIRMAR")
                        {
                            var characterKeys = _killerStreaks.Keys.ToList();
                            foreach (var key in characterKeys)
                            {
                                _killerStreaks[key] = new StreakData { CurrentStreak = 0, HighestStreak = 0 };
                            }
                            SaveStreaks();
                            ShowMessage("TODOS OS DADOS FORAM ZERADOS!", ConsoleColor.Green, true);
                        }
                        else
                        {
                            ShowMessage("Operação cancelada.", ConsoleColor.Yellow, true);
                        }
                        break;
                    case "3":
                        return;
                    default:
                        ShowMessage("Opção inválida.", ConsoleColor.Red, true);
                        break;
                }
            }
        }

        private static void TrackWins(string character)
        {
            while (true)
            {
                Console.Clear();
                var streakData = _killerStreaks[character];

                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"--- Jogando com: {character} ---");
                Console.ResetColor();

                Console.WriteLine($"\nVitórias em sequência atuais: {streakData.CurrentStreak}");
                Console.WriteLine($"Recorde de vitórias: {streakData.HighestStreak}\n");

                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write("Você venceu a partida? (s = sim / n = não / voltar) > ");
                Console.ResetColor();

                string result = Console.ReadLine()?.ToLower().Trim();

                if (result == "s" || result == "sim")
                {
                    streakData.CurrentStreak++;
                    if (streakData.CurrentStreak > streakData.HighestStreak)
                    {
                        streakData.HighestStreak = streakData.CurrentStreak;
                        ShowMessage($"NOVO RECORDE! Sequência atual: {streakData.CurrentStreak} vitórias!", ConsoleColor.Green);
                    }
                    else
                    {
                        ShowMessage($"Ótimo! Sua sequência agora é de {streakData.CurrentStreak} vitórias!", ConsoleColor.Green);
                    }
                }
                else if (result == "n" || result == "nao")
                {
                    streakData.CurrentStreak = 0;
                    ShowMessage($"Que pena! Sequência de vitórias zerada.", ConsoleColor.Red);
                }
                else if (result.StartsWith("voltar") || result.StartsWith("sair"))
                {
                    SaveStreaks();
                    return;
                }
                else
                {
                    ShowMessage("Opção inválida.", ConsoleColor.Red);
                }

                SaveStreaks();
                Console.WriteLine("\nPressione qualquer tecla para a próxima partida...");
                Console.ReadKey();
            }
        }

        private static void ShowMessage(string message, ConsoleColor color, bool waitForKey = false)
        {
            ConsoleColor originalColor = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Console.WriteLine(message);
            Console.ForegroundColor = originalColor;
            if (waitForKey)
            {
                Console.Write("\nPressione qualquer tecla para continuar...");
                Console.ReadKey();
            }
        }

        private static Dictionary<string, StreakData> LoadStreaks()
        {
            string filePath = SaveFilePath;
            var streaks = GetDefaultStreaks();

            if (!File.Exists(filePath))
            {
                return streaks;
            }

            string[] lines = File.ReadAllLines(filePath);

            foreach (string line in lines)
            {
                string[] parts = line.Split(':');
                if (parts.Length == 3)
                {
                    string characterName = parts[0];
                    if (streaks.ContainsKey(characterName) &&
                        int.TryParse(parts[1], out int current) &&
                        int.TryParse(parts[2], out int highest))
                    {
                        streaks[characterName].CurrentStreak = current;
                        streaks[characterName].HighestStreak = highest;
                    }
                }
            }
            return streaks;
        }

        private static void SaveStreaks()
        {
            string filePath = SaveFilePath;
            var lines = _killerStreaks.Select(kvp => $"{kvp.Key}:{kvp.Value.CurrentStreak}:{kvp.Value.HighestStreak}");
            File.WriteAllLines(filePath, lines);
        }


        private static Dictionary<string, StreakData> GetDefaultStreaks()
        {
            return new Dictionary<string, int>
            {
                { "Trapper", 0 }, { "Wraith", 0 }, { "Hillbilly", 0 }, { "Nurse", 0 },
                { "Shape", 0 }, { "Hag", 0 }, { "Doctor", 0 }, { "Huntress", 0 },
                { "Cannibal", 0 }, { "Nightmare", 0 }, { "Pig", 0 }, { "Clown", 0 },
                { "Spirit", 0 }, { "Legion", 0 }, { "Plague", 0 }, { "Ghost_Face", 0 },
                { "Demogorgon", 0 }, { "Executioner", 0 }, { "Blight", 0 }, { "Twins", 0 },
                { "Trickster", 0 }, { "Nemesis", 0 }, { "Cenobite", 0 }, { "Artist", 0 },
                { "Onryo", 0 }, { "Mastermind", 0 }, { "Knight", 0 }, { "Skull_Merchant", 0 },
                { "Singularity", 0 }, { "Xenomorph", 0 }, { "Good_Guy", 0 }, { "Unknown", 0 },
                { "Lich", 0 }, { "Houndmaster", 0 }, { "Animatronic", 0 }, { "Dark_lord", 0 },
                { "Survivor", 0 },
                { "Kaneki", 0 },
                { "Krasue", 0 }
            }.ToDictionary(kvp => kvp.Key, kvp => new StreakData { CurrentStreak = 0, HighestStreak = 0 });
        }
    }
}

