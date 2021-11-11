using Bogus;

namespace SampleMinimalWebApi;

public static class RandomHelper
{
    private static readonly Faker _faker = new();

    public static string RandomName()
    {

        return _faker.Name.FirstName();
    }

    public static int RandomNumber()
    {
        return _faker.Random.Number(1, 1000);
    }
}
