while (true)
{
    Console.Write("Please write a message: ");
    string? message = Console.ReadLine();
    Console.Write("Please write a key (at least as long as the message): ");
    string? key = Console.ReadLine();

    Encryption encryption = new Encryption(message, key);
    string encoded = encryption.Encoder();
    Console.WriteLine(encoded);
}


public class Encryption
{
    public string Key { get; set; }
    public string Message { get; set; }

    private List<char> validCharacters = new List<char> { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l',
    'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z', ' ' };

    public Encryption(string message, string key)
    {
        Key = key;
        Message = message;
    }

    public string Encoder()
    {
        if (Message.Length > Key.Length && Message == null)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Key shorter than message or no message!");
            Console.ForegroundColor= ConsoleColor.White;
            return string.Empty;
        }

        List<char> encodedCharacters = new List<char>();

        for (int i = 0; i < Message.Length; i++)
        {
            int charIndex = (validCharacters.IndexOf(Message[i]) + validCharacters.IndexOf(Key[i])) % 26;
            encodedCharacters.Add(validCharacters[charIndex]);
        }
        string? encodedMessage = new string(encodedCharacters.ToArray());
        return encodedMessage == null ? string.Empty : encodedMessage;
    }
}




