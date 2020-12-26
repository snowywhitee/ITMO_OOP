using BLL;
using DAL;
using System;
using System.Collections.Generic;
using System.Text;

namespace ReportMaker.BLL
{
    public class Report : IEntity
    {
        private static int idCount = 0;
        private int _id;
        private DateTime _time;
        private int _ownerId;
        private Report _parent;
        public int Id { get => _id; }
        public int OwnerId { get => _ownerId; }
        public DateTime Time { get => _time; }

        public Dictionary<int, List<Change>> _data = new Dictionary<int, List<Change>>(); 
        public Dictionary<int, List<Change>> Data { get => _data; }
        public Report(List<Report> reports)
        {
            foreach (var report in reports)
            {
                foreach (var data in report.Data)
                {
                    if (_data.ContainsKey(data.Key))
                    {
                        _data[data.Key].AddRange(data.Value);
                    }
                    else
                    {
                        _data.Add(data.Key, data.Value);
                    }
                }
            }
        }
        public Report(int teamMemberId, List<ProjectTask> tasks)
        {
            _id = idCount++;
            _time = DateTime.Now;
            _ownerId = teamMemberId;
            foreach (var item in tasks)
            {
                _data.Add(item.Id, item.Changes);
            }
        }
        public Report(Report parent, int teamMemberId, List<ProjectTask> tasks)
        {
            _id = idCount++;
            _time = DateTime.Now;
            _parent = parent;
            _ownerId = teamMemberId;
            foreach (var item in tasks)
            {
                if (!parent.Data.ContainsKey(item.Id))
                {
                    _data.Add(item.Id, item.Changes);
                }
                else
                {
                    var deltaChanges = GetChanges(item.Id, item.Changes);
                    if (deltaChanges.Count != 0)
                    {
                        _data.Add(item.Id, deltaChanges);
                    }
                }
            }
        }
        private List<Change> GetChanges(int id, List<Change> changes)
        {
            var parentChanges = _parent.Data[id];
            var newChanges = new List<Change>();
            foreach (var item in changes)
            {
                if (!parentChanges.Contains(item))
                {
                    newChanges.Add(item);
                }
            }
            return newChanges;
        }
        public void PrintData()
        {
            foreach (var item in _data)
            {
                Console.WriteLine($"Task{item.Key}:");
                foreach (var change in item.Value)
                {
                    Console.WriteLine($"{change.Time}   {change.ChangedProperty.Item1} -> {change.ChangedProperty.Item2}");
                }
            }
            Console.WriteLine();
        }
    }
}
