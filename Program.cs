using System.Runtime.InteropServices;

if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) {
    Console.WriteLine("only windows is supported lmao");
    return;
}

// foreach (var a in args) {
//     Console.WriteLine(a);
// }

string filename = "image.jpg";
ImageByter imgb = new(filename);
imgb.Bytes = ProcessBytes(imgb.Bytes);
imgb.SaveFile("output.jpg");

static byte[] ProcessBytes(byte[] bytes) {
    for (int i = 0; i < bytes.Length; i++) {
        bytes[i] = (byte)(Math.Round((float)bytes[i] / 255) * 255);
        /*if (bytes[i] < 50)
            bytes[i] = 0;
        else
            bytes[i] = 255;*/
    }
    return bytes;
}