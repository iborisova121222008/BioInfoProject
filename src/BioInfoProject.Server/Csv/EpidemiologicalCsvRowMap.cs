using CsvHelper.Configuration;

namespace BioInfoProject.Server.Csv;

public sealed class EpidemiologicalCsvRowMap : ClassMap<EpidemiologicalCsvRow>
{
    public EpidemiologicalCsvRowMap()
    {
        Map(row => row.DateReported).Name("date_reported");
        Map(row => row.CountryCode).Name("country_code");
        Map(row => row.Country).Name("country");
        Map(row => row.WhoRegion).Name("who_region");
        Map(row => row.NewCases).Name("new_cases");
        Map(row => row.CumulativeCases).Name("cumulative_cases");
        Map(row => row.NewDeaths).Name("new_deaths");
        Map(row => row.CumulativeDeaths).Name("cumulative_deaths");
    }
}
