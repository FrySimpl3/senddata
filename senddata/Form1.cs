using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace senddata
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (Clipboard.ContainsImage())
            {
                System.Drawing.Image image = Clipboard.GetImage();
                if (image != null)
                {
                    pictureBox1.Image = image;
                }
            }
        }

        private async void button2_Click(object sender, EventArgs e)
        {
            string webhookUrl = textBox3.Text; // Thay YOUR_WEBHOOK_URL bằng URL của Webhook Discord của bạn

            if (pictureBox1.Image != null)
            {
                string webhookName = textBox1.Text; // Tên Webhook được nhập bởi người dùng
                string avatarUrl = textBox2.Text; // Đường dẫn đến biểu tượng (avatar) được nhập bởi người dùng

                using (HttpClient client = new HttpClient())
                {
                    using (MultipartFormDataContent content = new MultipartFormDataContent())
                    {
                        // Lưu ảnh từ PictureBox vào một MemoryStream
                        MemoryStream imageStream = new MemoryStream();
                        pictureBox1.Image.Save(imageStream, System.Drawing.Imaging.ImageFormat.Png);
                        ByteArrayContent imageContent = new ByteArrayContent(imageStream.ToArray());
                        imageContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
                        {
                            Name = "file",
                            FileName = "image.png",
                        };
                        content.Add(imageContent);

                        // Tạo đối tượng JSON chứa tên và biểu tượng
                        var webhookData = new
                        {
                            name = webhookName,
                            avatar_url = avatarUrl
                        };

                        string payload = JsonConvert.SerializeObject(webhookData);

                        StringContent jsonContent = new StringContent(payload);
                        jsonContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                        content.Add(jsonContent, "payload_json");

                        // Gửi POST request với hình ảnh đính kèm và JSON payload
                        HttpResponseMessage response = await client.PostAsync(webhookUrl, content);

                        // Xem xét kết quả
                        if (response.IsSuccessStatusCode)
                        {
                            MessageBox.Show("Hình ảnh đã được gửi thành công.");
                        }
                        else
                        {
                            MessageBox.Show("Có lỗi xảy ra khi gửi hình ảnh.");
                        }
                    }
                }
            }
        }
    }
}
