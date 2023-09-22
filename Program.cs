using System.Collections;
using System.Runtime.InteropServices;
using System.Text;

if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) {
    Console.WriteLine("only windows is supported lmao");
    return;
}

// foreach (var a in args) {
//     Console.WriteLine(a);
// }

string filename = "image.jpg";
ImageByter imgb = new(filename);

if (args[0] == "-write") {
    string messageStr = File.ReadAllText("message.txt");
    byte[] messageBytes = Encoding.ASCII.GetBytes(messageStr);
    bool[] messageBits = new BitArray(messageBytes).Cast<bool>().ToArray();

    imgb.Bits = ProcessImage(imgb.Bits, messageBits);

    imgb.SaveFile("output.jpg");

    static BitArray ProcessImage(BitArray imageBits, bool[] messageBits) {
        Console.WriteLine(messageBits.Length);
        for (int i = 0; i < messageBits.Length; i++) {
            imageBits.Set(i, messageBits[i]);
        }

        return imageBits;
    }
}
else if (args[0] == "-read") {
    Console.WriteLine("the message is:");
    Console.WriteLine(GetMessage(imgb.Bytes));

    static string GetMessage(byte[] imageBytes) {
        return BitConverter.ToString(imageBytes);
    }
}