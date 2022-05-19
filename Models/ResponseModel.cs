using System;
namespace capstoneDotNet.Models
{
	public class ResponseModel
	{
		public string Message { set; get; }
		public bool Status { set; get; }
		public List<dynamic> Data { set; get; }
	}

	public class StatusResponseModel
	{
		public string Message { set; get; }
		public bool Status { set; get; }
	}
}

