using System.Drawing;

byte[] ImageToByteArray(System.Drawing.Image imageIn)
{
   using (var ms = new MemoryStream())
   {
      imageIn.Save(ms,imageIn.RawFormat);
      return  ms.ToArray();
   }
}

Image image = Image.FromFile("image.jpg");
byte[] bytes = ImageToByteArray(image);

for (int i = 0; i < bytes.Length; i++) {
    Console.Write(bytes[i].ToString() + " ");
}
