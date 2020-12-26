using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class FileRepository : IRepository
    {
        private string _path;
        private List<IEntity> _entities = new List<IEntity>();
        
        public FileRepository(string path)
        {
            //if (!File.Exists(path))
            //{
            //    File.Create(path);
            //}
            _path = path;
        }
        public void Add(IEntity entity)
        {
            _entities.Add(entity);
            //using (FileStream fs = File.OpenWrite(_path))
            //{
            //    using (StreamWriter sw = new StreamWriter(fs))
            //    {
            //        sw.WriteLineAsync(entity.ToString());
            //    }
            //}
        }

        public void Delete(IEntity entity)
        {
            _entities.Remove(entity);
        }

        public void Edit(IEntity entity)
        {
            if (!_entities.Contains(entity))
            {
                throw new RepositoryException($"Entry {entity.Id} not found");
            }
            _entities[_entities.IndexOf(_entities.Find(x => x.Id == entity.Id))] = entity;
        }

        public IEntity Get(int id)
        {
            return _entities.Find(x => x.Id == id);
        }

        public List<IEntity> GetAll()
        {
            return _entities;
        }
    }
}
