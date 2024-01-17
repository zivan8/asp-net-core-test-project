using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using ChartDirector;
using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Test.Data;
using Test.Models;
using System.IO;
using System.Net;

namespace Test.Controllers
{
    [Route("employees")]
    [ApiController]
    public class EmployeesController : Controller
    {
        
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _config;
        private readonly IEmployeesRepo _repository;     
        public EmployeesController(IConfiguration config, IHttpClientFactory httpClientFactory, IEmployeesRepo repo)
        {
            _httpClientFactory = httpClientFactory;
            _repository = repo;
            _config = config;
        }

        [HttpGet]
        private async Task<IEnumerable<TimeEntry>> GetTimeEntries()
        {
            var httpClient = _httpClientFactory.CreateClient("TestClient");
            var timeEntriesKey = _config.GetSection("WebConfig").GetValue<string>("TimeEntriesKey");
            var response = await httpClient.GetAsync("gettimeentries?code=" + timeEntriesKey);  // no need to URL encode as it comes from config

            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsStringAsync();                
                var timeEntries = JsonConvert.DeserializeObject<IEnumerable<TimeEntry>>(data);
                
                return timeEntries;
            }
            else
            {
                return null;
            }
        }       
        private async Task<IEnumerable<Employee>> GetEmployees()
        {
            IEnumerable<Employee> employees = _repository.GetEmployees();
            if (employees != null && employees.Count() > 0)
            {
                return employees;
            }

            var timeEntries = await GetTimeEntries(); 
            if (timeEntries == null)
                return null;

            ConcurrentDictionary<string, Employee> employeeDict = new ConcurrentDictionary<string, Employee>();
            var validTimeEntries = timeEntries.Where(e => !string.IsNullOrEmpty(e.EmployeeName));
            foreach (var entry in validTimeEntries)
            {
                Employee e = employeeDict.GetOrAdd(
                    entry.EmployeeName,
                    name => new Employee(name, new TimeSpan())
                );
                e.TotalTimeWorked = e.TotalTimeWorked.Add(entry.TotalTime);
            }

            employees = employeeDict.Values.OrderBy(e => e.TotalTimeWorked).ToList();
            _repository.SetEmployees(employees);

            return employees;
        }

        public async Task<IActionResult> Index()
        {
            IEnumerable<Employee> employees = await GetEmployees();
            if (employees == null)
                return BadRequest("Employees are not found!");
            return View(employees);
        }

        [Route("chart")]
        public async Task<FileResult> PieChart()
        {
            IEnumerable<Employee> employees = await GetEmployees();

            List<double> totalTimes = employees.Select(e => e.TotalTimeWorked.TotalSeconds).ToList();
            double totalSeconds = totalTimes.Sum();
            double[] data = employees.Select(e => e.TotalTimeWorked.TotalSeconds / totalSeconds).ToArray();
            string[] labels = employees.Select(e => e.Name).ToArray();

            PieChart chart = new PieChart(640, 640);
            chart.setPieSize(320, 320, 200);
            chart.setData(data, labels);
            byte[] bytes = chart.makeChart2(Chart.PNG);

            return new FileStreamResult(new MemoryStream(bytes), "image/png") { FileDownloadName = "chart.png" };
        }
    }
}
