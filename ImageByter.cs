using System.Runtime.InteropServices;
using System.Drawing;
using System.Drawing.Imaging;
using System.Collections;

[System.Runtime.Versioning.SupportedOSPlatform("windows")]
public class ImageByter {
    private byte[] _bytes = new byte[0];

    public byte[] Bytes {
        get {
            return _bytes;
        }
        set {
            _bytes = value;
            _bits = new BitArray(value);
        }
    }

    private BitArray _bits;

    public BitArray Bits {
        get {
            return _bits;
        }
        set {
            _bits = value;
            _bytes = BitArrayToByteArray(value);
        }
    }

    private Bitmap inputImage;
    private IntPtr pointeur;
    private BitmapData bmpData;

    public ImageByter(string fileName) {
        FileStream bmpstream = new FileStream(fileName, FileMode.Open, FileAccess.Read);
        inputImage = new Bitmap(bmpstream);

        Rectangle rect = new Rectangle(0, 0, inputImage.Width, inputImage.Height);
        bmpData = inputImage.LockBits(rect, ImageLockMode.ReadWrite, inputImage.PixelFormat);
        pointeur = bmpData.Scan0;
        int bytesLength = Math.Abs(bmpData.Stride) * bmpData.Height;
        
        _bytes = new byte[bytesLength];
        _bits = new BitArray(Bytes);

        Marshal.Copy(pointeur, Bytes, 0, bytesLength);
    }

    public void SaveFile(string outputFileName) {
        if (inputImage == null || bmpData == null)
            return;

        Marshal.Copy(Bytes, 0, pointeur, Bytes.Length);

        inputImage.UnlockBits(bmpData);

        inputImage.Save(outputFileName);
    }

    static byte[] BitArrayToByteArray(BitArray bits)
    {
        var reversed = new BitArray(bits.Cast<bool>().Reverse().ToArray());
        byte[] ret = new byte[(reversed.Length - 1) / 8 + 1];
        reversed.CopyTo(ret, 0);
        return ret;
    }
}