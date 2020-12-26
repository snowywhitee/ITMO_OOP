using DAL;
using ReportMaker.BLL;
using System;
using System.Collections.Generic;

namespace BLL
{

    public class ProjectTaskController
    {
        private IRepository _repository;
        private IRepository _reports;
        private System.Timers.Timer _timer;
        
        public ProjectTaskController(string tasksPath, string reportsPath)
        {
            _repository = new FileRepository(tasksPath);
            _reports = new FileRepository(reportsPath);
            _timer = new System.Timers.Timer();
            //_timer.Interval = 1000 * 60 * 60 * 24;
            //For testing
            _timer.Interval = 1000*2;
            _timer.Elapsed += OnTimerEvent;
            _timer.AutoReset = true;
            _timer.Enabled = true;
        }
        public void MakeSprintReport()
        {
            Report report = new Report(GetAllReports());
            _reports.Add(report);
            _timer.Enabled = false;
        }
        public void MakeReports()
        {
            Console.WriteLine("Making reports!!");
            foreach (var item in TeamMemberController.GetAll())
            {
                MakeReport(item.Id);
            }
        }
        public List<Report> GetAllReports(int teamMemberId)
        {
            var teamMember = TeamMemberController.GetTeamMember(teamMemberId);
            var ids = new HashSet<int>();
            ids.Add(teamMemberId);
            foreach (var item in teamMember.Team)
            {
                ids.Add(item.Key);
            }
            var reports = GetAllReports();
            for (int i = 0; i < reports.Count; i++)
            {
                if (ids.Contains(reports[i].OwnerId))
                {
                    reports.Add(reports[i]);
                }
            }

            return reports;
        }
        public List<Report> GetAllReports()
        {
            var reports = _reports.GetAll();
            var result = new List<Report>();
            for (int i = 0; i < reports.Count; i++)
            {
                result.Add(reports[i] as Report);
            }

            return result;
        }
        public List<ProjectTask> GetAll(int teamMemberId)
        {
            var tasks = GetAll();
            var result = new List<ProjectTask>();
            for (int i = 0; i < tasks.Count; i++)
            {
                if ((tasks[i] as ProjectTask).Owner != null && (tasks[i] as ProjectTask).Owner.Id == teamMemberId)
                {
                    result.Add(tasks[i] as ProjectTask);
                }
            }

            return result;
        }
        public List<ProjectTask> GetAll()
        {
            var tasks = _repository.GetAll();
            var result = new List<ProjectTask>();
            for (int i = 0; i < tasks.Count; i++)
            {
                result.Add(tasks[i] as ProjectTask);
            }

            return result;
        }
        public ProjectTask GetTask(int taskId)
        {
            return TryGet(taskId);
        }
        public void RemoveTask(int id)
        {
            ProjectTask task = TryGet(id);
            _repository.Delete(task);
        }
        public int AddTask(int teamMemberId, string name, string description)
        {
            ProjectTask task = new ProjectTask(name, description);
            task.Owner = TeamMember.ProjectTeam[teamMemberId];
            _repository.Add(task);
            return task.Id;
        }
        public int AddTask(string name, string description)
        {
            ProjectTask task = new ProjectTask(name, description);
            _repository.Add(task);
            return task.Id;
        }

        //Editing
        public void EditTaskName(int id, string name)
        {
            ProjectTask task = TryGet(id);
            task.Name = name;
            _repository.Edit(task);
        }
        public void EditTaskDescription(int id, string description)
        {
            ProjectTask task = TryGet(id);
            task.Description = description;
            _repository.Edit(task);
        }
        public void EditTaskOwner(int id, int teamMemberId)
        {
            ProjectTask task = TryGet(id);
            task.Owner = TeamMemberController.GetTeamMember(id);
            _repository.Edit(task);
        }
        public void EditTaskState(int id, ProjectTaskState state)
        {
            ProjectTask task = TryGet(id);
            task.State = state;
            _repository.Edit(task);
        }
        public void AddComment(int id, string comment)
        {
            ProjectTask task = TryGet(id);
            task.AddComment(comment);
            _repository.Edit(task);
        }

        //Helper
        private ProjectTask TryGet(int id)
        {
            ProjectTask task = (ProjectTask)_repository.Get(id);
            if (task == null)
            {
                throw new Exception($"No task found with id {id} in the current dataBase");
            }
            return task;
        }
        private void MakeReport(int teamMemberId)
        {
            var reports = GetAllReports(teamMemberId);
            if (reports.Count == 0)
            {
                _reports.Add(new Report(teamMemberId, GetAll(teamMemberId)));
            }
            else
            {
                DateTime last = DateTime.MinValue;
                for (int i = 0; i < reports.Count; i++)
                {
                    if (reports[i].Time > last)
                    {
                        last = reports[i].Time;
                    }
                }
                Report report = new Report(reports.Find(x => x.Time == last), teamMemberId, GetAll(teamMemberId));
                _reports.Add(report);
            }
        }
        private void OnTimerEvent(Object source, System.Timers.ElapsedEventArgs e)
        {
            MakeReports();
        }
    }
}
