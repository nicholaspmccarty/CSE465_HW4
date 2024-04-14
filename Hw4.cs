/* 
  Homework#4
  Add your name here: Nicholas McCarty
  
  Instructions:
  - You are free to create as many classes within the Hw4.cs file or across multiple files as you need.
  - Ensure that the Hw4.cs file is the only one that contains a Main method.
  - This method should be within a class named hw4.
  - This setup is crucial as your instructor will use the hw4 class to execute and evaluate your work.
  
  Bonus Points:
  - Used Pointers from lines 10 to 15
  - Used Pointers from lines 40 to 63
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class Hw4
{
    /*
     * Defines a delegate for handling data processed events.
     */
    public delegate void DataProcessedEventHandler();

    /*
     * Event to signal the completion of data processing.
     */
    public static event DataProcessedEventHandler DataProcessedEvent;

    /*
     * Main method to serve as the entry point of the program.
     *
     * Parameters:
     *  args - Command line arguments passed to the program
     *
     * No return value.
     */
    public static void Main(string[] args)
    {
        DateTime startTime = DateTime.Now; // Record the start time of the program

        string filename = "commonCityNames.txt";
        string latName = "LatLon.txt";
        string citName = "CityStates.txt";

        CommonCity commonCity = new CommonCity(filename);
        commonCity.FindCommonCities();

        LatLon latlonders = new LatLon(latName);
        CityStates cityState = new CityStates(citName);

        // Subscribing to the data processed event
        DataProcessedEvent += () => Console.WriteLine("Data processing complete.");

        // Example of using out
        if (cityState.TryGetPopulation("New York", out int population))
        {
            Console.WriteLine($"Population of New York: {population}");
        }

        DateTime endTime = DateTime.Now; // Record the end time of the program
        TimeSpan elapsedTime = endTime - startTime; // Calculate elapsed time
        
        Console.WriteLine($"Elapsed Time: {elapsedTime.TotalMilliseconds} ms"); // Display elapsed time
    }
}

/*
 * Interface to define methods for classes that process ZIP-related data.
 */
public interface IZipProcessor
{
    /*
     * Loads data from a data source.
     */
    void LoadData();

    /*
     * Processes the loaded data.
     */
    void doProcess();
}

/*
 * Class to manage common cities.
 */
public class CommonCity : IZipProcessor
{
    private Dictionary<string, HashSet<string>> stateCities = new Dictionary<string, HashSet<string>>();
    private string filename;

    /*
     * Constructor to initialize CommonCity with a filename.
     *
     * Parameters:
     *  FileName - The name of the file containing city data
     */
    public CommonCity(string FileName)
    {
        this.filename = FileName;
        LoadData();
    }

    /*
     * Constructor to initialize CommonCity with a FileInfo object.
     *
     * Parameters:
     *  file - The FileInfo object containing the file details
     */
    public CommonCity(FileInfo file)
    {
        this.filename = file.FullName;
        LoadData();
    }

    /*
     * Loads data from the file into stateCities dictionary.
     */
    public void LoadData()
    {
        string[] states = File.ReadAllLines("states.txt");
        foreach (string state in states)
        {
            stateCities[state.Trim()] = new HashSet<string>();
        }
    }

    /*
     * Processes the loaded city data to find common cities across states.
     */
    public void doProcess()
    {
        FindCommonCities();
    }

    /*
     * Finds and writes common cities to a file.
     *
     * No return value.
     */
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

        // ChatGPT method.
        var commonCities = stateCities.Values
            .Skip(1)
            .Aggregate(
                new HashSet<string>(stateCities.Values.First()),
                (h, e) => { h.IntersectWith(e); return h; }
            );

        File.WriteAllLines(filename, commonCities.OrderBy(x => x));
    }

    /*
     * Operator to merge two CommonCity objects.
     *
     * Parameters:
     *  a - The first CommonCity object
     *  b - The second CommonCity object
     *
     * Returns:
     *  A new CommonCity object containing merged data from both input objects.
     */
     // CHATGPT Method
    public static CommonCity operator +(CommonCity a, CommonCity b)
    {
        CommonCity result = new CommonCity("merged_common_cities.txt");
        foreach (var state in b.stateCities)
        {
            if (!result.stateCities.ContainsKey(state.Key))
                result.stateCities[state.Key] = new HashSet<string>();

            foreach (var city in state.Value)
                result.stateCities[state.Key].Add(city);
        }

        foreach (var state in a.stateCities)
        {
            if (!result.stateCities.ContainsKey(state.Key))
                result.stateCities[state.Key] = new HashSet<string>();

            foreach (var city in state.Value)
                result.stateCities[state.Key].Add(city);
        }

        return result;
    }

    /*
     * Property to get or set the filename.
     */
    public string Filename
    {
        get { return filename; }
        set { filename = value; }
    }
}

/*
 * Class to manage latitude and longitude data for ZIP codes.
 */
public class LatLon : IZipProcessor
{
    private Dictionary<string, string> zipLatLong = new Dictionary<string, string>();
    private string outputFileName;

    /*
     * Constructor to initialize LatLon with an output file name.
     *
     * Parameters:
     *  outputFileName - The name of the output file to write latitude and longitude data
     */
    public LatLon(string outputFileName)
    {
        this.outputFileName = outputFileName;
        LoadData();
    }

