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

    Console.WriteLine("messageBytes.Length " + messageBytes.Length.ToString());
    Console.WriteLine("imgb.Bytes.Length " + imgb.Bytes.Length.ToString());

    if (messageBytes.Length > imgb.Bytes.Length) {
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

    Console.WriteLine("bits.Length " + bits.Length.ToString());
    Console.WriteLine("messageBits.Length " + messageBits.Length.ToString());
    Console.WriteLine("messageByteLengthBits.Length " + messageByteLengthBits.Length.ToString());

    if (messageBits.Length + messageByteLengthBits.Length > bits.Length) {
        Console.WriteLine("Message too long / image too small. No message was put in the image :(");
        return bits;
    }

    int fullFillCount = (messageBits.Length + messageByteLengthBits.Length) * 8 / bits.Length;

    int bitIndex = 0;
    int bitIndexRestartAt = 1;
    for (int i = 0; i < messageByteLengthBits.Length; i++) {
        if (bitIndex >= bits.Length) {
            bitIndex = bitIndexRestartAt;
            bitIndexRestartAt += 1;
        }

        bits.Set(bitIndex, messageByteLengthBits.Get(i));

        bitIndex += 8;
    }

    for (int i = 0; i < messageBits.Length; i++) {
        if (bitIndex >= bits.Length) {
            bitIndex = bitIndexRestartAt;
            bitIndexRestartAt += 1;
        }

        bits.Set(bitIndex, messageBits.Get(i));

        bitIndex += 8/* imageBits.Length / messageBits.Length */;
    }

    return bits;
}

static string GetMessage(BitArray bits) {
    BitArray messageByteLengthBits = new(32);
    int bitIndex = 0;
    int bitIndexRestartAt = 1;
    for (int i = 0; i < 32; i++) {
        if (bitIndex >= bits.Length) {
            bitIndex = bitIndexRestartAt;
            bitIndexRestartAt += 1;
        }

        messageByteLengthBits.Set(i, bits.Get(bitIndex));
        bitIndex += 8;
    }

    int messageByteLength = BitConverter.ToInt32(ImageByter.BitArrayToByteArray(messageByteLengthBits));

    BitArray output = new(messageByteLength * 8);

    for (int i = 0; i < output.Length; i += 1) {
        if (bitIndex >= bits.Length) {
            bitIndex = bitIndexRestartAt;
            bitIndexRestartAt += 1;
        }

        output.Set(i, bits.Get(bitIndex));
        bitIndex += 8/* bits.Length / output.Length */;
    }

    return Encoding.ASCII.GetString(ImageByter.BitArrayToByteArray(output));
}