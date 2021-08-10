using System;
using System.IO;
using System.Linq;
using System.Diagnostics;
using System.Collections.Generic;



namespace Hangman
{
    class Program
    {
        static string ToMaskedWord(string unmaskedWord, string lettersToUnmask)
        {
            string maskedWord = "\nCapital to guess:\n";
            foreach (char c in unmaskedWord)
            {
                if (Char.IsWhiteSpace(c))
                {
                    maskedWord += c;
                }
                else if (lettersToUnmask.Contains(c))
                {
                    maskedWord += c;
                }
                else
                {
                    maskedWord += "_";
                }
            }
            return maskedWord;
        }

        static bool IsOnlyLetters(string s)
        {
            s = s.Replace(Environment.NewLine, string.Empty);
            foreach (char c in s)
            {
                if (!Char.IsLetter(c))
                    return false;
            }
            return true;
        }
        static bool IsOnlyLettersOrWhiteSpace(string s)
        {
            s = s.Replace(Environment.NewLine, string.Empty);
            foreach (char c in s)
            {
                if (!(Char.IsLetter(c) || Char.IsWhiteSpace(c)))
                    return false;
            }
            return true;
        }

        static void Main(string[] args)
        {
            string[] hangmanGallery = {
            "\n  +---+\n  |   |\n      |\n      |\n      |\n      |\n------+--\n",
            "\n  +---+\n  |   |\n  O   |\n      |\n      |\n      |\n------+--\n",
            "\n  +---+\n  |   |\n  O   |\n  |   |\n      |\n      |\n------+--\n",
            "\n  +---+\n  |   |\n  O   |\n  |\\  |\n      |\n      |\n------+--\n",
            "\n  +---+\n  |   |\n  O   |\n /|\\  |\n      |\n      |\n------+--\n",
            "\n  +---+\n  |   |\n  O   |\n /|\\  |\n   \\  |\n      |\n------+--\n",
            "\n  +---+\n  |   |\n  O   |\n /|\\  |\n / \\  |\n      |\n------+--\n",
            };
            string welcome = "Welcome to Jakub Kolodziej's Hangman Game";
            Console.SetCursorPosition((Console.WindowWidth - welcome.Length) / 2, Console.CursorTop);
            Console.WriteLine(welcome);
            var random = new Random();
            bool restart = false;
            while (!restart)
            {
                int lives = 6;
                Console.WriteLine($"\nHere are the rules:\n" +
                    $"\nIn order to win, you have to guess random country's capital name. " +
                    $"\nYou start with {lives} lives. You will lose one life after wrong letter guess or two lives after wrong whole word guess." +
                    $"\nGame is over when you reach 0 lives.\n");
                Console.WriteLine($"\nWould you like to guess capitals only from [E]urope or from a whole [W]orld? (pick E or W and press <Enter>)\n");
                string sCurrentDirectory = AppDomain.CurrentDomain.BaseDirectory;
                string countriesAndCapitalsFile = "";
                while (true)
                {
                    string userChoice = Console.ReadLine().ToUpper();
                    if (userChoice == "E")
                    {
                        countriesAndCapitalsFile = System.IO.Path.Combine(sCurrentDirectory, @"..\..\..\countries_and_capitals_europe.txt");
                        break;
                    }
                    else if (userChoice == "W")
                    {
                        countriesAndCapitalsFile = System.IO.Path.Combine(sCurrentDirectory, @"..\..\..\countries_and_capitals.txt");
                        break;
                    }
                    else
                    {
                        Console.WriteLine("\nWrong button, pick again.\n");
                    }
                }
                string countriesAndCapitalsFilePath = Path.GetFullPath(countriesAndCapitalsFile);
                if (!File.Exists(countriesAndCapitalsFilePath))
                {
                    Console.WriteLine("File not found");
                    System.Environment.Exit(0);
                }
                string[] lines = File.ReadAllLines(countriesAndCapitalsFilePath);
                int index = random.Next(lines.Length);
                string secret = lines[index].ToUpper();
                string secretCountry = secret.Split(" | ")[0];
                string secretCapital = secret.Split(" | ")[1];
                string tempCapital = secretCapital;
                string guessedCapitalLetters = "";
                List<string> guessedCapitalWords = new List<string>();
                string notInWord = "";
                string keyInput;
                bool result;
                int guessingTries = 0;
                string combinedResults = "";
                string maskedCapital = ToMaskedWord(secretCapital, guessedCapitalLetters);
                Console.WriteLine($"{maskedCapital}");
                Stopwatch stopWatch = new Stopwatch();
                stopWatch.Start();
                Console.WriteLine($"{hangmanGallery[6 - lives]}");
                while (lives > 0)
                {
                    Console.WriteLine("\nWould you like to enter a letter [press L] or a whole word [press W]?");
                    switch (Console.ReadKey().Key)
                    {
                        case ConsoleKey.L:
                            while (true)
                            {
                                Console.WriteLine("\nWrite a letter and press <Enter>: ");
                                keyInput = Console.ReadLine();
                                result = IsOnlyLetters(keyInput);
                                Console.Clear();
                                keyInput = keyInput.ToUpper();
                                if (!result)
                                {
                                    Console.WriteLine($"\nPlease, pick only letters, not {keyInput}\n");
                                }
                                else if (keyInput.Length != 1)
                                {
                                    Console.WriteLine("\nPlease, provide exactly one letter.\n");
                                }
                                else if (guessedCapitalLetters.Contains(keyInput))
                                {
                                    Console.WriteLine($"\nYou've already tried {keyInput}, pick a different letter.\n");
                                }
                                else
                                {
                                    if (tempCapital.Contains(keyInput))
                                    {
                                        tempCapital = tempCapital.Replace(keyInput, string.Empty);
                                        guessedCapitalLetters += keyInput;
                                        guessingTries++;
                                        Console.WriteLine("\nGreat! You guessed right.\n");
                                    }
                                    else
                                    {
                                        notInWord += keyInput + " ";
                                        lives--;
                                        guessingTries++;
                                        Console.WriteLine("\nSorry, wrong letter! You lose a life point.\n");
                                    }
                                }
                                maskedCapital = ToMaskedWord(secretCapital, guessedCapitalLetters);
                                Console.WriteLine($"{maskedCapital}");
                                break;
                            }
                            break;
                        case ConsoleKey.W:
                            while (true)
                            {
                                if (lives < 3)
                                {
                                    Console.WriteLine("\nNot enough life points to guess a whole word, try guessing letter instead.");
                                    Console.WriteLine("\nWrite a letter and press <Enter>: ");
                                    keyInput = Console.ReadLine();
                                    result = IsOnlyLetters(keyInput);
                                    Console.Clear();
                                    keyInput = keyInput.ToUpper();
                                    if (!result)
                                    {
                                        Console.WriteLine($"\nPlease, pick only letters, not {keyInput}\n");
                                    }
                                    else if (keyInput.Length != 1)
                                    {
                                        Console.WriteLine("\nPlease, provide exactly one letter.\n");
                                    }
                                    else if (guessedCapitalLetters.Contains(keyInput))
                                    {
                                        Console.WriteLine($"\nYou've already tried {keyInput}, pick a different letter.\n");
                                    }
                                    else
                                    {
                                        if (secretCapital.Contains(keyInput))
                                        {
                                            tempCapital = tempCapital.Replace(keyInput, string.Empty);
                                            guessedCapitalLetters += keyInput;
                                            guessingTries++;
                                            Console.WriteLine("\nGreat! You guessed right.\n");
                                        }
                                        else
                                        {
                                            notInWord += keyInput + " ";
                                            lives--;
                                            guessingTries++;
                                            Console.WriteLine("\nSorry, wrong letter! You lose a life point.\n");
                                        }
                                    }
                                    maskedCapital = ToMaskedWord(secretCapital, guessedCapitalLetters);
                                    Console.WriteLine($"{maskedCapital}");
                                    break;
                                }
                                else
                                {
                                    Console.WriteLine("\nWrite a word and press <Enter>: \n");
                                    keyInput = Console.ReadLine();
                                    result = IsOnlyLettersOrWhiteSpace(keyInput);
                                    Console.Clear();
                                    keyInput = keyInput.ToUpper();
                                    if (!result)
                                    {
                                        Console.WriteLine($"\nPlease, write a word with only letters, not {keyInput}\n");
                                    }
                                    else if (String.IsNullOrEmpty(keyInput))
                                    {
                                        Console.WriteLine("\nPlease, write a word (must contain at least one letter).\n");
                                    }
                                    else if (guessedCapitalWords.Contains(keyInput))
                                    {
                                        Console.WriteLine($"\nYou've already tried {keyInput}, write a different word.\n");
                                    }
                                    else
                                    {
                                        if (secretCapital == keyInput)
                                        {
                                            tempCapital = string.Empty;
                                            guessingTries++;
                                            Console.WriteLine("\nGreat! You guessed right.\n");
                                            maskedCapital = ToMaskedWord(secretCapital, keyInput);
                                            Console.WriteLine($"{maskedCapital}");
                                            break;
                                        }
                                        else
                                        {
                                            lives -= 2;
                                            Console.WriteLine("\nSorry, wrong capital! You lose two life points.\n");
                                            guessedCapitalWords.Add(keyInput);
                                            guessingTries++;
                                        }  
                                    }
                                    maskedCapital = ToMaskedWord(secretCapital, guessedCapitalLetters);
                                    Console.WriteLine($"{maskedCapital}");
                                    break;
                                }
                            }
                            break;
                        default:
                            Console.Clear();
                            Console.WriteLine("\nWrong button.\n");
                            maskedCapital = ToMaskedWord(secretCapital, guessedCapitalLetters);
                            Console.WriteLine($"{maskedCapital}");
                            break;
                    }
                    if (String.IsNullOrEmpty(tempCapital))
                    {
                        stopWatch.Stop();
                        Console.WriteLine("\nCongratulations! You are the winner!\n");
                        TimeSpan ts = stopWatch.Elapsed;
                        string elapsedTime = String.Format("{0}m {1}s",ts.Minutes, ts.Seconds);
                        DateTime localDate = DateTime.Now;
                        Console.WriteLine($"\nYou guessed the capital after {guessingTries} tries. It took you {elapsedTime}.\n");
                        Console.WriteLine("\nWrite your name to save results and press <Enter>: \n");
                        string name = Console.ReadLine();
                        combinedResults = $"{name} | {localDate} | {elapsedTime} | {guessingTries} | {secretCapital}";
                        Console.WriteLine($"\n{combinedResults}\n");
                        break;
                    }
                    Console.WriteLine($"{hangmanGallery[6 - lives]}");
                    if (notInWord.Length > 1)
                    {
                        Console.WriteLine($"\nLetters that do not appear in the word: {notInWord}\n");
                    }
                    Console.WriteLine($"\nWhole words you tried to guess:\n");
                    guessedCapitalWords.ForEach(i => Console.Write("{0} ", i));
                    if (lives > 0)
                    {
                        Console.WriteLine($"\n\nYou have {lives} lives left.");
                    }
                    else
                    {
                        Console.WriteLine($"\n\nYou lose.");
                    }
                    if (lives == 1)
                    {
                        Console.WriteLine($"\nThe word to guess is the capital of {secretCountry}.");
                    }
                }
                Console.WriteLine("TOP 10\n");
                string rankingFile = System.IO.Path.Combine(sCurrentDirectory, @"..\..\..\ranking.txt");
                string rankingFilePath = Path.GetFullPath(rankingFile);
                if (!File.Exists(rankingFilePath))
                {
                    using (StreamWriter sw = File.CreateText(rankingFilePath))
                    {
                        sw.WriteLine(combinedResults);
                    }
                }
                string[] ranking = File.ReadAllLines(rankingFilePath);
                List<string> highscores = new List<string>(ranking);
                highscores.Add(combinedResults);
                if (highscores.Count > 10)
                {
                    highscores.RemoveAt(highscores.Count - 1);
                }
                var sortedHighscores = highscores.OrderBy(score => Convert.ToInt32(score.Split(" | ")[3])).ThenBy(time => (Convert.ToInt32(time.Split(" | ")[2].Replace("s", string.Empty).Split("m ")[0]) * 60 + Convert.ToInt32(time.Split(" | ")[2].Replace("s", string.Empty).Split("m ")[1])));
                foreach (string h in sortedHighscores)
                {
                    Console.WriteLine($"{h}");
                }
                File.WriteAllText(rankingFilePath, string.Empty);
                File.WriteAllLines(rankingFilePath, sortedHighscores);
                Console.WriteLine($"\nWould you like to try again? \nPick [Y]es or [N]o and press <Enter>");
                while (true)
                {
                    string userChoice = Console.ReadLine().ToUpper();
                    if (userChoice == "N")
                    {
                        restart = true;
                        break;
                    }
                    else if (userChoice == "Y")
                    {
                        break;
                    }
                    else
                    {
                        Console.WriteLine("\nWrong button, pick again.\n");
                    }
                }
                Console.Clear();

            }
        }
    }
}
