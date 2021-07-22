using CowinWatcher.Entities;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace CowinWatcher
{
    class Business
    {
        public Appointment GetMasterDataForDistrict(string districtId)
        {
            try
            {
                string center = "https://cdn-api.co-vin.in/api/v2/appointment/sessions/public/calendarByDistrict?district_id=" + districtId + "&date=" + DateTime.Now.ToString("dd-MM-yyyy");
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(center);
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Accept = "*/*";
                httpWebRequest.Method = "GET";
                httpWebRequest.KeepAlive = false;
                //ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls13;
                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();

                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    Appointment result = JsonConvert.DeserializeObject<Appointment>(streamReader.ReadToEnd());
                    return result;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }

        public StateData GetStates()
        {
            string center = "https://cdn-api.co-vin.in/api/v2/admin/location/states";
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(center);
            // WebResponse httpResponse;
            try
            {

                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Accept = "*/*";
                httpWebRequest.Method = "GET";
                httpWebRequest.KeepAlive = false;
                //ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls13;
                Task<WebResponse> httpResponse = httpWebRequest.GetResponseAsync();

                using (var streamReader = new StreamReader(httpResponse.Result.GetResponseStream()))
                {
                    StateData result = JsonConvert.DeserializeObject<StateData>(streamReader.ReadToEnd());
                    httpResponse.Result.Close();
                    httpWebRequest.Abort();
                    return result;
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }

        }

        public DistrictData GetDistricts(string state_id)
        {
            try
            {
                string center = "https://cdn-api.co-vin.in/api/v2/admin/location/districts/" + state_id;
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(center);
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Accept = "*/*";
                httpWebRequest.Method = "GET";
                httpWebRequest.KeepAlive = false;
                //ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls13;
                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();

                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    DistrictData result = JsonConvert.DeserializeObject<DistrictData>(streamReader.ReadToEnd());
                    httpResponse.Close();
                    return result;
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }

    }
}
