using System.Collections;
using System.Runtime.InteropServices;
using System.Text;

if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) {
    Console.WriteLine("only windows is supported lmao");
    return;
}

/* if (args[0] == "-write") */ {
    string filename = "image.jpg";
    ImageByter imgb = new(filename);

    string messageStr = File.ReadAllText("message.txt");
    byte[] messageBytes = Encoding.ASCII.GetBytes(messageStr);
    BitArray messageBits = new BitArray(messageBytes);

    imgb.Bits = ProcessImage(imgb.Bits, messageBits);

    Console.WriteLine(GetMessage(imgb.Bytes));

    static string GetMessage(byte[] imageBytes) {
        byte[] bt = new byte[100];
        for (int i = 0; i < bt.Length; i++) {
            bt[i] = imageBytes[i];
        }
        Console.WriteLine(bt[0]);
        return Encoding.ASCII.GetString(bt);
    }
    /* int i = 0;
    foreach (bool b in imgb.Bits) {
        Console.Write(b ? 1 : 0);
        i++;
        if (i > 100) break;
    } */

    imgb.SaveFile("message_image.jpg");

    static BitArray ProcessImage(BitArray imageBits, BitArray messageBits) {
        for (int i = 0; i < messageBits.Count; i++) {
            imageBits.Set(i, messageBits.Get(i));
        }
        return imageBits;
    }
}

Console.WriteLine("\n-------\n");

/* else if (args[0] == "-read") */ {
    string filename = "message_image.jpg";
    ImageByter imgb = new(filename);

    Console.WriteLine("\nthe message is:");
    Console.WriteLine(GetMessage(imgb.Bytes));

    static string GetMessage(byte[] imageBytes) {
        byte[] bt = new byte[100];
        for (int i = 0; i < bt.Length; i++) {
            bt[i] = imageBytes[i];
        }
        Console.WriteLine(bt[0]);
        return Encoding.ASCII.GetString(bt);
    }
}