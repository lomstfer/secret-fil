using System.Collections;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) {
    Console.WriteLine("only windows is supported lmao");
    return;
}

static void PrintHelp() {
    Console.WriteLine("Use -write [message file] [input image file] [output image file] to embedd a message. Example:\n\tsecret-fil -write message.txt image.png output.png");
    Console.WriteLine("");
    Console.WriteLine("Use -read [image file] to read a message. Example:\n\tsecret-fil -read image.png");
}

if (args.Length <= 1) {
    PrintHelp();
    return;
}

// -test img.png 25
if (args[0] == "-test") {
    if (args.Length < 3) {
        Console.WriteLine("no");
        return;
    }

    ImageByter imgb = new(args[1]);

    Random random = new();
    byte[] testOutputBytes = new byte[(int)(int.Parse(args[2])/100f*imgb.Bytes.Length)];
    random.NextBytes(testOutputBytes);
    byte[] messageBytes = testOutputBytes;

    if (messageBytes.Length > imgb.Bytes.Length / 8) {
        Console.WriteLine("Message too long / image too small.");
        return;
    }
    BitArray messageBits = new BitArray(messageBytes);

    imgb.Bits = EmbedMessage(imgb.Bits, messageBits, messageBytes.Length);

    imgb.SaveFile("testOutput.png");
}

if (args[0] == "-write") {
    if (args.Length <= 3) {
        PrintHelp();
        return;
    }
    
    if (!File.Exists(args[1])) {
        Console.WriteLine("Message file does not exist.");
        if (!File.Exists(args[2])) {
            Console.WriteLine("Image file does not exist.");
            return;
        }
        return;
    }
    if (!File.Exists(args[2])) {
        Console.WriteLine("Image file does not exist.");
        return;
    }

    string filename = args[2];
    ImageByter imgb = new(filename);

    string messageStr = File.ReadAllText(args[1]);
    byte[] messageBytes = Encoding.ASCII.GetBytes(messageStr);
    if (messageBytes.Length == 0) {
        Console.WriteLine("Message file was empty.");
        return;
    }

    if (messageBytes.Length > imgb.Bytes.Length / 8) {
        Console.WriteLine("Message too long / image too small.");
        return;
    }
    BitArray messageBits = new BitArray(messageBytes);

    imgb.Bits = EmbedMessage(imgb.Bits, messageBits, messageBytes.Length);

    imgb.SaveFile(args[3]);
}

else if (args[0] == "-read") {
    if (!File.Exists(args[1])) {
        Console.WriteLine("Image file does not exist.");
        return;
    }
    string filename = args[1];
    ImageByter imgb = new(filename);

    Console.WriteLine(GetMessage(imgb.Bits));
}

else {
    PrintHelp();
    return;
}

static BitArray EmbedMessage(BitArray bits, BitArray messageBits, int messageBytesLength) {
    BitArray messageByteLengthBits = new BitArray(BitConverter.GetBytes(messageBytesLength));

    if (messageBits.Length + messageByteLengthBits.Length > bits.Length / 8) {
        Console.WriteLine("Message too long / image too small. No message was put in the image :(");
        return bits;
    }
    
    // store messageLength
    int bitIndex = 0;
    for (int i = 0; i < messageByteLengthBits.Length; i++) {
        bits.Set(bitIndex, messageByteLengthBits.Get(i));
        bitIndex += 8;
    }

    // store message
    int spacing = (bits.Length - messageByteLengthBits.Length) / messageBits.Length;
    for (int i = 0; i < messageBits.Length; i++) {
        bits.Set(bitIndex, messageBits.Get(i));
        bitIndex += spacing;
    }
    return bits;
}

static string GetMessage(BitArray bits) {
    BitArray messageByteLengthBits = new(32);
    int bitIndex = 0;
    for (int i = 0; i < 32; i++) {
        messageByteLengthBits.Set(i, bits.Get(bitIndex));
        bitIndex += 8;
    }
    int messageByteLength = BitConverter.ToInt32(ImageByter.BitArrayToByteArray(messageByteLengthBits));

    int spacing = (bits.Length - 32) / (messageByteLength * 8);

    BitArray output = new(messageByteLength * 8);
    for (int i = 0; i < output.Length; i += 1) {
        output.Set(i, bits.Get(bitIndex));
        bitIndex += spacing;
    }

    return Encoding.ASCII.GetString(ImageByter.BitArrayToByteArray(output));
}