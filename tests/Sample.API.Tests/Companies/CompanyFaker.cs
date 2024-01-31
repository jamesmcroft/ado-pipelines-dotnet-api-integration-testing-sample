using Bogus;
using Sample.API.Companies;
using Sample.API.Companies.Entities;
using Sample.API.Companies.Models;

namespace Sample.API.Tests.Companies;

public static class CompanyFaker
{
    public static Faker<Company> Company() =>
        new Faker<Company>()
            .RuleFor(x => x.Name, faker => faker.Company.CompanyName());

    public static Faker<CompanyModel> CompanyModel() =>
        new Faker<CompanyModel>()
            .RuleFor(x => x.Id, faker => faker.Random.Int())
            .RuleFor(x => x.Name, faker => faker.Company.CompanyName());

    public static Faker<CreateCompany.CreateCompanyRequest> CreateCompanyRequest() =>
        new Faker<CreateCompany.CreateCompanyRequest>()
            .RuleFor(x => x.Name, faker => faker.Company.CompanyName());
}
