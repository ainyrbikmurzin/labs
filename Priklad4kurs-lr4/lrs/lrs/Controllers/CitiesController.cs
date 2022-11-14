﻿using AutoMapper;
using Contracts;
using Entities.DataTransferObjects;
using Entities.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace lrs.Controllers
{
    [Route("api/partworlds/{partWorldId}/countries/{countryId}/cities")]
    [ApiController]
    public class CitiesController : ControllerBase
    {
        private readonly IRepositoryManager _repository;
        private readonly ILoggerManager _logger;
        private readonly IMapper _mapper;

        public CitiesController(IRepositoryManager repository, ILoggerManager logger, IMapper mapper)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
        }
        [HttpGet]
        public IActionResult GetCities(Guid partWorldId, Guid countryId)
        {
            var actionResult = checkResult(partWorldId, countryId);
            if (actionResult != null)
                return actionResult;
            var citiesFromDb = _repository.City.GetCities(countryId, false);
            var citiesDto = _mapper.Map<IEnumerable<CityDto>>(citiesFromDb);
            return Ok(citiesDto);
        }
        [HttpGet("{id}")]
        public IActionResult GetCity(Guid partWorldId, Guid countryId, Guid id)
        {
            var actionResult = checkResult(partWorldId, countryId);
            if (actionResult != null)
                return actionResult;
            var cityDb = _repository.City.GetCity(countryId, id, false);
            if (cityDb == null)
            {
                _logger.LogInfo($"City with id: {id} doesn't exist in the database.");
                return NotFound();
            }
            var city = _mapper.Map<CityDto>(cityDb);
            return Ok(city);
        }
        private IActionResult checkResult(Guid partWorldId, Guid countryId)
        {
            var partWorld = _repository.PartWorld.GetPartWorld(partWorldId, false);
            if (partWorld == null)
            {
                _logger.LogInfo($"Partworld with id: {partWorldId} doesn't exist in the database.");
                return NotFound();
            }
            var country = _repository.Country.GetCountry(partWorldId, countryId, false);
            if (country == null)
            {
                _logger.LogInfo($"Country with id: {countryId} doesn't exist in the database.");
                return NotFound();
            }
            return null;
        }
    }
}
