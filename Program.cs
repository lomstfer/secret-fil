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

    // debug random message. example -write 20000. 20000 random bytes as message
    Random random = new();
    byte[] testOutputBytes = new byte[int.Parse(args[1])];
    random.NextBytes(testOutputBytes);
    File.WriteAllText("message.txt", Encoding.ASCII.GetString(testOutputBytes));
    // --

    string messageStr = File.ReadAllText("message.txt");
    byte[] messageBytes = Encoding.ASCII.GetBytes(messageStr);
    if (messageBytes.Length * 8 > imgb.Bytes.Length) {
        Console.WriteLine("Message too long / image too small. No message was put in the image :(");
        return;
    }
    BitArray messageBits = new BitArray(messageBytes);

    imgb.Bits = ProcessImage(imgb.Bits, messageBits, messageBytes.Length);

    imgb.SaveFile("message_image.png");

    static BitArray ProcessImage(BitArray imageBits, BitArray messageBits, int messageBytesLength) {
        BitArray messageByteLengthBits = new BitArray(BitConverter.GetBytes(messageBytesLength));

        if ((messageBits.Length + messageByteLengthBits.Length) * 8 > imageBits.Length) {
            Console.WriteLine("Message too long / image too small. No message was put in the image :(");
            return imageBits;
        }

        int i = 0;
        for (int j = 0; j < messageByteLengthBits.Length; j++) {
            imageBits.Set(i, messageByteLengthBits.Get(j));

            i += 8;
        }

        for (int j = 0; j < messageBits.Length; j++) {
            imageBits.Set(i, messageBits.Get(j));

            i += 8/* imageBits.Length / messageBits.Length */;
        }

        return imageBits;
    }
}

else if (args[0] == "-read") {
    string filename = "message_image.png";
    ImageByter imgb = new(filename);

    Console.WriteLine(GetMessage(imgb.Bytes, imgb.Bits));
}

else {
    Console.WriteLine("Use -write or -read");
}

static string GetMessage(byte[] imageBytes, BitArray imageBits) {
    BitArray messageByteLengthBits = new(32);
    int j = 0;
    for (int i = 0; i < 32; i++) {
        messageByteLengthBits.Set(i, imageBits.Get(j));
        j += 8;
    }

    int messageByteLength = BitConverter.ToInt32(ImageByter.BitArrayToByteArray(messageByteLengthBits));

    BitArray output = new(messageByteLength * 8);

    for (int i = 0; i < output.Length; i += 1) {
        output.Set(i, imageBits.Get(j));
        j += 8/* imageBits.Length / output.Length */;
    }

    return Encoding.ASCII.GetString(ImageByter.BitArrayToByteArray(output));
}