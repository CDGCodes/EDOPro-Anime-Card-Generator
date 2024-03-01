using System;
using System.Net;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Drawing;
using Newtonsoft.Json;

namespace CardDownloader {
    public class Misc
    {
        public int? question_atk { get; set; }
        public int? question_def { get; set; }
    }
    public class CardDB
    {
        public string? name { get; set; }
        public string? desc { get; set; }
        public string? type { get; set; }
        public string? frameType { get; set; }
        public string? race { get; set; }
        public string? attribute { get; set; }
        public int id { get; set; }
        public int? atk { get; set; }
        public int? def { get; set; }
        public int? level { get; set; }
        public int? scale { get; set; }
        public int? linkval { get; set; }
        public List<Misc> misc_info { get; set; }
        public List<string> linkmarkers { get; set; }
        public List<CardImages> card_images { get; set; }
    }
    public class CardImages
    {
        public int id { get; set; }
    }
    public class YGODB
    {
        public List<CardDB> data { get; set; }
    }
    class Program
    {
        public static string path = "./pics/";
        public static string fieldpath = "./pics/field/";
        public static string framepath = "./frames and icons/";
        public static string mcpath = "./raw/";
        public static string artworks = "https://images.ygoprodeck.com/images/cards_cropped/{0}.jpg";
        public static Image removeBlackBox (Image artwork)
        {
            if (((Bitmap)artwork).GetPixel(0, artwork.Height - 1) == Color.FromArgb(255,0,0,0) && ((Bitmap)artwork).GetPixel(artwork.Width/2, artwork.Height - 1) == Color.FromArgb(255, 0, 0, 0) && ((Bitmap)artwork).GetPixel(artwork.Width - 1, artwork.Height - 1) == Color.FromArgb(255, 0, 0, 0))
            {
                for (int i = 2; i < artwork.Height; i++)
                {
                    if (!(((Bitmap)artwork).GetPixel(0, artwork.Height-i) == Color.FromArgb(255, 0, 0, 0) || ((Bitmap)artwork).GetPixel(artwork.Width-1, artwork.Height-i) == Color.FromArgb(255, 0, 0, 0)))
                    {
                        Rectangle cropArea = new Rectangle(new Point(0, 0), new Size(artwork.Width, artwork.Height - i));
                        Image cropped = (Image)new Bitmap(cropArea.Width, cropArea.Height);
                        ((Bitmap)cropped).SetResolution(artwork.HorizontalResolution, artwork.VerticalResolution);
                        using (Graphics g = Graphics.FromImage(cropped))
                        {
                            g.DrawImage(artwork, 0, 0);
                        }
                        return cropped;
                    }
                }
            }
            return artwork;
        }
        public static bool validateImages(string filename, string type)
        {
            try
            {
                using (var bmp = new Bitmap(path + filename))
                {

                }
                if (type == "Field")
                {
                    using (var bmp = new Bitmap(fieldpath + filename))
                    {

                    }
                }
                return true;
            } catch (Exception ex)
            {
                return false;
            }
        }
        public static bool validateMcImages(string filename, string type)
        {
            try
            {
                using (var bmp = new Bitmap(mcpath + filename))
                {

                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public static void constructImage(CardDB card, bool saveMc, bool saveEdo)
        {
            using (WebClient webClient = new WebClient())
            {
                for (int art = 0; art < card.card_images.Count;)
                {
                    try
                    {
                        if (!validateImages(card.card_images[art].id + ".jpg", card.race)) {
                            byte[] img = webClient.DownloadData(String.Format(artworks, card.card_images[art].id));
                            Image artwork;
                            Image blank = (Image)new Bitmap(653, 941);
                            using (MemoryStream ms = new MemoryStream(img))
                            {
                                artwork = Image.FromStream(ms);
                                if (card.race == "Field" && saveEdo)
                                {
                                    artwork.Save(fieldpath + card.card_images[art].id + ".jpg");
                                }
                            }
                            if (card.type.Contains("Pendulum"))
                            {
                                artwork = removeBlackBox(artwork);
                            }
                            if (artwork.Width != 624)
                            {
                                double ratio = ((double)artwork.Width / 624);
                                artwork = (Image)new Bitmap((Bitmap)artwork, new Size(624, (int)(artwork.Height / ratio)));
                            }
                            if (artwork.Height < 664)
                            {
                                artwork = (Image)new Bitmap((Bitmap)artwork, new Size(624, 664));
                            }
                                ((Bitmap)artwork).SetResolution(96, 96);
                            Image frame;
                            if (card.name == "Obelisk the Tormentor" || card.name.Contains("Raviel, Lord of Phantasms"))
                            {
                                frame = Image.FromFile(framepath + "Obelisk the Tormentor.png");
                            }
                            else if (card.name == "Slifer the Sky Dragon" || card.name == "Uria, Lord of Searing Flames")
                            {
                                frame = Image.FromFile(framepath + "Slifer the Sky Dragon.png");
                            }
                            else if (card.name.Contains("The Winged Dragon of Ra"))
                            {
                                frame = Image.FromFile(framepath + "The Winged Dragon of Ra.png");
                            }
                            else if (card.name == "Hamon, Lord of Striking Thunder")
                            {
                                frame = Image.FromFile(framepath + "Hamon, Lord of Striking Thunder.png");
                            }
                            else if (card.name.StartsWith("The Wicked") && (card.name.Contains("Dreadroot") || card.name.Contains("Eraser") || card.name.Contains("Avatar")))
                            {
                                frame = Image.FromFile(framepath + "The Wicked Gods.png");
                            }
                            else if (card.name.StartsWith("Supreme King Z-ARC") || card.name == "Odd-Eyes Arcray Dragon")
                            {
                                frame = Image.FromFile(framepath + "Supreme King Z-Arc.png");
                            }
                            else if (card.name == "The Eye of Timaeus" || card.name == "The Fang of Critias" || card.name == "The Claw of Hermos" || card.name == "Legend of Heart")
                            {
                                frame = Image.FromFile(framepath + "Legendary Dragons.png");
                            }
                            else
                            {
                                frame = Image.FromFile(framepath + card.frameType + ".png");
                            }
                                ((Bitmap)blank).SetResolution(artwork.HorizontalResolution, artwork.VerticalResolution);
                            ((Bitmap)frame).SetResolution(artwork.HorizontalResolution, artwork.VerticalResolution);
                            Graphics merge = Graphics.FromImage(blank);
                            merge.DrawImage(artwork, 14, 15);
                            merge.DrawImage(frame, 0, 0);
                            if (!card.type.Contains("Pendulum") && card.desc.Contains("[ Pendulum Effect ]"))
                            {
                                Image missingPendulum = Image.FromFile(framepath + "Pendulum Frame Fix.png");
                                ((Bitmap)missingPendulum).SetResolution(artwork.HorizontalResolution, artwork.VerticalResolution);
                                merge.DrawImage(missingPendulum, 0, 0);
                            }
                            if (card.type.Contains("Monster") || card.type == "Token")
                            {
                                if (card.name == "Slifer the Sky Dragon" || card.name == "The Wicked Eraser")
                                {
                                    Font souvenir = new Font("Souvenir-Light", 65);
                                    centerAlignText(merge, "X000", souvenir, Color.Black, new Point(177, 815));
                                    centerAlignText(merge, "X000", souvenir, Color.Black, new Point(477, 815));
                                }
                                else if (card.type.Contains("Link"))
                                {
                                    Font souvenir = new Font("Souvenir-Light", 65);
                                    Font linkSouvenir = new Font("Souvenir-Light", 65, FontStyle.Bold);
                                    if (card.misc_info[0].question_atk == 1)
                                    {
                                        centerAlignText(merge, "?", souvenir, Color.Black, new Point(177, 813));
                                    }
                                    else
                                    {
                                        centerAlignText(merge, card.atk.ToString(), souvenir, Color.Black, new Point(177, 813));
                                    }
                                    centerAlignText(merge, card.linkval.ToString(), linkSouvenir, Color.Black, new Point(477, 813));
                                    for (int i = 0; i < card.linkmarkers.Count; i++)
                                    {
                                        Image marker = Image.FromFile(framepath + card.linkmarkers[i] + ".png");
                                        ((Bitmap)marker).SetResolution(artwork.HorizontalResolution, artwork.VerticalResolution);
                                        merge.DrawImage(marker, 0, 0);
                                    }
                                }
                                else if (!card.name.Contains("The Winged Dragon of Ra"))
                                {
                                    Font souvenir = new Font("Souvenir-Light", 65);
                                    if (card.misc_info[0].question_atk == 1)
                                    {
                                        centerAlignText(merge, "?", souvenir, Color.Black, new Point(177, 815));
                                    }
                                    else
                                    {
                                        centerAlignText(merge, card.atk.ToString(), souvenir, Color.Black, new Point(177, 815));
                                    }
                                    if (card.misc_info[0].question_def == 1)
                                    {
                                        centerAlignText(merge, "?", souvenir, Color.Black, new Point(477, 815));
                                    }
                                    else
                                    {
                                        centerAlignText(merge, card.def.ToString(), souvenir, Color.Black, new Point(477, 815));
                                    }
                                }
                                if (card.attribute != null)
                                {
                                    Image attribute = Image.FromFile(framepath + card.attribute + ".png");
                                    ((Bitmap)attribute).SetResolution(artwork.HorizontalResolution, artwork.VerticalResolution);
                                    merge.DrawImage(attribute, 0, 0);
                                }
                                if (card.type.Contains("XYZ"))
                                {
                                    Image rank = Image.FromFile(framepath + "Rank.png");
                                    ((Bitmap)rank).SetResolution(artwork.HorizontalResolution, artwork.VerticalResolution);
                                    for (int i = 0; i < card.level; i++)
                                    {
                                        merge.DrawImage(rank, 59 + (37 * i), 727);
                                    }
                                }
                                else
                                {
                                    Image level = Image.FromFile(framepath + "Level.png");
                                    ((Bitmap)level).SetResolution(artwork.HorizontalResolution, artwork.VerticalResolution);
                                    for (int i = 0; i < card.level; i++)
                                    {
                                        double offset = (double)i / 2;
                                        offset = Math.Ceiling(offset);
                                        if (i % 2 == 0)
                                        {
                                            merge.DrawImage(level, (int)(285 + (37 * offset)), 727);
                                        }
                                        else
                                        {
                                            merge.DrawImage(level, (int)(285 - (37 * offset)), 727);
                                        }
                                    }
                                }
                                if (card.type.Contains("Pendulum") || card.desc.Contains("[ Pendulum Effect ]"))
                                {
                                    Font souvenir = new Font("Souvenir-Light", 40, FontStyle.Bold);
                                    centerAlignText(merge, card.scale.ToString(), souvenir, Color.Black, new Point(48, 633));
                                    centerAlignText(merge, card.scale.ToString(), souvenir, Color.Black, new Point(604, 633));
                                }
                            }
                            if (saveEdo)
                            {
                                blank.Save(path + card.card_images[art].id + ".jpg");
                                if (validateImages(card.card_images[art].id + ".jpg", card.type))
                                {
                                    if (card.card_images.Count > 1)
                                    {
                                        Console.WriteLine(card.name + " #" + (art + 1) + " has been downloaded");
                                    }
                                    else
                                    {
                                        Console.WriteLine(card.name + " has been downloaded");
                                    }
                                    if (!saveMc) { art++; }
                                }
                                else
                                {
                                    if (card.card_images.Count > 1)
                                    {
                                        Console.WriteLine(card.name + " #" + (art + 1) + " failed to download. Retrying...");
                                    }
                                    else
                                    {
                                        Console.WriteLine(card.name + " failed to download. Retrying...");
                                    }
                                }
                            }
                            if (saveMc)
                            {
                                blank.Save(mcpath + card.card_images[art].id + "_0.jpg");
                                if (validateMcImages(card.card_images[art].id + "_0.jpg", card.type))
                                {
                                    if (card.card_images.Count > 1)
                                    {
                                        Console.WriteLine(card.name + " #" + (art + 1) + " has been downloaded for Minecraft");
                                    }
                                    else
                                    {
                                        Console.WriteLine(card.name + " has been downloaded for Minecraft");
                                    }
                                    art++;
                                }
                                else
                                {
                                    if (card.card_images.Count > 1)
                                    {
                                        Console.WriteLine(card.name + " #" + (art + 1) + " failed to download for Minecraft. Retrying...");
                                    }
                                    else
                                    {
                                        Console.WriteLine(card.name + " failed to download for Minecraft. Retrying...");
                                    }
                                }
                            }
                        }
                        else
                        {
                            art++;
                            if (card.card_images.Count > 1)
                            {
                                Console.WriteLine(card.name + " #" + (art + 1) + " has already been downloaded");
                            }
                            else
                            {
                                Console.WriteLine(card.name + " has already been downloaded");
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        art++;
                        Console.WriteLine(card.name + " failed: " + e);
                    }
                }
            }
        }
        public static void centerAlignText(Graphics g, String text, Font font, Color color, Point point)
        {
            Brush textBrush = new SolidBrush(color);       
            SizeF textSize = g.MeasureString(text, font);
            point.X -= (int)textSize.Width / 2;
            g.DrawString(text, font, textBrush, point);
        }
        static void Main(string[] args) {

            string YGODB;
            YGODB = "https://db.ygoprodeck.com/api/v7/cardinfo.php?misc=yes";

            WebRequest getDB;
            getDB = WebRequest.Create(YGODB);

            Stream DBStream;
            DBStream = getDB.GetResponse().GetResponseStream();

            StreamReader DBReader = new StreamReader(DBStream);

            string sLine = DBReader.ReadLine();
            //Console.WriteLine(sLine);
            YGODB DB = JsonConvert.DeserializeObject<YGODB>(sLine);
            Console.WriteLine("Do you want to save your images for Minecraft? Y/N");
            string mcresponse = Console.ReadLine();
            while (mcresponse != "Y" && mcresponse != "N" && mcresponse != "y" && mcresponse != "n")
            {
                Console.WriteLine("Invalid input, please try again");
                Console.WriteLine("Do you want to save your images for Minecraft? Y/N");
                mcresponse = Console.ReadLine();
            }
            bool saveMc = (mcresponse == "Y" || mcresponse == "y");
            bool saveEDO;
            if (saveMc)
            {
                Console.WriteLine("Do you want to save your images for EDOPro as well? Y/N");
                string edoresponse = Console.ReadLine();
                while (edoresponse != "Y" && edoresponse != "N" && edoresponse != "y" && edoresponse != "n")
                {
                    Console.WriteLine("Invalid input, please try again");
                    Console.WriteLine("Do you want to save your images for EDOPro as well? Y/N");
                    edoresponse = Console.ReadLine();
                }
                saveEDO = (edoresponse == "Y" || edoresponse == "y");
            }
            else
            {
                saveEDO = true;
            }
            if (saveEDO)
            {
                if (!Directory.Exists(path))
                {
                    DirectoryInfo di = Directory.CreateDirectory(path);
                }
                if (!Directory.Exists(fieldpath))
                {
                    DirectoryInfo di = Directory.CreateDirectory(fieldpath);
                }
            }
            if (saveMc)
            {
                if (!Directory.Exists(mcpath))
                {
                    DirectoryInfo di = Directory.CreateDirectory(mcpath);
                }
            }

            for (int i = 0; i < DB.data.Count; i++)
            {
/*                if (DB.data[i].race == "Field" && !File.Exists(fieldpath + DB.data[i].id + ".jpg"))
                {
                    using (WebClient webClient = new WebClient())
                    {
                        byte[] img = webClient.DownloadData(String.Format(artworks, DB.data[i].id));
                        File.WriteAllBytes(fieldpath + DB.data[i].id + ".jpg", img);
                    }
                }*/
#pragma warning disable CA1416 // Validate platform compatibility
                constructImage(DB.data[i], saveMc, saveEDO);
            }       
#pragma warning restore CA1416 // Validate platform compatibility
        }
    }
}