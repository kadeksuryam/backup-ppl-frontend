﻿using Cakrawala.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using static Cakrawala.Data.DashboardService;

namespace Cakrawala.Data
{
    public class TransferService
    {
        HttpClient client;

        public TransferService()
        {
            client = new HttpClient();
            client.DefaultRequestHeaders
                .Accept
                .Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
        }

        public async Task<bool> TransferAsync(string receiverId, int nominal)
        {
            Uri uri = new Uri(string.Format(Constants.LocalRestUrl + "transfer", string.Empty));
            bool output = false;
            try
            {
                string json = JsonConvert.SerializeObject(new TransferRequest(receiverId, nominal));
                StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, uri);
                request.Content = content;
                request.Headers.Add("Authorization", $"Bearer {Application.Current.Properties["token"]}");
                HttpResponseMessage response = await client.SendAsync(request);

                // HttpResponseMessage response = await client.PostAsync(uri, content);

                if (response.IsSuccessStatusCode)
                {
                    Debug.WriteLine("Success Transfer");
                    output = true;
                }

                Debug.WriteLine(response.ToString());
            }
            catch (Exception ex)
            {
                Console.WriteLine(@"\tERROR {0}", ex.Message);
            }

            return output;
        }

        public async Task<User> FindUserAsync(string userId)
        {
            Uri uri = new Uri(string.Format(Constants.RestUrl + $"users?userId={userId}", string.Empty));
            User userResp = null;

            try
            {
                // string json = JsonConvert.SerializeObject(new TopupRequest(voucherCode));
                // StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, uri);
                request.Headers.Add("Authorization", $"Bearer {Application.Current.Properties["token"]}");
                HttpResponseMessage response = await client.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    string rawResp = await response.Content.ReadAsStringAsync();
                    userResp = JsonConvert.DeserializeObject<User>(rawResp);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error" + ex.Message);
                return null;
            }

            return userResp;
        }



        // BELOW IS CLASSES

        private class TransferRequest
        {
            public string receiverId { get; set; }
            public int nominal { get; set; }

            public TransferRequest(string _receiverId, int _nominal)
            {
                receiverId = _receiverId;
                nominal = _nominal;
            }
        }

        private class TransferResponse
        {
            public Boolean success { get; set; }

            public TransferResponse(Boolean _success)
            {
                success = _success;
            }
        }
    }
}