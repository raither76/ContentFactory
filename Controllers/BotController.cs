using ContentFactory.Code;
using ContentFactory.Data;
using ContentFactory.Models;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ContentFactory.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class BotController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        IConfiguration _configuration;
        public BotController(ApplicationDbContext context, IConfiguration configuration)
        {
            _configuration = configuration;
            _context = context;

        }
        [HttpPost]

        public async Task<string> GetLink(string _hash, CancellationToken cancellationToken)
        {
            User user = new User();
            user = _context.Users.FirstOrDefault(x => x.SecurityStamp == _hash);
            string token = _configuration["Keys:TBotKey"];

            string code = Coder.Encrypt(user.SecurityStamp, user.PasswordHash);
            string dd = Coder.Decrypt(code, user.PasswordHash);

            if (user != null) return "https://www.ContentFactory.store/Home/Login?_hash=" + code;
            else return "Вы не зарегистрированы в нашей системе";
        }
        // GET: api/<Bot>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<Bot>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<Bot>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<Bot>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<Bot>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
