using System.Runtime.InteropServices;
using System.Drawing;
using System.Drawing.Imaging;

if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) {
    Console.WriteLine("only windows is supported lmao");
    return;
}

string filename = "input2.jpg";

FileStream bmpstream = new FileStream(filename, FileMode.Open, FileAccess.Read);
Bitmap inputImage = new Bitmap(bmpstream);

Rectangle rect = new Rectangle(0, 0, inputImage.Width, inputImage.Height);
BitmapData bmpData = inputImage.LockBits(rect, ImageLockMode.ReadWrite, inputImage.PixelFormat);
IntPtr pointeur = bmpData.Scan0;
int bytesLength = Math.Abs(bmpData.Stride) * bmpData.Height;
byte[] bytes = new byte[bytesLength];
Marshal.Copy(pointeur, bytes, 0, bytesLength);

ProcessBytes(ref bytes);

// copy buffer back to image
Marshal.Copy(bytes, 0, pointeur, bytes.Length);

inputImage.UnlockBits(bmpData);

inputImage.Save("output.jpg");


static void ProcessBytes(ref byte[] bytes) {
    for (int i = 0; i < bytes.Length; i++) {
        if (bytes[i] < 50)
            bytes[i] = 0;
        else
            bytes[i] = 255;
    }
}