    /*
     * Constructor to initialize LatLon with a FileInfo object.
     *
     * Parameters:
     *  file - The FileInfo object containing the file details
     */
    public LatLon(FileInfo file)
    {
        this.outputFileName = file.FullName;
        LoadData();
    }

    /*
     * Loads latitude and longitude data from a file into a dictionary.
     */
    public void LoadData()
    {
        var zipCodes = new HashSet<string>(File.ReadAllLines("zips.txt"));
        string[] lines = File.ReadAllLines("zipcodes.txt");
        foreach (var line in lines)
        {
            var parts = line.Split('\t');
            var zipCode = parts[1];
            var lat = parts[6];
            var lon = parts[7];

            if (zipCodes.Contains(zipCode) && !zipLatLong.ContainsKey(zipCode))
            {
                zipLatLong[zipCode] = lat + " " + lon;
            }
        }

        doProcess();
    }

    /*
     * Writes the processed latitude and longitude data to the specified output file.
     */
    public void doProcess()
    {
        using (var file = new StreamWriter(outputFileName))
        {
            foreach (var entry in zipLatLong)
            {
                file.WriteLine($"{entry.Value}");
            }
        }
    }

    /*
     * Updates the latitude and longitude dictionary with data from another dictionary.
     *
     * Parameters:
     *  zipLatLongUpdate - The dictionary to update with new latitude and longitude data
     */
    public void UpdateLatLon(ref Dictionary<string, string> zipLatLongUpdate)
    {
        foreach (var entry in zipLatLong)
        {
            zipLatLongUpdate[entry.Key] = entry.Value;
        }
    }

    /*
     * Property to get or set the filename.
     */
    public string Filename
    {
        get { return outputFileName; }
        set { outputFileName = value; }
    }
}

/*
 * Class to manage city-state relationships and populations.
 */
public class CityStates : IZipProcessor
{
    private HashSet<string> cities;  // Stores cities from cities.txt
    private string outputFileName;   // Filename to write the results
    private Dictionary<string, int?> cityPopulations; // Nullable field to store city populations

    /*
     * Constructor to initialize CityStates with an output file name.
     *
     * Parameters:
     *  outputFileName - The name of the output file to write city-state relationships and populations
     */
    public CityStates(string outputFileName)
    {
        this.outputFileName = outputFileName;
        cities = new HashSet<string>(StringComparer.OrdinalIgnoreCase);  // Ignore case when comparing strings
        LoadData();
    }

    /*
     * Constructor to initialize CityStates with a FileInfo object.
     *
     * Parameters:
     *  file - The FileInfo object containing the file details
     */
    public CityStates(FileInfo file)
    {
        this.outputFileName = file.FullName;
        LoadData();
    }

    /*
     * Loads cities from a file into a HashSet, converting them to uppercase and trimming any whitespace.
     */
    public void LoadData()
    {
        foreach (var line in File.ReadAllLines("cities.txt"))
        {
            cities.Add(line.Trim().ToUpper());
        }
        doProcess();
    }

    /*
     * Processes the loaded data to map cities to states and track city populations.
     */
    public void doProcess()
    {
        // Initialize city populations dictionary
        cityPopulations = new Dictionary<string, int?>();

        // Read 'ZipCodes.txt' and process each line
        string[] lines = File.ReadAllLines("zipcodes.txt");
        
        // I used ChatGPT for this comparison and the following for each loop.
        Dictionary<string, SortedSet<string>> cityStates = new Dictionary<string, SortedSet<string>>(StringComparer.OrdinalIgnoreCase);

        foreach (string line in lines)
        {
            string[] parts = line.Split('\t');
            if (parts.Length > 4)
            {
                string city = parts[3].Trim().ToUpper();  // Convert to uppercase and trim whitespace
                string state = parts[4].Trim();
                int population;

                // Parse population if available
                if (int.TryParse(parts[5], out population))
                {
                    cityPopulations[city] = population;
                }

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

        // Write the results to the output file, with each city followed by its associated states and population if available
        using (StreamWriter writer = new StreamWriter(outputFileName))
        {
            foreach (var cityState in cityStates)
            {
                string populationInfo = cityPopulations.ContainsKey(cityState.Key) ? $" (Population: {cityPopulations[cityState.Key]})" : "";
                writer.WriteLine($"{cityState.Key}{populationInfo}: {string.Join(", ", cityState.Value)}");
            }
        }
    }

    /*
     * Tries to get the population for a given city.
     *
     * Parameters:
     *  city - The city whose population is to be retrieved
     *  population - The output parameter that will hold the population value if found
     *
     * Returns:
     *  True if the population was found, otherwise false.
     */

     // CHATGPT METHOD
    public bool TryGetPopulation(string city, out int population)
    {
        population = 0;
        if (cityPopulations.TryGetValue(city, out int? pop))
        {
            if (pop.HasValue)
            {
                population = pop.Value;
                return true;
            }
        }
        return false;
    }
    
    /*
     * Property to get or set the filename.
     */
    public string Filename
    {
        get { return outputFileName; }
        set { outputFileName = value; }
    }
}

/* 
 * Base class for books.
 */
class Book
{
    /*
     * Prints information about the book.
     */
    public virtual void printInformation()
    {
      Console.WriteLine("I'm a book!");
    }
}

/* 
 * Derived class representing an EBook.
 */
class EBook : Book
{
    /*
     * Overridden method to print information specific to an ebook.
     */
    public override void printInformation()
    {
      Console.WriteLine("I'm an ebook, but I'm still a book!");
    }
}
