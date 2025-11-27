using Bogus;
using PurposePunch.Application.Interfaces;
using System.Globalization;

namespace PurposePunch.Infrastructure.Services;

public class NicknameGenerator : INicknameGenerator
{
    private readonly Faker _faker = new("en");

    public string Generate()
    {
        var adj = _faker.Hacker.Adjective();
        var noun = _faker.Hacker.Noun();

        var textInfo = new CultureInfo("en-US").TextInfo;
        adj = textInfo.ToTitleCase(adj);
        noun = textInfo.ToTitleCase(noun);

        var number = _faker.Random.Number(10, 999);

        return $"{adj}{noun}{number}".Replace(" ", "").Replace("-", "");
    }
}
