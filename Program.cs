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

if (args[0] == "-write") {
    string filename = "image.png";
    ImageByter imgb = new(filename);

    // // debug random message. example: -write 20000. 20000 random bytes as message
    // Random random = new();
    // byte[] testOutputBytes = new byte[int.Parse(args[1])];
    // random.NextBytes(testOutputBytes);
    // //File.WriteAllText("message.txt", Encoding.ASCII.GetString(testOutputBytes));
    // // --
    // byte[] messageBytes = testOutputBytes;
    string messageStr = File.ReadAllText("message.txt");
    byte[] messageBytes = Encoding.ASCII.GetBytes(messageStr);

    if (messageBytes.Length > imgb.Bytes.Length / 8) {
        Console.WriteLine("Message too long / image too small. No message was put in the image :(");
        return;
    }
    BitArray messageBits = new BitArray(messageBytes);

    imgb.Bits = EmbedMessage(imgb.Bits, messageBits, messageBytes.Length);

    imgb.SaveFile("message_image.png");
}

else if (args[0] == "-read") {
    string filename = "message_image.png";
    ImageByter imgb = new(filename);

    Console.WriteLine(GetMessage(imgb.Bits));
}

else {
    Console.WriteLine("Use -write or -read");
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