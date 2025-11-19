Encrypt encrypt = new();
string message = "hello";
string key = "abcdefg";
string encrypted = encrypt.Encoder(message, key);
string unencrypted = encrypt.Decoder(encrypted, key);

public class Encrypt
{
    private List<char> validCharacters = new List<char> { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l',
             'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z', ' ' };

    private List<string> words = File.ReadAllLines("words.txt").ToList();
    private List<string> possibleKeys = new List<string>();

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
    (string, string) TestMethod()
    {
        string unencoded1 = "curiosity killed the cat";
        string unencoded2 = "early bird catches the worm";

        string key = string.Empty;
        Random random = new Random();

        for (int i = 0; i <= unencoded2.Length; i++)
            key += validCharacters[random.Next(0, validCharacters.Count)];

        string encoded1 = Encoder(unencoded1, key);
        string encoded2 = Encoder(unencoded2, key);

        Console.WriteLine($"Key used: {key}");

        return (encoded1, encoded2);
    }

    public List<string> KeyFinder(string encrypted1, string encrypted2)
    {
        foreach (string word in words)
        {
            List<char> possibleKeyChars = new List<char>();
            List<char> unencrypted1Chars = new List<char>();
            List<char> unencrypted2Chars = new List<char>();

            if (word.Length > encrypted1.Length)
                continue;
            else if (word.Length < encrypted1.Length)
                unencrypted1Chars.AddRange(word + " ");
            else if (word.Length == encrypted1.Length)
                unencrypted1Chars.AddRange(word);

            for (int i = 0; i < unencrypted1Chars.Count; i++)
            {
                if (i < possibleKeyChars.Count)
                    continue;
                int charIndex = (validCharacters.IndexOf(encrypted1[i]) - validCharacters.IndexOf(unencrypted1Chars[i]) + 27) % 27;

                possibleKeyChars.Add(validCharacters[charIndex]);
            }
            string possibleKey = string.Concat(possibleKeyChars);

            string unencrypted2 = Decoder(encrypted2, possibleKey);
            if (word == "curiosity")
                Console.WriteLine($"'{word}': '{possibleKey}'; '{unencrypted2}'");

            List<string> possibleMatches = PossibleMatchingWords(unencrypted2);
            unencrypted2Chars.Clear();

            if (possibleMatches.Count == 0)
                continue;
            else if (unencrypted1Chars.Count < encrypted1.Length || unencrypted2Chars.Count < encrypted2.Length)
            {
                Console.WriteLine("calling recursive");
                KeyFinderRecursive(encrypted2, encrypted1, unencrypted2Chars, unencrypted1Chars, possibleKeyChars, possibleMatches);
            }
            else
                possibleKeys.Add(possibleKey);
        }
        return possibleKeys;
    }

    void KeyFinderRecursive(string encrypted1, string encrypted2, List<char> unencrypted1Chars, List<char> unencrypted2Chars, List<char> possibleKeyChars, List<string> matchingWords)
    {
        foreach (string word in matchingWords)
        {
            int initialLength = unencrypted1Chars.Count;
            int unencrypted2Length = unencrypted2Chars.Count;

            if (word.Length > encrypted1.Length)
                continue;
            else if (word.Length < encrypted1.Length)
                unencrypted1Chars.AddRange(word + " ");
            else if (word.Length == encrypted1.Length)
                unencrypted1Chars.AddRange(word);

            for (int i = 0; i < unencrypted1Chars.Count; i++)
            {
                if (i < possibleKeyChars.Count)
                    continue;
                int charIndex = (validCharacters.IndexOf(encrypted1[i]) - validCharacters.IndexOf(unencrypted1Chars[i]) + 27) % 27;

                possibleKeyChars.Add(validCharacters[charIndex]);
            }
            string possibleKey = string.Concat(possibleKeyChars);

            string unencrypted2 = Decoder(encrypted2, possibleKey);


            List<string> possibleMatches = PossibleMatchingWords(unencrypted2.Substring(unencrypted2Length, unencrypted2Chars.Count - unencrypted2Length));
            if (possibleMatches.Count == 0)
            {
                unencrypted1Chars.RemoveAll(c => unencrypted1Chars.IndexOf(c) >= initialLength);
                continue;
            }
            else if (unencrypted1Chars.Count < encrypted1.Length || unencrypted2Chars.Count < encrypted2.Length)
                KeyFinderRecursive(encrypted2, encrypted1, unencrypted2Chars, unencrypted1Chars, possibleKeyChars, possibleMatches);
            else
                possibleKeys.Add(possibleKey);
        }
    }

    List<string> PossibleMatchingWords(string wordFragment)
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
                possibleMatches.Add(word);
        }
        return possibleMatches;
    }
}