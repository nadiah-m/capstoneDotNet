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
    public class ClientController : ControllerBase
    {
        private IConfiguration _configuration;
        private readonly IWebHostEnvironment _env;

        public ClientController(IConfiguration config, IWebHostEnvironment env) //constructor
        {
            _configuration = config;
            _env = env;
        }

        [HttpGet]
        public ResponseModel Get()
        {
            ResponseModel _objResponseModel = new ResponseModel();

            string query = @"
                            select * from Client_Details
                            ";

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

            List<dynamic> clientList = new List<dynamic>();
            for (int i = 0; i < table.Rows.Count; i++)
            {
                Client temp = new Client();
                temp.id = Convert.ToInt32(table.Rows[i]["id"]);
                temp.clientName = table.Rows[i]["client_name"].ToString();
                temp.clientDesc = table.Rows[i]["client_description"].ToString();

                clientList.Add(temp);
            }

            _objResponseModel.Data = clientList;
            _objResponseModel.Status = true;
            _objResponseModel.Message = "Client details data received successfully";
            return _objResponseModel;

        }

        [HttpGet("{id}")]
        public ResponseModel Get(int id)
        {
            ResponseModel _objResponseModel = new ResponseModel();

            string query = @"
                            select * from Client_Details where id = @id
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

            List<dynamic> clientList = new List<dynamic>();

            if (table.Rows.Count == 0)
            {
                _objResponseModel.Message = "No such id can be found";
            }
            else
            {
                for (int i = 0; i < table.Rows.Count; i++)
                {
                    Client temp = new Client();
                    temp.id = Convert.ToInt32(table.Rows[i]["id"]);
                    temp.clientName = table.Rows[i]["client_name"].ToString();
                    temp.clientDesc = table.Rows[i]["client_description"].ToString();

                    clientList.Add(temp);
                }
                _objResponseModel.Message = "Client id data received successfully";
            }

            _objResponseModel.Data = clientList;
            _objResponseModel.Status = true;
            //status if success or fail

            return _objResponseModel;
        }

        [HttpPost]
        public StatusResponseModel Post(Client clientdata)
        {
            StatusResponseModel _objResponseModel = new StatusResponseModel();

            string queryProject = @"select count(*) from Client_Details where client_name = @client_name";

            int RowExists = 0;

            string query = @"
                            insert into Client_Details (client_name,client_description) values (@client_name,@client_description)
                            ";

            DataTable table = new DataTable();

            string sqlDataSource = _configuration.GetConnectionString("Default");
            SqlDataReader myReader;

            using (SqlConnection myCon = new SqlConnection(sqlDataSource))
            {
                myCon.Open();

                using (SqlCommand findClient = new SqlCommand(queryProject, myCon))
                {
                    findClient.Parameters.AddWithValue("@client_name", clientdata.clientName);
                    RowExists = (int)findClient.ExecuteScalar();
                }

                if (RowExists == 1)
                {
                    _objResponseModel.Message = "The client already exist. Please create a new client";
                }
                else
                {
                    using (SqlCommand myCommand = new SqlCommand(query, myCon))
                    {
                        myCommand.Parameters.AddWithValue("@client_name", clientdata.clientName);
                        myCommand.Parameters.AddWithValue("@client_description", clientdata.clientDesc);
                        
                        myReader = myCommand.ExecuteReader();
                        table.Load(myReader);
                    }
                    _objResponseModel.Message = "New client created successfully";

                    myReader.Close();
                    myCon.Close();
                }

            }

            _objResponseModel.Status = true;

            return _objResponseModel;
        }

        [HttpPut]
        public StatusResponseModel Put(Client clientdata)
        {
            StatusResponseModel _objResponseModel = new StatusResponseModel();

            string query = @"
                            update Client_Details set
                            client_name = @client_name,
                            client_description = @client_description
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
                    myCommand.Parameters.AddWithValue("@id", clientdata.id);
                    myCommand.Parameters.AddWithValue("@client_name", clientdata.clientName);
                    myCommand.Parameters.AddWithValue("@client_description", clientdata.clientDesc);
                    
                    myReader = myCommand.ExecuteReader();
                    table.Load(myReader);
                    myReader.Close();
                    myCon.Close();
                }
            }

            _objResponseModel.Status = true;
            _objResponseModel.Message = "Client data updated successfully";
            return _objResponseModel;
        }

        [HttpDelete("{id}")]
        public StatusResponseModel Delete(int id)
        {
            StatusResponseModel _objResponseModel = new StatusResponseModel();

            string query = @"delete from Client_Details where id=@id";

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
                _objResponseModel.Message = "Client data deleted successfully";
                return _objResponseModel;
            }

        }

    }
}

