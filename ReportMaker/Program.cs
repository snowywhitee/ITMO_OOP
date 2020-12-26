using BLL;
using System;
using System.Threading;

namespace ReportMaker
{
    class Program
    {
        static void Main(string[] args)
        {
            //Make a team ex

            int teamLead = TeamMemberController.AddTeamMember("Vasiliy");
            int developer = TeamMemberController.AddTeamMember("Anton");
            int juniorDeveloper = TeamMemberController.AddTeamMember("Alexander");
            int tester = TeamMemberController.AddTeamMember("Katya");

            //Manage teams

            TeamMemberController.AddToTeam(teamLead, developer);
            TeamMemberController.AddToTeam(teamLead, tester);
            TeamMemberController.AddToTeam(developer, juniorDeveloper);

            var taskController = new ProjectTaskController("path", "path");
            int task1 = taskController.AddTask("Task1", "Make project wiki");
            int task2 = taskController.AddTask("Task2", "Add structure");
            int task3 = taskController.AddTask("Task3", "Make tests");

            //Print tasks state

            Console.WriteLine(taskController.GetTask(task1).State);
            Console.WriteLine(taskController.GetTask(task2).State);
            Console.WriteLine(taskController.GetTask(task3).State);

            //Assign

            taskController.EditTaskOwner(task1, juniorDeveloper);
            taskController.EditTaskOwner(task2, developer);
            taskController.EditTaskOwner(task3, tester);

            //Emitate work

            taskController.EditTaskDescription(task1, "some better description");
            taskController.EditTaskState(task2, ProjectTaskState.Active);
            taskController.EditTaskOwner(task3, teamLead);
            Console.WriteLine("Edited!");
            taskController.EditTaskDescription(task1, "even more better description");
            
            Thread.Sleep(5000);
            //Check our reports!
            foreach (var item in taskController.GetAllReports())
            {
                item.PrintData();
            }
        }
    }
}
