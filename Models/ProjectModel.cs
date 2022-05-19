using System;
namespace capstoneDotNet.Models
{
	public class Project
	{
		public int id { get; set; }
		public string projectName { get; set; }
		public string startDate { get; set; }
		public string projectDesc { get; set; }
		public int? projectMgrId { get; set; }
		public long projectCost { get; set; }
		public long? currentExp { get; set; }
		public long? availFunds { get; set; }
		public string status { get; set; }
		public int clientId { get; set; }
		

	}
}

