using ProductTracking.Models;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Entities;
using System.Transactions;

namespace ProductTracking.App_Start
{
    public class InitData
    {
        static public void init()
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                if (db.QueryWorkflows.Count() == 0)
                {
                    string path = Path.Combine(HttpContext.Current.Server.MapPath("~"), "InitData");
                    string fileName = Path.Combine(path, "QueryWorkflow.csv");
                    if (File.Exists(fileName))
                    {
                        var lines = File.ReadAllLines(fileName);
                        int count = 1;
                        var usernameWiseIds = db.Users.GroupBy(u => u.UserName).ToDictionary(d => d.Key, d => d.ToList().First().Id);

                        foreach (var line in lines)
                        {
                            QueryWorkflow queryWorkflow = new QueryWorkflow();
                            queryWorkflow.Name = $"Workflow - {count++}";

                            var users = line.Split(',');
                            for (int i = 1; i <= users.Length; i++)
                            {
                                string userName = users[i - 1];
                                if (usernameWiseIds.ContainsKey(userName))
                                {
                                    queryWorkflow.WorkflowUsers.Add(new QueryWorkflowUser
                                    {
                                        Role = (QueryRole)i,
                                        UserId = usernameWiseIds[userName],
                                        Workflow = queryWorkflow
                                    });
                                }
                            }
                            db.QueryWorkflows.Add(queryWorkflow);
                        }
                        db.SaveChanges();
                    }
                    else
                        throw new FileNotFoundException($"Default Query Workflows Not Found In The Database. Please Create {fileName}.");
                }
            }
        }
    }
}