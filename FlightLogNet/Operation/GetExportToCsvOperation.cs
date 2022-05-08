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
                "FlightId;TakeoffTime;LandingTime;Immatriculation;Type;Pilot;Copilot;Task;TowplaneID;GliderID");

            foreach (var report in this.flightRepository.GetReport())
            {
                if (report.Towplane != null)
                {
                    // Towplane
                    csv.Append($"{report.Towplane.Id};{report.Towplane.TakeoffTime.ToString(DATE_FORMAT)};{report.Towplane.LandingTime?.ToString(DATE_FORMAT)};");
                    csv.Append($"{report.Towplane.Airplane?.Immatriculation};{report.Towplane.Airplane?.Type};");
                    csv.Append($"{report.Towplane.Pilot?.FirstName} {report.Towplane.Pilot?.LastName};");
                    csv.Append($"{report.Towplane.Copilot?.FirstName} {report.Towplane.Copilot?.LastName};");
                    csv.Append($"{report.Towplane.Task};{report.Towplane?.Id};{report.Glider?.Id}");
                    csv.AppendLine();
                }

                if (report.Glider != null)
                {
                    // Glider
                    csv.Append($"{report.Glider.Id};{report.Glider.TakeoffTime.ToString(DATE_FORMAT)};{report.Glider.LandingTime?.ToString(DATE_FORMAT)};");
                    csv.Append($"{report.Glider.Airplane?.Immatriculation};{report.Glider.Airplane?.Type};");
                    csv.Append($"{report.Glider.Pilot?.FirstName} {report.Glider.Pilot?.LastName};");
                    csv.Append($"{report.Glider.Copilot?.FirstName} {report.Glider.Copilot?.LastName};");
                    csv.Append($"{report.Glider.Task};{report.Towplane?.Id};{report.Glider?.Id}");
                    csv.AppendLine();
                }
            }

            return Encoding.UTF8.GetPreamble().Concat(Encoding.UTF8.GetBytes(csv.ToString())).ToArray();
        }
    }
}
