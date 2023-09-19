using System.Drawing;
using System.Drawing.Imaging;

//convert bytearray to image
System.Drawing.Image ByteArrayToImage(byte[] byteArrayIn)
{
   using (MemoryStream mStream = new MemoryStream(byteArrayIn))
   {
      return Image.FromStream(mStream);
   }
}

//another easy way to convert image to bytearray
byte[] ImgToByteConverter(Image inImg)
{
   ImageConverter imgCon = new ImageConverter();
   return (byte[])imgCon.ConvertTo(inImg, typeof(byte[]));
}

Image image = Image.FromFile("image.jpg");
byte[] bytes = ImgToByteConverter(image);

// for (int i = 0; i < bytes.Length; i++) {
//     Console.Write(bytes[i].ToString() + " ");
// }

ByteArrayToImage(bytes).Save("output.jpg", ImageFormat.Jpeg);