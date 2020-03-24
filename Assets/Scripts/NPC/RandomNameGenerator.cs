using System;
using System.Collections;
using System.Collections.Generic;

public class RandomNameGenerator
{
    static Random random = new Random();

    /// <summary>
    /// Generates random name with minimum 3 letter, maximum 8 letter and with being one vowel and one consonant order 
    /// </summary>
    /// <returns>Generated Random name</returns>
    public static string NameGenerator()
    {
        string Name = "";
        string[] vowels = { "a", "e", "i", "o", "u" };
        string[] consonants = { "b", "c", "d", "f", "g",
                "h", "j", "k", "l", "m", "n", "p", "q", "r",
                "s", "t", "v", "x", "z", "y", "w" };
        int maxNumber = random.Next(3, 9);
        var start = random.NextDouble();
        while (Name.Length < maxNumber)
        {
            if (start < 0.5)
            {
                Name += vowels[random.Next(0, 5)];
                Name += consonants[random.Next(0, 21)];
            }
            else
            {
                Name += consonants[random.Next(0, 21)];
                Name += vowels[random.Next(0, 5)];
            }
        }
        return char.ToUpper(Name[0]) + Name.Substring(1);

    }
}
