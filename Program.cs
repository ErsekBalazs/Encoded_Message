Encrypt encrypt = new();
(string encrypt1, string encrypt2, string keyUsed) = encrypt.TestMethod();
encrypt.KeyFinder(encrypt1, encrypt2);
foreach(string key in encrypt.possibleKeys)
    Console.WriteLine(key);

Console.WriteLine(encrypt.possibleKeys.Contains(keyUsed));


public class Encrypt
{
    private List<char> validCharacters = new List<char> { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l',
             'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z', ' ' };

    public List<string> words = File.ReadAllLines("words.txt").ToList();
    public List<string> possibleKeys = new List<string>();

    bool ValidityCheck(string text)
    {
        foreach (char c in text)
        {
            if (!validCharacters.Contains(c))
                return false;
        }
        return true;
    }

    public string Encoder(string message, string key)
    {
        if (message.Length > key.Length || message == null)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Key is shorter than message or there is no message given!");
            Console.ForegroundColor = ConsoleColor.White;
            return string.Empty;
        }

        List<char> encodedCharacters = new List<char>();

        for (int i = 0; i < message.Length; i++)
        {
            int charIndex = (validCharacters.IndexOf(message[i]) + validCharacters.IndexOf(key[i])) % 27;
            encodedCharacters.Add(validCharacters[charIndex]);
        }
        string? encodedMessage = new string(encodedCharacters.ToArray());
        return encodedMessage == null ? string.Empty : encodedMessage;
    }

    public string Decoder(string encodedMessage, string key)
    {
        List<char> decodedCharacters = new List<char>();

        int range = Math.Min(encodedMessage.Length, key.Length);

        for (int i = 0; i < range; i++)
        {
            int charIndex = (validCharacters.IndexOf(encodedMessage[i]) - validCharacters.IndexOf(key[i]) + 27) % 27;
            decodedCharacters.Add(validCharacters[charIndex]);
        }
        string? decodedMessage = new string(decodedCharacters.ToArray());
        return decodedMessage == null ? string.Empty : decodedMessage;
    }

    // Test method to generate and return two encrypted messages from a randomly generated key 
    public (string, string, string) TestMethod()
    {
        string unencoded1 = "curiosity";
        string unencoded2 = "early bird";

        string key = string.Empty;
        Random random = new Random();
        int max = Math.Max(unencoded1.Length, unencoded2.Length);

        for (int i = 0; i < max; i++)
            key += validCharacters[random.Next(0, validCharacters.Count)];

        string encoded1 = Encoder(unencoded1, key);
        string encoded2 = Encoder(unencoded2, key);

        Console.WriteLine($"Key used: '{key}'");

        return (encoded1, encoded2, key);
    }

    public List<string> KeyFinder(string encrypted1, string encrypted2)
    {
        foreach (string word in words)
        {
            string possibleKey = string.Empty;
            string unencrypted1 = string.Empty;
            string unencrypted2 = string.Empty;

            if (word.Length > encrypted1.Length)
                continue;
            else if (word.Length < encrypted1.Length)
                unencrypted1 = word + " ";
            else if (word.Length == encrypted1.Length)
                unencrypted1 = word;

            for (int i = 0; i < unencrypted1.Length; i++)
            {
                if (i < possibleKey.Length)
                    continue;
                int charIndex = (validCharacters.IndexOf(encrypted1[i]) - validCharacters.IndexOf(unencrypted1[i]) + 27) % 27;

                possibleKey += validCharacters[charIndex];
            }

            unencrypted2 = Decoder(encrypted2, possibleKey);
            string[] unencrypted2Substring = unencrypted2.Split(' ');
            List<string> matches = new List<string>();

            int keyLength = Math.Max(encrypted1.Length, encrypted2.Length);

            if (unencrypted2Substring.Length == 1)
            {
                matches = PossibleMatchingWords(unencrypted2);
                if (matches.Count == 0)
                    continue;
                else if (unencrypted1.Length == encrypted1.Length && unencrypted2.Length == encrypted2.Length && possibleKey.Length == keyLength)
                    possibleKeys.Add(possibleKey);
                else
                    KeyFinderRecursive(encrypted2, encrypted1, unencrypted2, unencrypted1, possibleKey, matches);
            }
            else if (unencrypted2Substring.Length > 1 && unencrypted2Substring[^1] == string.Empty)
            {
                bool onlyValidWords = true;
                for(int i = 0; i < unencrypted2Substring.Length - 1; i++)
                {
                    if (!words.Contains(unencrypted2Substring[i]))
                        onlyValidWords = false;
                }
                if (onlyValidWords)
                {
                    if (unencrypted1.Length == encrypted1.Length && unencrypted2.Length == encrypted2.Length && possibleKey.Length == keyLength)
                        possibleKeys.Add(possibleKey);
                    else
                        KeyFinderRecursive(encrypted2, encrypted1, unencrypted2, unencrypted1, possibleKey, matches);
                }
                else
                    continue;
            }
            else if (unencrypted2Substring.Length > 1 && unencrypted2Substring[^1] != string.Empty)
            {
                matches = PossibleMatchingWords(unencrypted2Substring[^1]);
                bool onlyValidWords = matches.Count > 0;
                for (int i = 0; i < unencrypted2Substring.Length - 1; i++)
                {
                    if (!words.Contains(unencrypted2Substring[i]))
                        onlyValidWords = false;
                }
                if (onlyValidWords)
                {
                    if (unencrypted1.Length == encrypted1.Length && unencrypted2.Length == encrypted2.Length && possibleKey.Length == keyLength)
                        possibleKeys.Add(possibleKey);
                    else
                        KeyFinderRecursive(encrypted2, encrypted1, unencrypted2, unencrypted1, possibleKey, matches);
                }
                else
                    continue;
            }
        }
        return possibleKeys;
    }

    void KeyFinderRecursive(string encrypt1, string encrypt2, string unencrypt1, string unencrypt2, string Key, List<string> matchingWords)
    {
        string encrypted1 = encrypt1;
        string encrypted2 = encrypt2;
        string unencrypted1 = unencrypt1;
        string unencrypted2 = unencrypt2;
        string possibleKey = Key;
        List<string> words = matchingWords;

        foreach (string word in words)
        {
            if ((word.Length + unencrypted1.Length) > encrypted1.Length)
                continue;
            else if ((word.Length + unencrypted1.Length) < encrypted1.Length)
                unencrypted1 += word + " ";
            else if ((word.Length + unencrypted1.Length) == encrypted1.Length)
                unencrypted1 += word;

            for (int i = 0; i < unencrypted1.Length; i++)
            {
                if (i < possibleKey.Length)
                    continue;
                int charIndex = (validCharacters.IndexOf(encrypted1[i]) - validCharacters.IndexOf(unencrypted1[i]) + 27) % 27;

                possibleKey += validCharacters[charIndex];
            }
            unencrypted2 = Decoder(encrypted2, possibleKey);

            string[] unencrypted2Substring = unencrypted2.Split(' ');
            List<string> matches = new List<string>();

            int keyLength = Math.Max(encrypted1.Length, encrypted2.Length);

            if (unencrypted2Substring.Length == 1)
            {
                matches = PossibleMatchingWords(unencrypted2);
                if (matches.Count == 0)
                    continue;
                else if (unencrypted1.Length == encrypted1.Length && unencrypted2.Length == encrypted2.Length && possibleKey.Length == keyLength)
                    possibleKeys.Add(possibleKey);
                else
                    KeyFinderRecursive(encrypted2, encrypted1, unencrypted2, unencrypted1, possibleKey, matches);
            }
            else if (unencrypted2Substring.Length > 1 && unencrypted2Substring[^1] == string.Empty)
            {
                bool onlyValidWords = true;
                for (int i = 0; i < unencrypted2Substring.Length - 1; i++)
                {
                    if (!words.Contains(unencrypted2Substring[i]))
                        onlyValidWords = false;
                }
                if (onlyValidWords)
                {
                    if (unencrypted1.Length == encrypted1.Length && unencrypted2.Length == encrypted2.Length && possibleKey.Length == keyLength)
                        possibleKeys.Add(possibleKey);
                    else 
                        KeyFinderRecursive(encrypted2, encrypted1, unencrypted2, unencrypted1, possibleKey, matches);
                }
                else
                    continue;
                
            }
            else if (unencrypted2Substring.Length > 1 && unencrypted2Substring[^1] != string.Empty)
            {
                matches = PossibleMatchingWords(unencrypted2Substring[^1]);
                bool onlyValidWords = matches.Count > 0;
                for (int i = 0; i < unencrypted2Substring.Length - 1; i++)
                {
                    if (!words.Contains(unencrypted2Substring[i]))
                        onlyValidWords = false;
                }

                if (onlyValidWords)
                {
                    if (unencrypted1.Length == encrypted1.Length && unencrypted2.Length == encrypted2.Length && possibleKey.Length == keyLength)
                        possibleKeys.Add(possibleKey);
                    else 
                        KeyFinderRecursive(encrypted2, encrypted1, unencrypted2, unencrypted1, possibleKey, matches);
                }
                else
                    continue;
            }
        }
    }

    public List<string> PossibleMatchingWords(string wordFragment)
    {
        List<string> possibleMatches = new List<string>();

        foreach (string word in words)
        {
            if (word.Length < wordFragment.Length)
                continue;

            bool match = false;
            for (int i = 0; i < wordFragment.Length; i++)
            {
                if (word[i] != wordFragment[i])
                    break;
                else if (word[i] == wordFragment[i] && i == wordFragment.Length - 1)
                    match = true;
            }
            if (match is true)
                possibleMatches.Add(word.Substring(wordFragment.Length));
        }
        return possibleMatches;
    }
}