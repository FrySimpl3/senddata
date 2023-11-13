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
        string filePath = "data.json";

        class data
        {
            public string Name { get; set; }
            public string Url { get; set; }
            public string webhook { get; set; }
        }
        private void SaveAndWriteToJsonFile<T>(T data, string filePath)
        {
            try
            {
                // Convert the data object to a JSON string
                string jsonString = JsonConvert.SerializeObject(data);

                // Create or overwrite the file and write the JSON string to it
                File.WriteAllText(filePath, jsonString);
            }
            catch (IOException ex)
            {
                Console.WriteLine($"Error saving and writing data to file: {ex.Message}");
            }
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
            if (System.IO.File.Exists(filePath))
            {
                data data = ReadFromJsonFile<data>(filePath);
                if (data != null)
                {
                    textBox1.Text = data.Name;
                    textBox2.Text = data.Url;
                    textBox3.Text = data.webhook;
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            save();
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
        private void save()
        {
            var data = new data
            {
                Name = textBox1.Text,
                Url = textBox2.Text,
                webhook = textBox3.Text
            };
            SaveAndWriteToJsonFile(data, filePath);
        }
        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            save();
        }
        private T ReadFromJsonFile<T>(string filePath)
        {
            try
            {
                if (File.Exists(filePath))
                {
                    // Read the JSON data from the file and deserialize it into an object
                    string jsonContent = File.ReadAllText(filePath);
                    T data = JsonConvert.DeserializeObject<T>(jsonContent);
                    return data;
                }
            }
            catch (IOException ex)
            {
                Console.WriteLine($"Error reading data from file: {ex.Message}");
            }

            return default; // Return the default value (null) if reading fails
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
           
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            
        }
    }
}
