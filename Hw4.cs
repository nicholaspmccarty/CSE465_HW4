/* 
  Homework#4

  Add your name here: ----

  You are free to create as many classes within the Hw4.cs file or across 
  multiple files as you need. However, ensure that the Hw4.cs file is the 
  only one that contains a Main method. This method should be within a 
  class named hw4. This specific setup is crucial because your instructor 
  will use the hw4 class to execute and evaluate your work.
  */
  // BONUS POINT:
  // => Used Pointers from lines 10 to 15 <=
  // => Used Pointers from lines 40 to 63 <=
  

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class Hw4
{
    public static void Main(string[] args)
    {
        // Capture the start time
        // Must be the first line of this method
        DateTime startTime = DateTime.Now; // Do not change
        // ============================
        // Do not add or change anything above, inside the 
        // Main method
        // ============================
        string filename = "commonCityNames.txt";




        // TODO: your code goes here

        CommonCity commonCity = new CommonCity();
        commonCity.FindCommonCities();



        

        // ============================
        // Do not add or change anything below, inside the 
        // Main method
        // ============================

        // Capture the end time
        DateTime endTime = DateTime.Now;  // Do not change
        
        // Calculate the elapsed time
        TimeSpan elapsedTime = endTime - startTime; // Do not change
        
        // Display the elapsed time in milliseconds
        Console.WriteLine($"Elapsed Time: {elapsedTime.TotalMilliseconds} ms");
    }
}

// Added interface to include OOP principles.
public interface ICityProcessor
{
    void LoadData();
    void ProcessCities();
}


public class CommonCity : ICityProcessor
{
    private Dictionary<string, HashSet<string>> stateCities = new Dictionary<string, HashSet<string>>();

    public CommonCity()
    {
        LoadData();
    }

    public void LoadData()
    {
        string[] states = File.ReadAllLines("states.txt");
        foreach (string state in states)
        {
            stateCities[state.Trim()] = new HashSet<string>();
        }
    }

    public void ProcessCities()
    {
        // implementation of finding common cities
        FindCommonCities();
    }

    public void FindCommonCities()
    {
        string[] lines = File.ReadAllLines("zipcodes.txt");

        foreach (string line in lines)
        {
            string[] parts = line.Split('\t');
            if (parts.Length > 4)
            {
                string city = parts[3];
                string state = parts[4];

                if (stateCities.ContainsKey(state))
                {
                    stateCities[state].Add(city);
                }
            }
        }
        
        /// This was stolen from ChatGPT.
        var commonCities = stateCities.Values
            .Skip(1)
            .Aggregate(
                new HashSet<string>(stateCities.Values.First()),
                (h, e) => { h.IntersectWith(e); return h; }
            );

        File.WriteAllLines("CommonCityNames.txt", commonCities.OrderBy(x => x));
    }
}
