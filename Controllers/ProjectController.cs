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
    public class ProjectController : ControllerBase
    {
        private IConfiguration _configuration;
        private readonly IWebHostEnvironment _env;

        public ProjectController(IConfiguration config, IWebHostEnvironment env)
        {
            _configuration = config;
            _env = env;
        }

        [HttpGet]
        public ResponseModel Get()
        {
            ResponseModel _objResponseModel = new ResponseModel();

            string query = @"select * from Project_Details";

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

            List<dynamic> projectList = new List<dynamic>();
            for (int i = 0; i < table.Rows.Count; i++)
            {
                Project temp = new Project();
                temp.id = Convert.ToInt32(table.Rows[i]["id"]);
                temp.projectName = table.Rows[i]["project_name"].ToString();
                temp.startDate = table.Rows[i]["start_date"].ToString();
                temp.projectDesc = table.Rows[i]["project_description"].ToString();
                temp.projectCost = Convert.ToInt64(table.Rows[i]["project_cost"]);
                temp.currentExp = Convert.ToInt64(table.Rows[i]["current_expenditure"]);
                temp.availFunds = Convert.ToInt64(table.Rows[i]["avail_funds"]);
                temp.status = table.Rows[i]["status"].ToString();
                temp.clientId = Convert.ToInt32(table.Rows[i]["client_id"]);
                projectList.Add(temp);
            }

            _objResponseModel.Data = projectList;
            _objResponseModel.Status = true;
            _objResponseModel.Message = "Project details data received successfully";
            return _objResponseModel;
        }

        [HttpGet("{id}")]
        public ResponseModel Get(int id)
        {
            ResponseModel _objResponseModel = new ResponseModel();

            string query = @"
                            select * from Project_Details where id = @id
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

            List<dynamic> projectList = new List<dynamic>();

            if (table.Rows.Count == 0)
            {
                _objResponseModel.Message = "No such id can be found";
            }
            else
            {
                for (int i = 0; i < table.Rows.Count; i++)
                {
                    Project temp = new Project();
                    temp.id = Convert.ToInt32(table.Rows[i]["id"]);
                    temp.projectName = table.Rows[i]["project_name"].ToString();
                    temp.startDate = table.Rows[i]["start_date"].ToString();
                    temp.projectDesc = table.Rows[i]["project_description"].ToString();
                    temp.projectCost = Convert.ToInt64(table.Rows[i]["project_cost"]);
                    temp.currentExp = Convert.ToInt64(table.Rows[i]["current_expenditure"]);
                    temp.availFunds = Convert.ToInt64(table.Rows[i]["avail_funds"]);
                    temp.status = table.Rows[i]["status"].ToString();
                    temp.clientId = Convert.ToInt32(table.Rows[i]["client_id"]);
                    projectList.Add(temp);
                }
                _objResponseModel.Message = "Project id data received successfully";
            }

            _objResponseModel.Data = projectList;
            _objResponseModel.Status = true;
            //status if success or fail

            return _objResponseModel;
        }

        //get list of users involved in project

        [Route("team")]
        [HttpGet()]
        public ResponseModel GetTeam(int id)
        {
            ResponseModel _objResponseModel = new ResponseModel();

            string query = @"
                            select *
                            from ProjectUserRelation  
                            INNER join User_Details on id = user_id
                            where project_id = @project_id
                            
                            ";

            DataTable table = new DataTable();

            string sqlDataSource = _configuration.GetConnectionString("Default");
            SqlDataReader myReader;

            using (SqlConnection myCon = new SqlConnection(sqlDataSource))
            {
                myCon.Open();
                using (SqlCommand myCommand = new SqlCommand(query, myCon))
                {
                    myCommand.Parameters.AddWithValue("@project_id", id);
                    myReader = myCommand.ExecuteReader();
                    table.Load(myReader);
                    myReader.Close();
                    myCon.Close();
                }
            }

            List<dynamic> teamList = new List<dynamic>();

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
                    temp.role = table.Rows[i]["role"].ToString();
                    temp.designation = table.Rows[i]["designation"].ToString();
                    teamList.Add(temp);
                }
                _objResponseModel.Message = "Team data for project received successfully";
            }

            _objResponseModel.Data = teamList;
            _objResponseModel.Status = true;
            //status if success or fail

            return _objResponseModel;
        }

        //get list of projects assigned to users

        [Route("UserProjects")]
        [HttpGet()]
        public ResponseModel GetProjectsUsers(int id)
        {
            ResponseModel _objResponseModel = new ResponseModel();

            string query = @"
                            select *
                            from ProjectUserRelation
                            INNER join Project_Details on id = project_id
                            where user_id = @user_id
                            ";

            DataTable table = new DataTable();

            string sqlDataSource = _configuration.GetConnectionString("Default");
            SqlDataReader myReader;

            using (SqlConnection myCon = new SqlConnection(sqlDataSource))
            {
                myCon.Open();
                using (SqlCommand myCommand = new SqlCommand(query, myCon))
                {
                    myCommand.Parameters.AddWithValue("@user_id", id);
                    myReader = myCommand.ExecuteReader();
                    table.Load(myReader);
                    myReader.Close();
                    myCon.Close();
                }
            }

            List<dynamic> projectUserList = new List<dynamic>();

            if (table.Rows.Count == 0)
            {
                _objResponseModel.Message = "No such id can be found";
            }
            else
            {
                for (int i = 0; i < table.Rows.Count; i++)
                {
                    Project temp = new Project();
                    temp.id = Convert.ToInt32(table.Rows[i]["id"]);
                    temp.projectName = table.Rows[i]["project_name"].ToString();
                    temp.startDate = table.Rows[i]["start_date"].ToString();
                    temp.projectDesc = table.Rows[i]["project_description"].ToString();
                    temp.projectCost = Convert.ToInt64(table.Rows[i]["project_cost"]);
                    temp.currentExp = Convert.ToInt64(table.Rows[i]["current_expenditure"]);
                    temp.availFunds = Convert.ToInt64(table.Rows[i]["avail_funds"]);
                    temp.status = table.Rows[i]["status"].ToString();
                    temp.clientId = Convert.ToInt32(table.Rows[i]["client_id"]);
                    projectUserList.Add(temp);
                }
                _objResponseModel.Message = "Project data for user received successfully";
            }

            _objResponseModel.Data = projectUserList;
            _objResponseModel.Status = true;
            //status if success or fail

            return _objResponseModel;
        }

        
        //link user to project

        [Route("UserProjects")]
        [HttpPost]
        public StatusResponseModel Post(int userId, int projectId)
        {
            StatusResponseModel _objResponseModel = new StatusResponseModel();

            string query = @"insert into ProjectUserRelation (user_id,project_id) values (@user_id, @project_id)";
            DataTable table = new DataTable();

            string sqlDataSource = _configuration.GetConnectionString("Default");
            SqlDataReader myReader;

            using (SqlConnection myCon = new SqlConnection(sqlDataSource))
            {
                myCon.Open();
                using (SqlCommand myCommand = new SqlCommand(query, myCon))
                {
                    myCommand.Parameters.AddWithValue("@user_id", userId);
                    myCommand.Parameters.AddWithValue("@project_id", projectId);
                    myReader = myCommand.ExecuteReader();
                    table.Load(myReader);
                }

                _objResponseModel.Message = "New project user relation data created successfully";

                myReader.Close();
                myCon.Close();
            }

            _objResponseModel.Status = true;

            return _objResponseModel;
        }





        [HttpPost]
        public StatusResponseModel Post(Project projectdata)
        {
            StatusResponseModel _objResponseModel = new StatusResponseModel();

            string queryProject = @"select count(*) from Project_Details where project_name = @project_name";

            int RowExists = 0;

            string query = @"
                            insert into Project_Details(project_name,start_date,project_description,project_cost,current_expenditure,avail_funds,status,client_id) values (@project_name,@start_date,@project_description,@project_cost,@current_expenditure,@avail_funds,@status,@client_id)
                            ";

            DataTable table = new DataTable();

            string sqlDataSource = _configuration.GetConnectionString("Default");
            SqlDataReader myReader;

            using (SqlConnection myCon = new SqlConnection(sqlDataSource))
            {
                myCon.Open();

                using (SqlCommand findProject = new SqlCommand(queryProject, myCon))
                {
                    findProject.Parameters.AddWithValue("@project_name", projectdata.projectName);
                    RowExists = (int)findProject.ExecuteScalar();
                }

                if (RowExists == 1)
                {
                    _objResponseModel.Message = "The project already exist. Please create a new project";
                }
                else
                {
                    using (SqlCommand myCommand = new SqlCommand(query, myCon))
                    {
                        myCommand.Parameters.AddWithValue("@project_name", projectdata.projectName);
                        myCommand.Parameters.AddWithValue("@start_date", projectdata.startDate);
                        myCommand.Parameters.AddWithValue("@project_description", projectdata.projectDesc);
                        myCommand.Parameters.AddWithValue("@project_cost", projectdata.projectCost);
                        myCommand.Parameters.AddWithValue("@current_expenditure", projectdata.currentExp);
                        myCommand.Parameters.AddWithValue("@avail_funds", projectdata.availFunds);
                        myCommand.Parameters.AddWithValue("@status", projectdata.status);
                        myCommand.Parameters.AddWithValue("@client_id", projectdata.clientId);
                        myReader = myCommand.ExecuteReader();
                        table.Load(myReader);
                    }
                    _objResponseModel.Message = "New project created successfully";

                    myReader.Close();
                    myCon.Close();
                }
            }

            _objResponseModel.Status = true;

            return _objResponseModel;
        }

        [HttpPut]
        public StatusResponseModel Put(Project projectdata)
        {
            StatusResponseModel _objResponseModel = new StatusResponseModel();

            string query = @"
                            update Project_Details set
                            project_name = @project_name,       
                            start_date = @start_date,
                            project_description = @project_description,
                            project_cost = @project_cost,
                            current_expenditure = @current_expenditure,
                            avail_funds = @avail_funds,    
                            status = @status,
                            client_id = @client_id
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
                    myCommand.Parameters.AddWithValue("@id", projectdata.id);
                    myCommand.Parameters.AddWithValue("@project_name", projectdata.projectName);
                    myCommand.Parameters.AddWithValue("@start_date", projectdata.startDate);
                    myCommand.Parameters.AddWithValue("@project_description", projectdata.projectDesc);
                    myCommand.Parameters.AddWithValue("@project_cost", projectdata.projectCost);
                    myCommand.Parameters.AddWithValue("@current_expenditure", projectdata.currentExp);
                    myCommand.Parameters.AddWithValue("@avail_funds", projectdata.availFunds);
                    myCommand.Parameters.AddWithValue("@status", projectdata.status);
                    myCommand.Parameters.AddWithValue("@client_id", projectdata.clientId);
                    myReader = myCommand.ExecuteReader();
                    table.Load(myReader);
                    myReader.Close();
                    myCon.Close();
                }
            }

            _objResponseModel.Status = true;
            _objResponseModel.Message = "Project data updated successfully";
            return _objResponseModel;
        }

        //delete user link to project
        [Route("UserProjects")]
        [HttpDelete]
        public StatusResponseModel Delete(int userId, int projectId)
        {
            StatusResponseModel _objResponseModel = new StatusResponseModel();

            string query = @"delete from ProjectUserRelation where user_id = @user_id and project_id = @project_id";

            DataTable table = new DataTable();

            string sqlDataSource = _configuration.GetConnectionString("Default");
            SqlDataReader myReader;

            using (SqlConnection myCon = new SqlConnection(sqlDataSource))
            {
                myCon.Open();
                using (SqlCommand myCommand = new SqlCommand(query, myCon))
                {
                    myCommand.Parameters.AddWithValue("@user_id", userId);
                    myCommand.Parameters.AddWithValue("@project_id", projectId);
                    myReader = myCommand.ExecuteReader();
                    table.Load(myReader);
                    myReader.Close();
                    myCon.Close();
                }

                _objResponseModel.Status = true;
                _objResponseModel.Message = "Project user relation data deleted successfully";
                return _objResponseModel;
            }

        }


        [HttpDelete("{id}")]
        public StatusResponseModel Delete(int id)
        {
            StatusResponseModel _objResponseModel = new StatusResponseModel();

            string query = @"delete from Project_Details where id=@id";

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
                _objResponseModel.Message = "Project data deleted successfully";
                return _objResponseModel;
            }

        }

        
    }
}
