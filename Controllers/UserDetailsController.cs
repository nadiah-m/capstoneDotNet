using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using capstoneDotNet.Models;
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

        public UsersController(IConfiguration config, IWebHostEnvironment env)
        {
            _configuration = config;
            _env = env;
        }

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
                temp.password = table.Rows[i]["password"].ToString();
                temp.role = table.Rows[i]["role"].ToString();
                temp.designation = table.Rows[i]["designation"].ToString();
                userList.Add(temp);
            }

            _objResponseModel.Data = userList;
            _objResponseModel.Status = true;
            _objResponseModel.Message = "User details data received successfully";
            return _objResponseModel;
        }

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


        [Route("signup")]
        [HttpPost]
        public StatusResponseModel Post (UserDetails userdata)
        {
            StatusResponseModel _objResponseModel = new StatusResponseModel();

            string queryEmail = @"select count(*) from User_Details where email = @email";

            int RowExists = 0;

            string query = @"insert into User_Details(first_name, last_name, email, password) values (@first_name, @last_name, @email, @password)";

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
                    _objResponseModel.Message = "The email already exist. Please login";
                }

                else
                {
                    using (SqlCommand myCommand = new SqlCommand(query, myCon))
                    {
                        myCommand.Parameters.AddWithValue("@first_name", userdata.firstName);
                        myCommand.Parameters.AddWithValue("@last_name", userdata.lastName);
                        myCommand.Parameters.AddWithValue("@email", userdata.email);
                        myCommand.Parameters.AddWithValue("@password", userdata.password);
                        myReader = myCommand.ExecuteReader();
                        table.Load(myReader);
                    }
                    _objResponseModel.Message = "User account created successfully";

                    myReader.Close();
                    myCon.Close();

                }
                
             }
            
            _objResponseModel.Status = true;
            return _objResponseModel;
        }

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

