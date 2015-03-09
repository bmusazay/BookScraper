using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Collections;
using System.Text.RegularExpressions;
using System.Net;
using System.IO;
using System.Drawing.Imaging;

namespace BooksScraper
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            int scrapeCount = 0;

            ArrayList bookUrls = new ArrayList();
            HttpSession session = new HttpSession();
            session.setUserAgent();
            session.getPage("http://www.barnesandnoble.com/s/?pro=1886&startat=&store=book&view=grid");

            string pattern = "<a href=\"(.*?)\"  hidefocus=\"true\"";
            MatchCollection matches = Regex.Matches(session.html, pattern);
            if (matches.Count > 0)
                foreach (Match m in matches)
                    bookUrls.Add( m.Groups[1].ToString() );

            foreach (String url in bookUrls)
            {
                session.getPage(url);
                String category = textBox1.Text;
                String isbn = session.extractFromHtml("ISBN-13:</span> ", "\n                        </li>\n                    \n                                <li>", 16);
                String title = session.extractFromHtml("'product title' : '", "',\n'store", 19).Replace("\\", "");
                String price = session.extractFromHtml("'unit price' : '$", "'\n});\n}\n", 17);
                String author = session.extractFromHtml("Contributor_1\" class=\"subtle\">", "</a>\n            </li>\n        \n     </ul>", 30);
                Random random = new Random();
                String year = random.Next(1995, 2015).ToString();
                String inventory = random.Next(300, 2500).ToString();
                String imageUrl = session.extractFromHtml("data-bn-src-url=\"", "\">\n                        </li>", 17);
                Image coverPicture = getImage(imageUrl, isbn);
                saveToTextFile("BookData", isbn + "|" + title + "|" + inventory + "|" + price + "|" + category + "|" + author + "|" + year);
                scrapeCount++;
                label1.Text = scrapeCount.ToString();
            }
        }


        private static void saveToTextFile(string filename, string toSave)
        {
            filename += ".txt";
            System.IO.StreamWriter file = new System.IO.StreamWriter(filename, true);
            file.WriteLine(toSave);
            file.Close();
        }

        public Image getImage(String url, string isbn)
        {
            WebResponse result = null;
            Image rImage = null;
            try
            {
                WebRequest request = WebRequest.Create(url);
                byte[] rBytes;
                result = request.GetResponse();
                Stream rStream = result.GetResponseStream();
                using (BinaryReader br = new BinaryReader(rStream))
                {
                    rBytes = br.ReadBytes(10000000);
                    br.Close();
                }
                result.Close();
                using (MemoryStream imageStream = new MemoryStream(rBytes, 0, rBytes.Length))
                {
                    imageStream.Write(rBytes, 0, rBytes.Length);
                    rImage = Image.FromStream(imageStream, true);
                    rImage.Save(isbn + ".png", ImageFormat.Png);
                    imageStream.Close();
                }
            }
            catch { }
            finally
            {
                if (result != null) result.Close();
            }

            return rImage;
        }
    }
}
