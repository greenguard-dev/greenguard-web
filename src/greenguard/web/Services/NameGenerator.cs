namespace web.Services;

public static class NameGenerator
{
    private static string[] _names =
    [
        "Lysandra", "Thalia", "Zinnia", "Amaranth", "Saphira", "Calyx", "Thornis", "Petalyn", "Solara", "Virella",
        "Acantha", "Lavendra", "Iriska", "Elowen", "Flora", "Orion", "Celestia", "Lumo", "Zephyr", "Nimbus",
        "Solace", "Vega", "Astra", "Mira", "Ceres", "Sylphix", "Aqualis", "Florion", "Verdant", "Beryl",
        "Sylva", "Vella", "Nova", "Fauna", "Elara", "Quilla", "Vaera", "Talia", "Aria", "Cassia", "Lyra",
        "Irina", "Vega", "Fira", "Thalix", "Elysia", "Nysa", "Saphir", "Helia", "Calia"
    ];

    private static readonly Random Random = new();

    public static string GenerateName()
    {
        var firstNameIndex = Random.Next(0, _names.Length);
        var secondNameIndex = Random.Next(0, _names.Length);

        while (firstNameIndex == secondNameIndex)
        {
            secondNameIndex = Random.Next(0, _names.Length);
        }

        return $"{_names[firstNameIndex]} {_names[secondNameIndex]}";
    }
}