using BankRDMS.API.Models.Requests;
using BankRDMS.API.Models.Responses;
using BankRDMS.Core.Entities;
using BankRDMS.Service.Abstract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace BankRDMS.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CustomerController : ControllerBase
{
    private readonly ICustomerService _customerService;

    public CustomerController(ICustomerService customerService)
    {
        _customerService = customerService;
    }

    [AllowAnonymous]
    [EnableRateLimiting("registration")]
    [HttpPost("create")]
    public async Task<IActionResult> CreateCustomer([FromBody] CreateCustomerRequest request)
    {
        var customer = new Customer
        {
            FullName = request.FullName,
            IdentityNumber = request.IdentityNumber,
            BirthPlace = request.BirthPlace,
            BirthDate = request.BirthDate
        };

        await _customerService.AddCustomerWithAccountAsync(customer);

        return Ok(new
        {
            Message = "Müşteri, vadesiz hesap ve banka kartı başarıyla oluşturuldu.",
            CustomerId = customer.Id
        });
    }

    [HttpGet("getall")]
    public IActionResult GetAllCustomers()
    {
        var customers = _customerService.GetAllCustomers()
            .Select(CustomerResponse.FromEntity)
            .ToList();

        return customers.Count == 0
            ? NotFound(new { Message = "Müşteri bulunamadı." })
            : Ok(customers);
    }

    [HttpGet("getbyid/{id:int}")]
    public IActionResult GetById(int id)
    {
        var customer = _customerService.GetCustomerById(id);
        return customer is null
            ? NotFound("Müşteri bulunamadı.")
            : Ok(CustomerResponse.FromEntity(customer));
    }
}
