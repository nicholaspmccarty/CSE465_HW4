/* 
  Homework#4

  Add your name here: Nicholas McCarty

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
        string latName = "LatLon.txt";
        string citName = "CityStates.txt";



        // TODO: your code goes here

        CommonCity commonCity = new CommonCity(filename);
        commonCity.FindCommonCities();

        LatLon latlonders = new LatLon(latName);
        CityStates cityState = new CityStates(citName);



        

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
public interface IZipProcessor
{
    
  
    void LoadData();
    void doProcess();
}


public class CommonCity : IZipProcessor
{
    private Dictionary<string, HashSet<string>> stateCities = new Dictionary<string, HashSet<string>>();
    string filename = "";

    public CommonCity(string FileName)
    {
        this.filename = FileName;
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

    public void doProcess()
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

        File.WriteAllLines(filename, commonCities.OrderBy(x => x));
    

}
 public string Filename
    {
        get { return filename; }
        set { filename = value; }
    }
}



public class LatLon : IZipProcessor
{
    private Dictionary<string, string> zipLatLong = new Dictionary<string, string>();
    private string outputFileName;

    public LatLon(string outputFileName)
    {
        this.outputFileName = outputFileName;
        LoadData();
    }

    public void LoadData()
    {
        // Load zip codes from 'zips.txt' into a HashSet for quick lookup.
        var zipCodes = new HashSet<string>(File.ReadAllLines("zips.txt"));

        // Read 'zipcodes.txt' and map each zip code to its first latitude and longitude if it's in zipCodes.
        string[] lines = File.ReadAllLines("zipcodes.txt");
        foreach (var line in lines)
        {
            var parts = line.Split('\t');
            var zipCode = parts[1];
            var lat = parts[6];
            var lon = parts[7];

            // Add the first occurrence of the latitude and longitude for the zip code in zipCodes.
            if (zipCodes.Contains(zipCode) && !zipLatLong.ContainsKey(zipCode))
            {
                zipLatLong[zipCode] = lat + " " + lon;
            }
        }

        doProcess();
    }

    public void doProcess()
    {
        // Write the latitudes and longitudes to 'LatLon.txt'.
        using (var file = new StreamWriter(outputFileName))
        {
            foreach (var entry in zipLatLong)
            {
                file.WriteLine($"{entry.Key}: {entry.Value}");
            }
        }
    }
     public string Filename
    {
        get { return outputFileName; }
        set { outputFileName = value; }
    }
}

    

public class CityStates : IZipProcessor
{
    private HashSet<string> cities;  // Stores cities from cities.txt
    private string outputFileName;   // Filename to write the results

    public CityStates(string outputFileName)
    {
        this.outputFileName = outputFileName;
        cities = new HashSet<string>(StringComparer.OrdinalIgnoreCase);  // Ignore case when comparing strings
        LoadData();
    }

    public void LoadData()
    {
        // Load cities from 'cities.txt', converting them to uppercase and trimming any whitespace
        foreach (var line in File.ReadAllLines("cities.txt"))
        {
            cities.Add(line.Trim().ToUpper());
        }
        doProcess();
    }

    public void doProcess()
    {
        // Read 'ZipCodes.txt' and process each line
        string[] lines = File.ReadAllLines("zipcodes.txt");
        Dictionary<string, SortedSet<string>> cityStates = new Dictionary<string, SortedSet<string>>(StringComparer.OrdinalIgnoreCase);

        foreach (string line in lines)
        {
            string[] parts = line.Split('\t');
            if (parts.Length > 4)
            {
                string city = parts[3].Trim().ToUpper();  // Convert to uppercase and trim whitespace
                string state = parts[4].Trim();

                // Check if the city is in our loaded list of cities, then add the state to the corresponding city's SortedSet
                if (cities.Contains(city))
                {
                    if (!cityStates.ContainsKey(city))
                    {
                        cityStates[city] = new SortedSet<string>();
                    }
                    cityStates[city].Add(state);
                }
            }
        }

        // Write the results to the output file, with each city followed by its associated states
        using (StreamWriter writer = new StreamWriter(outputFileName))
        {
            foreach (var cityState in cityStates)
            {
                writer.WriteLine($"{cityState.Key}: {string.Join(" ", cityState.Value)}");
            }
        }
    }
     public string Filename
    {
        get { return outputFileName; }
        set { outputFileName = value; }
    }
    
}