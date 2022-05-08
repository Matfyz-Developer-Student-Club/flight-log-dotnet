﻿namespace FlightLogNet.Repositories
{
    using System.Collections.Generic;
    using System.Linq;

    using AutoMapper;

    using FlightLogNet.Models;
    using FlightLogNet.Repositories.Entities;
    using FlightLogNet.Repositories.Interfaces;

    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;

    public class FlightRepository : IFlightRepository
    {
        private readonly IMapper mapper;
        private readonly IConfiguration configuration;

        public FlightRepository(IMapper mapper, IConfiguration configuration)
        {
            this.mapper = mapper;
            this.configuration = configuration;
        }

        // TODO 2.1: Upravte metodu tak, aby vrátila pouze lety specifického typu
        public IList<FlightModel> GetAllFlights(FlightType? flightType)
        {
            using var dbContext = new LocalDatabaseContext(this.configuration);

            var flights = dbContext.Flights.AsQueryable();
            if (flightType.HasValue)
            {
                flights = flights.Where(f => f.Type == flightType.Value);
            }

            return this.mapper.ProjectTo<FlightModel>(flights).ToList();
        }

        // TODO 2.3: Vytvořte metodu, která načte letadla, která jsou ve vzduchu, seřadí je od nejstarších,
        // a v případě shody dá vlečné pred kluzák, který táhne
        public IList<FlightModel> GetAirPlanesInAir()
        {
            using var dbContext = new LocalDatabaseContext();

            var flights = dbContext.Flights
                .Include(f => f.Airplane)
                .Include(f => f.Copilot)
                .Include(f => f.Pilot)
                .Where(f => f.LandingTime == null)
                .OrderBy(f => f.TakeoffTime)
                .ThenBy(f => f.Type);

            return this.mapper.ProjectTo<FlightModel>(flights).ToList();
        }


        public void LandFlight(FlightLandingModel landingModel)
        {
            using var dbContext = new LocalDatabaseContext(this.configuration);

            var flight = dbContext.Flights.Find(landingModel.FlightId);
            flight.LandingTime = landingModel.LandingTime;
            dbContext.SaveChanges();
        }

        public void TakeoffFlight(long? gliderFlightId, long? towplaneFlightId)
        {
            using var dbContext = new LocalDatabaseContext(this.configuration);

            var flightStart = new FlightStart
            {
                Glider = dbContext.Flights.Find(gliderFlightId),
                Towplane = dbContext.Flights.Find(towplaneFlightId),
            };

            dbContext.FlightStarts.Add(flightStart);
            dbContext.SaveChanges();
        }

        public long CreateFlight(CreateFlightModel model)
        {
            using var dbContext = new LocalDatabaseContext(this.configuration);

            var copilot = model.CopilotId != null
                ? dbContext.Persons.Find(model.CopilotId)
                : null;

            var flight = new Flight
            {
                Airplane = dbContext.Airplanes.Find(model.AirplaneId),
                Copilot = copilot,
                Pilot = dbContext.Persons.Find(model.PilotId),
                TakeoffTime = model.TakeOffTime,
                Task = model.Task
            };

            dbContext.Flights.Add(flight);
            dbContext.SaveChanges();

            return flight.Id;
        }

        public IList<ReportModel> GetReport()
        {
            using var dbContext = new LocalDatabaseContext(this.configuration);

            var flightStarts = dbContext.FlightStarts
                .Include(flight => flight.Glider)
                .Include(flight => flight.Glider.Airplane)
                .Include(flight => flight.Glider.Pilot)
                .Include(flight => flight.Glider.Copilot)
                .Include(flight => flight.Towplane)
                .Include(flight => flight.Towplane.Airplane)
                .Include(flight => flight.Towplane.Pilot)
                .Include(flight => flight.Towplane.Copilot)
                .OrderByDescending(start => start.Towplane.TakeoffTime);

            return this.mapper.ProjectTo<ReportModel>(flightStarts).ToList();
        }
    }
}
