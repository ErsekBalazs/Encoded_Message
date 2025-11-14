List<char> validCharacters = new List<char> { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l',
    'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z', ' ' };

while (true)
{
    Console.Write("Code or decode?: ");
    string? choice = Console.ReadLine();
    
    if(choice == "code")
    {
        Console.Write("Please write a message: ");
        string? message = Console.ReadLine();
        Console.Write("Please write a key (at least as long as the message): ");
        string? key = Console.ReadLine();
        if (message is null || key is null)
            continue;
        else if (!ValidityCheck(message) || !ValidityCheck(key))
            continue;

        string encoded = Encoder(message, key);
        Console.WriteLine($"Coded message: {encoded}");
    }
    else if (choice == "decode")
    {
        Console.Write("Coded message: ");
        string? coded = Console.ReadLine();
        Console.Write("Key: ");
        string? key = Console.ReadLine();
        if (coded is null || key is null)
            continue;
        else if (!ValidityCheck(coded) || !ValidityCheck(key))
            continue;

        string decoded = Decoder(coded, key);
        Console.WriteLine($"Decoded message: {decoded}");
    }
    else 
        continue;
}

bool ValidityCheck(string text)
{
    foreach (char c in text)
    {
        if (!validCharacters.Contains(c))
            return false;
    }
    return true;
}

string Encoder(string message, string key)
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

string Decoder(string encodedMessage,  string key)
{
    List<char> decodedCharacters = new List<char>();

    for (int i = 0; i < encodedMessage.Length; i++)
    {
        int charIndex = (validCharacters.IndexOf(encodedMessage[i]) - validCharacters.IndexOf(key[i]) + 27) % 27;
        decodedCharacters.Add(validCharacters[charIndex]);
    }
    string? decodedMessage = new string(decodedCharacters.ToArray());
    return decodedMessage == null ? string.Empty : decodedMessage;
}





