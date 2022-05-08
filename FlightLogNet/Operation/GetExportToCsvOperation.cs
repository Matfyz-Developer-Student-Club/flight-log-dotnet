namespace FlightLogNet.Operation
{
    using System.Text;
    using System.Linq;
    using System.Globalization;

    using FlightLogNet.Models;
    using FlightLogNet.Repositories.Interfaces;

    public class GetExportToCsvOperation
    {
        private readonly IFlightRepository flightRepository;
        private const string DATE_FORMAT = "dd.MM.yyyy HH:mm:ss";

        public GetExportToCsvOperation(IFlightRepository flightRepository)
        {
            this.flightRepository = flightRepository;
        }

        public byte[] Execute()
        {
            //before your loop
            var csv = new StringBuilder();

            // var reports = this.flightRepository.GetReport();

            csv.AppendLine(
                "ID Letu;Čas vzletu;Typ letadla; Imatrikulace Letadla;Příjmení Pilota;Jméno Pilota;Adresa pilota - ulice;Adresa pilota - město;Adresa pilota - PSČ;Adresa pilota - země;Type;Příjmení kopilota;Jméno kopilota;Čas přistání;Doba letu v minutách;Úloha;Poznámka");

            foreach (var report in this.flightRepository.GetReport())
            {
                if (report.Towplane != null)
                {
                    // Towplane
                    csv.Append($"{report.Towplane.Id};");
                    csv.Append($"{report.Towplane.TakeoffTime.ToString(DATE_FORMAT)};");
                    csv.Append($"{report.Towplane.Airplane?.Type};");
                    csv.Append($"{report.Towplane.Airplane?.Immatriculation}");
                    csv.Append($"{report.Towplane.Pilot?.LastName};{report.Towplane.Pilot?.FirstName};");
                    csv.Append($"{report.Towplane.Pilot?.Address.Street};{report.Towplane.Pilot?.Address.City};{report.Towplane.Pilot?.Address.PostalCode};{report.Towplane.Pilot?.Address.Country};");
                    csv.Append($"{report.Towplane.Copilot?.LastName} {report.Towplane.Copilot?.FirstName};");
                    csv.Append($"{report.Towplane.LandingTime?.ToString(DATE_FORMAT)};");
                    csv.Append($"{report.Towplane.LandingTime - report.Towplane.TakeoffTime};");
                    csv.Append($"{report.Towplane.Task};");
                    csv.AppendLine();
                    
                }

                if (report.Glider != null)
                {
                    // Glider
                    csv.Append($"{report.Glider.Id};");
                    csv.Append($"{report.Glider.TakeoffTime.ToString(DATE_FORMAT)};");
                    csv.Append($"{report.Glider.Airplane?.Type};");
                    csv.Append($"{report.Glider.Airplane?.Immatriculation}");
                    csv.Append($"{report.Glider.Pilot?.LastName};{report.Glider.Pilot?.FirstName};");
                    csv.Append($"{report.Glider.Pilot?.Address.Street};{report.Glider.Pilot?.Address.City};{report.Glider.Pilot?.Address.PostalCode};{report.Glider.Pilot?.Address.Country};");
                    csv.Append($"{report.Glider.Copilot?.LastName} {report.Glider.Copilot?.FirstName};");
                    csv.Append($"{report.Glider.LandingTime?.ToString(DATE_FORMAT)};");
                    csv.Append($"{report.Glider.LandingTime - report.Glider.TakeoffTime};");
                    csv.Append($"{report.Glider.Task};");
                    csv.AppendLine();
                }
            }

            return Encoding.UTF8.GetPreamble().Concat(Encoding.UTF8.GetBytes(csv.ToString())).ToArray();
        }
    }
}
