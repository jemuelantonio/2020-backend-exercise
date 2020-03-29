using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using CustomerApi.Models;

namespace CustomerApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly CustomerDatabaseContext _customerContext;
        private const string BETWEEN_PATTERN = @"(?<minValue>[\d]+)-(?<maxValue>[\d]+)";
        private const string GREATER_THAN_PATTERN = @">(?<value>[\d]+)";
        private const string LESS_THAN_PATTERN = @"<(?<value>[\d]+)";

        public CustomerController(CustomerDatabaseContext customerContext) {
            _customerContext = customerContext;
        }
        
        
        [Authorize]
        [HttpGet]
        public ActionResult<IEnumerable<Customer>> GetCustomer(string tags, string employees)
        {
            var customers = _customerContext.Customer.ToList();
            
            if (!string.IsNullOrEmpty(tags)) 
            {
                var tagList = tags.ToLower().Split(',').ToList();
                customers = customers.Where(x => x.Tags.Split(',').ToList().Any(y => tagList.Contains(y.ToLower()))).ToList();
            }

            if (!string.IsNullOrEmpty(employees))
            {
                if (Regex.IsMatch(employees, BETWEEN_PATTERN)) 
                {
                    var match = Regex.Match(employees, BETWEEN_PATTERN);
                    var min = match.Groups["minValue"].Value;
                    var max = match.Groups["maxValue"].Value;
                    customers = customers.Where(x => x.Employees >= int.Parse(min) && x.Employees <= int.Parse(max)).ToList();
                } 
                else if (Regex.IsMatch(employees, GREATER_THAN_PATTERN))
                {
                    var match = Regex.Match(employees, GREATER_THAN_PATTERN);
                    var value = match.Groups["value"].Value;
                    customers = customers.Where(x => x.Employees > int.Parse(value)).ToList();
                }
                else if (Regex.IsMatch(employees, LESS_THAN_PATTERN))
                {
                    var match = Regex.Match(employees, LESS_THAN_PATTERN);
                    var value = match.Groups["value"].Value;
                    customers = customers.Where(x => x.Employees < int.Parse(value)).ToList();
                }
                else if (int.TryParse(employees, out int value))
                {
                    customers = customers.Where(x => x.Employees == value).ToList();
                }
            }
            return customers;
        }
    }
}