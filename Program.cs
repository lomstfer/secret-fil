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

    // // debug random message. example -write 20000. 20000 random bytes as message
    // Random random = new();
    // byte[] testOutputBytes = new byte[int.Parse(args[1])];
    // random.NextBytes(testOutputBytes);
    // File.WriteAllText("message.txt", Encoding.ASCII.GetString(testOutputBytes));
    // // --

    string messageStr = File.ReadAllText("message.txt");
    byte[] messageBytes = Encoding.ASCII.GetBytes(messageStr);
    if (messageBytes.Length > imgb.Bytes.Length) {
        Console.WriteLine("Message too long / image too small. No message was put in the image :(");
        return;
    }
    BitArray messageBits = new BitArray(messageBytes);

    imgb.Bits = ProcessImage(imgb.Bits, messageBits, messageBytes.Length);

    imgb.SaveFile("message_image.png");

    static BitArray ProcessImage(BitArray imageBits, BitArray messageBits, int messageBytesLength) {
        BitArray messageByteLengthBits = new BitArray(BitConverter.GetBytes(messageBytesLength));

        if (messageBits.Length + messageByteLengthBits.Length > imageBits.Length) {
            Console.WriteLine("Message too long / image too small. No message was put in the image :(");
            return imageBits;
        }

        int i = 0;
        while (i < messageByteLengthBits.Length) {
            imageBits.Set(i, messageByteLengthBits.Get(i));

            i += 1;
        }

        int j = 0;
        while (j < messageBits.Length) {
            imageBits.Set(i, messageBits.Get(j));

            i += imageBits.Length / messageBits.Length;
            j += 1;
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
    // how many bytes the message is
    int messageByteLength = BitConverter.ToInt32(new byte[]{imageBytes[0], imageBytes[1], imageBytes[2], imageBytes[3]});

    BitArray output = new(messageByteLength * 8);

    int j = 32; // skipping the messageByteLength bite
    for (int i = 0; i < output.Length; i += 1) {
        output.Set(i, imageBits.Get(j));
        j += imageBits.Length / output.Length;
    }

    return Encoding.ASCII.GetString(ImageByter.BitArrayToByteArray(output));
}