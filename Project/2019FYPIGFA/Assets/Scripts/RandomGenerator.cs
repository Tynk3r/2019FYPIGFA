using System;
using System.Text;
using UnityEngine;

public class RandomGenerator
{
    // Generate a random number between two numbers    
    public int RandomNumber(int min, int max)
    {
        System.Random random = new System.Random();
        return random.Next(min, max);
    }

    // Generate a random string with a given size    
    public string RandomString(int size, bool lowerCase = false)
    {
        StringBuilder builder = new StringBuilder();
        System.Random random = new System.Random();
        char ch;
        for (int i = 0; i < size; i++)
        {
            ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65)));
            builder.Append(ch);
        }
        if (lowerCase)
            return builder.ToString().ToLower();
        return builder.ToString();
    }

    // Generate a random string with a random size from 3 to 10
    public string RandomString(bool lowerCase = false)
    {
        StringBuilder builder = new StringBuilder();
        System.Random random = new System.Random();
        char ch;
        for (int i = 0; i < RandomNumber(3,10); i++)
        {
            ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65)));
            builder.Append(ch);
        }
        if (lowerCase)
            return builder.ToString().ToLower();
        return builder.ToString();
    }

    // Generate a random password    
    public string RandomPassword()
    {
        StringBuilder builder = new StringBuilder();
        builder.Append(RandomString(4, true));
        builder.Append(RandomNumber(1000, 9999));
        builder.Append(RandomString(2, false));
        return builder.ToString();
    }
}
