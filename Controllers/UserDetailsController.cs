using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using capstoneDotNet.DTOs;
using capstoneDotNet.Interfaces;
using capstoneDotNet.Models;
using capstoneDotNet.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace capstoneDotNet.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class UsersController : ControllerBase

    {
        private IConfiguration _configuration;
        private readonly IWebHostEnvironment _env;
        private readonly ITokenService _tokenService;

        public UsersController(IConfiguration config, IWebHostEnvironment env, ITokenService tokenService)
        {
            _configuration = config;
            _env = env;
            _tokenService = tokenService;
        }

        //get all users
        [HttpGet]
        public ResponseModel Get()
        {
            ResponseModel _objResponseModel = new ResponseModel();

            string query = @"select * from User_Details";

            DataTable table = new DataTable();

            string sqlDataSource = _configuration.GetConnectionString("Default");
            SqlDataReader myReader;

            using (SqlConnection myCon = new SqlConnection(sqlDataSource))
            {
                myCon.Open();
                using (SqlCommand myCommand = new SqlCommand(query, myCon))
                {
                    myReader = myCommand.ExecuteReader();
                    table.Load(myReader);
                    myReader.Close();
                    myCon.Close();
                }
            }

            List<dynamic> userList = new List<dynamic>();
            for (int i = 0; i < table.Rows.Count; i++)
            {
                UserDetails temp = new UserDetails();
                temp.id = Convert.ToInt32(table.Rows[i]["id"]);
                temp.firstName = table.Rows[i]["first_name"].ToString();
                temp.lastName = table.Rows[i]["last_name"].ToString();
                temp.email = table.Rows[i]["email"].ToString();
                temp.passwordHash = table.Rows[i]["password_hash"].ToString();
                temp.passwordSalt = table.Rows[i]["password_salt"].ToString();
                temp.role = table.Rows[i]["role"].ToString();
                temp.designation = table.Rows[i]["designation"].ToString();
                userList.Add(temp);
            }

            _objResponseModel.Data = userList;
            _objResponseModel.Status = true;
            _objResponseModel.Message = "User details data received successfully";
            return _objResponseModel;
        }


        //get id of user
        [Authorize]
        [HttpGet("{id}")]
        public ResponseModel Get(int id)
        {
            ResponseModel _objResponseModel = new ResponseModel();

            string query = @"
                            select * from User_Details where id = @id
                            ";

            DataTable table = new DataTable();

            string sqlDataSource = _configuration.GetConnectionString("Default");
            SqlDataReader myReader;

            using (SqlConnection myCon = new SqlConnection(sqlDataSource))
            {
                myCon.Open();
                using (SqlCommand myCommand = new SqlCommand(query, myCon))
                {
                    myCommand.Parameters.AddWithValue("@id", id);
                    myReader = myCommand.ExecuteReader();
                    table.Load(myReader);
                    myReader.Close();
                    myCon.Close();
                }
            }

            List<dynamic> userList = new List<dynamic>();

            if (table.Rows.Count == 0)
            {
                _objResponseModel.Message = "No such id can be found";
            }
            else
            {
                for (int i = 0; i < table.Rows.Count; i++)
                {
                    UserDetails temp = new UserDetails();
                    temp.id = Convert.ToInt32(table.Rows[i]["id"]);
                    temp.firstName = table.Rows[i]["first_name"].ToString();
                    temp.lastName = table.Rows[i]["last_name"].ToString();
                    temp.email = table.Rows[i]["email"].ToString();
                    temp.password = table.Rows[i]["password"].ToString();
                    temp.role = table.Rows[i]["role"].ToString();
                    temp.designation = table.Rows[i]["designation"].ToString();
                    userList.Add(temp);
                    
                }
                _objResponseModel.Message = "User id data received successfully";
            }

            _objResponseModel.Data = userList;
            _objResponseModel.Status = true;
            //status if success or fail

            return _objResponseModel;
        }

        //sign up account
        [HttpPost("signup")]
        public async Task<ActionResult<string>> Signup(UserDetails userdata)
        {
            using var hmac = new HMACSHA512();

            var password = userdata.password;

            var user = new UserDetails
            {
                email = userdata.email.ToLower(),
                passwordHash = Convert.ToBase64String(hmac.ComputeHash(Encoding.UTF8.GetBytes(password))),
                passwordSalt = Convert.ToBase64String(hmac.Key)
            };

            string queryEmail = @"select count(*) from User_Details where email = @email";

            int RowExists = 0;

            string query = @"insert into User_Details(first_name, last_name, email, password_hash, password_salt) values (@first_name, @last_name, @email, @password_hash, @password_salt)";

            DataTable table = new DataTable();

            string sqlDataSource = _configuration.GetConnectionString("Default");
            SqlDataReader myReader;

            using (SqlConnection myCon = new SqlConnection(sqlDataSource))
            {
                myCon.Open();

                using (SqlCommand findEmail = new SqlCommand(queryEmail, myCon))
                {
                    findEmail.Parameters.AddWithValue("@email", userdata.email);
                    RowExists = (int)findEmail.ExecuteScalar();
                }

                if (RowExists == 1)
                {
                    return BadRequest("Email already exists. Please login or signup with a different email address");
                }

                else
                { 
                    using (SqlCommand myCommand = new SqlCommand(query, myCon))
                        {
                                myCommand.Parameters.AddWithValue("@first_name", userdata.firstName);
                                myCommand.Parameters.AddWithValue("@last_name", userdata.lastName);
                                myCommand.Parameters.AddWithValue("@email", user.email);
                                myCommand.Parameters.AddWithValue("@password_hash", user.passwordHash);
                                myCommand.Parameters.AddWithValue("@password_salt", user.passwordSalt);
                        

                                myReader = myCommand.ExecuteReader();
                                table.Load(myReader);
                        }
                }
                myReader.Close();
                myCon.Close();

                string token = _tokenService.CreateToken(userdata);

                return Ok(token);
            }
        }

        //login
        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login(UserDetails userdata)
        {

            string query = @"select * from User_Details where email = @email";

            DataTable table = new DataTable();

            string sqlDataSource = _configuration.GetConnectionString("Default");
            SqlDataReader myReader;

            using (SqlConnection myCon = new SqlConnection(sqlDataSource))
            {
                myCon.Open();
                using (SqlCommand myCommand = new SqlCommand(query, myCon))
                {
                    myCommand.Parameters.AddWithValue("@email", userdata.email);
                    myReader = myCommand.ExecuteReader();
                    table.Load(myReader);
                    myReader.Close();
                    myCon.Close();
                }

            }

            List<dynamic> userList = new List<dynamic>();

            if (table.Rows.Count == 0)
            {
                return BadRequest("Invalid email address. Please login with the correct email address");
            }

            else
            {
                for (int i = 0; i < table.Rows.Count; i++)
                {
                    UserDetails temp = new UserDetails();

                    temp.email = table.Rows[i]["email"].ToString();
                    temp.passwordHash = table.Rows[i]["password_hash"].ToString();
                    temp.passwordSalt = table.Rows[i]["password_salt"].ToString();
                    userList.Add(temp);

                }

                var loginPassword = userdata.password;

                var storedPasswordHash = Convert.FromBase64String(userList[0].passwordHash);

                if (!VerifyPasswordHash(loginPassword, storedPasswordHash, userList[0].passwordSalt))
                {
                    return BadRequest("Invalid password");
                }

            }

            string token = _tokenService.CreateToken(userdata);

            var userDto = new UserDto
            {
                email = userdata.email,
                token = token,
                role = userdata.role
            };



            return Ok(userDto);


          
        }

        private bool VerifyPasswordHash(string loginPassword, byte[] storedPasswordHash, string storedPasswordSalt)
        {
            using var hmac = new HMACSHA512(Convert.FromBase64String(storedPasswordSalt));

            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginPassword));

            for (int y = 0; y < computedHash.Length; y++)
            {
                if (computedHash[y] != storedPasswordHash[y]) return false;
            }

            return true;
        }


        //update role and designation 
        [Route("manageUsers")]
        [HttpPut]
        public StatusResponseModel Put(UserDetails userdata)
        {
            StatusResponseModel _objResponseModel = new StatusResponseModel();

            string query = @"
                            update User_Details set
                            role = @role,
                            designation = @designation
                            where id = @id
                            ";

            DataTable table = new DataTable();

            string sqlDataSource = _configuration.GetConnectionString("Default");
            SqlDataReader myReader;

            using (SqlConnection myCon = new SqlConnection(sqlDataSource))
            {
                myCon.Open();
                using (SqlCommand myCommand = new SqlCommand(query, myCon))
                {
                    myCommand.Parameters.AddWithValue("@id", userdata.id);
                    myCommand.Parameters.AddWithValue("@role", userdata.role);
                    myCommand.Parameters.AddWithValue("@designation", userdata.designation);
                    
                    myReader = myCommand.ExecuteReader();
                    table.Load(myReader);
                    myReader.Close();
                    myCon.Close();
                }
            }

            _objResponseModel.Status = true;
            _objResponseModel.Message = "User roles updated successfully";
            return _objResponseModel;
        }


        //delete user
        [HttpDelete("{id}")]
        public StatusResponseModel Delete(int id)
        {
            StatusResponseModel _objResponseModel = new StatusResponseModel();

            string query = @"delete from User_Details where id=@id";

            DataTable table = new DataTable();

            string sqlDataSource = _configuration.GetConnectionString("Default");
            SqlDataReader myReader;

            using (SqlConnection myCon = new SqlConnection(sqlDataSource))
            {
                myCon.Open();
                using (SqlCommand myCommand = new SqlCommand(query, myCon))
                {
                    myCommand.Parameters.AddWithValue("@id", id);
                    myReader = myCommand.ExecuteReader();
                    table.Load(myReader);
                    myReader.Close();
                    myCon.Close();
                }

                _objResponseModel.Status = true;
                _objResponseModel.Message = "User id data deleted successfully";
                return _objResponseModel;
            }

        }
    }
}

