﻿using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Http;
using Domain;
using SimpleStorage.Infrastructure;

namespace SimpleStorage.Controllers
{
    public class ValuesController : ApiController
    {
        private readonly IConfiguration configuration;
        private readonly IStateRepository stateRepository;
        private readonly IMasterConfiguration masterConfiguration;
        private readonly IStorage storage;

        public ValuesController(IStorage storage, IStateRepository stateRepository, IConfiguration configuration, IMasterConfiguration masterConfiguration)
        {
            this.storage = storage;
            this.stateRepository = stateRepository;
            this.configuration = configuration;
            this.masterConfiguration = masterConfiguration;
        }

        // GET api/values 
        public IEnumerable<ValueWithId> Get()
        {
            CheckState();
            return storage.GetAll().ToArray();
        }

        private void CheckState()
        {
            if (stateRepository.GetState() != State.Started)
                throw new HttpResponseException(HttpStatusCode.InternalServerError);
        }

        // GET api/values/5 
        public Value Get(string id)
        {
            CheckState();
            var result = storage.Get(id);
            if (result == null)
                throw new HttpResponseException(HttpStatusCode.NotFound);
            return result;
        }

        // PUT api/values/5
        public void Put(string id, [FromBody] Value value)
        {
            if (!masterConfiguration.IsMaster)
                throw new HttpResponseException(HttpStatusCode.NotImplemented);
            CheckState();
            storage.Set(id, value);
        }
    }
}