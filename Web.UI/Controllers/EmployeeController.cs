using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Models.UI;

namespace Web.UI.Controllers
{
    public class EmployeeController : Controller
    {
        private readonly string baseAPI_Url = System.Configuration.ConfigurationManager.AppSettings["BaseUrl"];
        private readonly string token = System.Web.HttpContext.Current?.Session["Token"]?.ToString();
        private readonly string employeeId = System.Web.HttpContext.Current?.Session["UserId"]?.ToString();

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult> EmployeesList()
        {
            var model = new List<EmployeeModel>();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseAPI_Url);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var json = JsonConvert.SerializeObject(model);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var result1 = await client.GetAsync($"api/employee/getemployees");

                var res = result1.Content.ReadAsStringAsync().Result;

                var result = JsonConvert.DeserializeObject<List<EmployeeModel>>(res);

                model = result;
            }

            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult> Update(int id)
        {
            var model = new EmployeeModel();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseAPI_Url);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var json = JsonConvert.SerializeObject(model);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var result1 = await client.GetAsync($"api/employee/getemployee/{id}");

                var res = result1.Content.ReadAsStringAsync().Result;

                var result = JsonConvert.DeserializeObject<EmployeeModel>(res);

                model = result;

                result1 = await Task.Run(() => client.GetAsync("api/account/getroles")).ConfigureAwait(false);

                res = result1.Content.ReadAsStringAsync().Result;

                var rolesResult = JsonConvert.DeserializeObject<List<RoleModel>>(res);

                model.Roles = rolesResult;

                result1 = await Task.Run(() => client.GetAsync("api/employee/getgender")).ConfigureAwait(false);

                res = result1.Content.ReadAsStringAsync().Result;

                var gendersResult = JsonConvert.DeserializeObject<List<GenderModel>>(res);

                model.Genders = gendersResult;
            }

            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> Update(EmployeeModel model)
        {
            var response = new List<EmployeeModel>();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseAPI_Url);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var json = JsonConvert.SerializeObject(model);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var result1 = await client.PutAsync($"api/employee/updateemployee", content);

                var res = result1.Content.ReadAsStringAsync().Result;

                var updateResult = JsonConvert.DeserializeObject<BaseResponse>(res);

                result1 = await client.GetAsync($"api/employee/getemployees");

                res = result1.Content.ReadAsStringAsync().Result;

                var result = JsonConvert.DeserializeObject<List<EmployeeModel>>(res);

                response = result;
            }

            return View("EmployeesList", response);
        }
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult> Profile()
        {
            var model = new EmployeeModel();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseAPI_Url);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var json = JsonConvert.SerializeObject(model);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var result1 = await client.GetAsync($"api/employee/getemployee/{employeeId}");

                var res = result1.Content.ReadAsStringAsync().Result;

                var result = JsonConvert.DeserializeObject<EmployeeModel>(res);

                model = result;
            }

            return View(model);
        }

        [AllowAnonymous]
        public async Task<ActionResult> Delete(int id)
        {
            var model = new List<EmployeeModel>();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseAPI_Url);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var result1 = await client.DeleteAsync($"api/employee/deleteemployee/{id}");

                var res = result1.Content.ReadAsStringAsync().Result;

                var result = JsonConvert.DeserializeObject<BaseResponse>(res);

                result1 = await client.GetAsync($"api/employee/getemployees");

                res = result1.Content.ReadAsStringAsync().Result;

                model = JsonConvert.DeserializeObject<List<EmployeeModel>>(res);

            }

            return View("EmployeesList", model);
        }
    }
}