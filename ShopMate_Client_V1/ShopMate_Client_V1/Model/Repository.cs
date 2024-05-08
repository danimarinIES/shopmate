﻿using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace ShopMate_Client_V1.Model
{

    public class Repository : HttpResponseMessage
    {

        private static string apiKey = "c11cddd5b1554b78b6532b41287bd243";
        string ws1 = $"http://172.16.24.21:6385/api/";
        static List<User> usersList;
        static List<Category> categoryList;
        static List<Item> itemList;


        public static object MakeRequest(string requestUrl, object JSONRequest, string JSONmethod, string JSONContentType, Type JSONResponseType)
        {
            try
            {
                HttpWebRequest request = WebRequest.Create(requestUrl) as HttpWebRequest;
                request.Headers["x-api-key"] = apiKey;
                string sb = JsonConvert.SerializeObject(JSONRequest);
                request.Method = JSONmethod;

                if (JSONmethod != "GET")
                {
                    request.ContentType = JSONContentType;
                    Byte[] bt = Encoding.UTF8.GetBytes(sb);
                    Stream st = request.GetRequestStream();
                    st.Write(bt, 0, bt.Length);
                    st.Close();
                }

                using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                {
                    if (response.StatusCode != HttpStatusCode.OK)
                        throw new Exception(String.Format("Server error (HTTP {0}: {1}).", response.StatusCode, response.StatusDescription));

                    Stream stream1 = response.GetResponseStream();
                    StreamReader sr = new StreamReader(stream1);
                    string strsb = sr.ReadToEnd();
                    object objResponse = JsonConvert.DeserializeObject(strsb, JSONResponseType);
                    return objResponse;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }

        public HttpResponseMessage PostBuffer(Uri uri, HttpContent content)
        {
            try
            {
                // Crear cliente HttpClient
                using (var client = new HttpClient())
                {
                    // Establecer encabezados y otros parámetros necesarios
                    client.DefaultRequestHeaders.Add("x-api-key", apiKey);

                    // Realizar la solicitud HTTP POST y devolver la respuesta
                    var response = client.PostAsync(uri, content).Result;
                    return response;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error en la solicitud HTTP POST: " + e.Message);
                return null;
            }
        }

        // USER
        public List<User> GetUsers()
        {
            usersList = (List<User>)MakeRequest(ws1 + "User", null, "GET", "application/json", typeof(List<User>));
            return usersList;
        }
        public User GetUserbyUserName(String username)
        {

            return (User)MakeRequest(ws1 + "User/" + username, null, "GET", "application/json", typeof(List<User>));
        }
        public User PostUser(User user)
        {

            User user1 = new User();
            user1 = user;
            try
            {
                // Serializar el objeto User a JSON
                // string jsonUser = JsonConvert.SerializeObject(user1);

                // Mostrar el formato JSON en un MessageBox
                // MessageBox.Show("Formato JSON del objeto User:\n" + jsonUser);
                return (User)MakeRequest(string.Concat(ws1 + "User"), user1, "POST", "application/json", typeof(User));
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }


        public User PostUserWithImage(User user, IFormFile image)
        {
            try
            {
                var uri = new Uri(ws1 + "User/profileImage");
                var content = new MultipartFormDataContent();

                // Agregar los datos del usuario como contenido JSON
                string userJson = JsonConvert.SerializeObject(user);
                content.Add(new StringContent(userJson), "user");

                // Agregar la imagen como contenido multimedia
                if (image != null && image.Length > 0)
                {
                    using (var stream = image.OpenReadStream())
                    {
                        var fileName = Path.GetFileName(image.FileName);
                        content.Add(new StreamContent(stream), "media", fileName);
                    }
                }

                // Realizar la solicitud HTTP POST utilizando PostBuffer
                var response = PostBuffer(uri, content);
                response.EnsureSuccessStatusCode();

                // Devolver el usuario creado desde la respuesta del servidor
                string jsonResponse = response.Content.ReadAsStringAsync().Result;
                User newUser = JsonConvert.DeserializeObject<User>(jsonResponse);
                return newUser;
            }
            catch (Exception e)
            {
                Console.WriteLine("Error al publicar el usuario con imagen: " + e.Message);
                return null;
            }
        }



        //public User PostUserWithImage(User user, string imagePath)
        //{
        //    try
        //    {
        //        string requestUrl = $"{ws1}User";

        //        // Construir los datos del formulario
        //        MultipartFormDataContent formData = new MultipartFormDataContent();

        //        // Convertir el objeto User a JSON y agregarlo como texto plano
        //        string userJson = JsonConvert.SerializeObject(user);
        //        StringContent userContent = new StringContent(userJson, Encoding.UTF8, "application/json");
        //        formData.Add(userContent, "user");

        //        // Cargar la imagen desde el archivo y agregarla como archivo adjunto
        //        byte[] imageBytes = File.ReadAllBytes(imagePath);
        //        ByteArrayContent imageContent = new ByteArrayContent(imageBytes);
        //        imageContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("image/jpeg");
        //        formData.Add(imageContent, "profileImage", "profile.jpg");

        //        // Crear la solicitud HTTP POST
        //        HttpClient client = new HttpClient();
        //        client.DefaultRequestHeaders.Add("x-api-key", apiKey);

        //        // Enviar la solicitud y recibir la respuesta
        //        HttpResponseMessage response = client.PostAsync(requestUrl, formData).Result;

        //        // Procesar la respuesta
        //        if (response.IsSuccessStatusCode)
        //        {
        //            string responseContent = response.Content.ReadAsStringAsync().Result;
        //            return JsonConvert.DeserializeObject<User>(responseContent);
        //        }
        //        else
        //        {
        //            Console.WriteLine($"Error: {response.StatusCode}");
        //            return null;
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        Console.WriteLine(e.Message);
        //        return null;
        //    }
        //}


        public void DeleteUserById(int userId)
        {

            MakeRequest(ws1 + "User/" + userId, null, "DELETE", "application/json", typeof(void));
        }
        public User PutUser(User user, uint userId, string newName, string newUsername, string newPhone, string newEmail)
        {
            try
            {
                user.Name = newName;
                user.Username = newUsername;
                user.PhoneNumber = newPhone;
                user.Email = newEmail;

                string requestUrl = $"{ws1}User/{userId}";

                return (User)MakeRequest(requestUrl, user, "PUT", "application/json", typeof(User));


            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }



        // CATEGORY
        public List<Category> GetCategories()
        {
            categoryList = (List<Category>)MakeRequest(ws1 + "Category", null, "GET", "application/json", typeof(List<Category>));
            return categoryList;
        }
        public Category PostCategory(Category category)
        {
            try
            {
                // string jsonUser = JsonConvert.SerializeObject(category);
                // MessageBox.Show("Formato JSON del objeto User:\n" + jsonUser);
                return (Category)MakeRequest(string.Concat(ws1 + "Category"), category, "POST", "application/json", typeof(Category));
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }
        public void DeleteCategoriesById(int categoryId)
        {

            // MessageBox.Show(userId + "h");
            MakeRequest(ws1 + "Category/" + categoryId, null, "DELETE", "application/json", typeof(void));
        }
        public Category PutCategory(Category categorySelected, uint categoryId, string newName)
        {
            try
            {
                categorySelected.Name = newName;


                string requestUrl = $"{ws1}Category/{categoryId}";

                return (Category)MakeRequest(requestUrl, categorySelected, "PUT", "application/json", typeof(Category));


            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }



        // ITEMS
        public List<Item> GetItems()
        {
            itemList = (List<Item>)MakeRequest(ws1 + "Item", null, "GET", "application/json", typeof(List<Item>));
            return itemList;
        }
        public Item PostItem(Item item)
        {
            try            
            {
                string jsonItem = JsonConvert.SerializeObject(item);
                MessageBox.Show("Formato JSON del objeto Item:\n" + jsonItem);
                return (Item)MakeRequest(string.Concat(ws1 + "Item"), item, "POST", "application/json", typeof(Item));
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }

        // IMAGE
        public Image GetImageLocal(string imageUrl)
        {
            try
            {
                string requestUrl = $"http://172.16.24.21/api/user/images/{imageUrl}";
                string extension = Path.GetExtension(imageUrl).ToLower();


                string contentType = "";
                switch (extension)
                {
                    case ".jpeg":
                    case ".jpg":
                        contentType = "image/jpeg";
                        break;
                    case ".png":
                        contentType = "image/png";
                        break;
                    default:
                        throw new NotSupportedException($"Extension {extension} not supported.");
                }


                byte[] imageData;
                using (WebClient webClient = new WebClient())
                {
                    webClient.Headers.Add("Content-Type", contentType);
                    imageData = webClient.DownloadData(requestUrl);
                }


                using (MemoryStream ms = new MemoryStream(imageData))
                {
                    return Image.FromStream(ms);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }

        public Image GetGoogleImage(User u)
        {
            try
            {
                // Usar WebClient para descargar la imagen desde la URL
                using (WebClient webClient = new WebClient())
                {
                    byte[] imageData = webClient.DownloadData(u.ProfileImage);

                    // Convertir los datos de la imagen descargada a un objeto Image
                    using (var stream = new System.IO.MemoryStream(imageData))
                    {
                        Image image = Image.FromStream(stream);
                        return image;
                    }
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error to charge Google Profile Photo: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;


            }
        }


    }
}

    
